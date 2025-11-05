using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure encoding to support UTF-8 characters
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = true; // Enable sampling to reduce telemetry volume and DB wake-ups
    options.EnableQuickPulseMetricStream = false; // Disable live metrics to reduce constant connections
});

// Add services to the container.
builder.Services.AddRazorPages();

// Configure localization and encoding
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});

// Add Controllers (for API endpoints)
builder.Services.AddControllers();

// Add Authentication services
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.Name = "AutoClick.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? 
            CookieSecurePolicy.None : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache(); // Agregar Memory Cache para performance
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "AutoClick.Session";
});

// Add Entity Framework - Azure SQL Database for all environments
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // Use Azure SQL Server for all environments
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Enable connection resiliency for Azure SQL Database
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        
        // Set command timeout for long-running operations (30 seconds reduced from 60)
        sqlOptions.CommandTimeout(30);
    });
    
    // Enable sensitive data logging in development for debugging
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
    
    // Add performance optimizations
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}, ServiceLifetime.Scoped, ServiceLifetime.Singleton); // Explicitly set lifetimes

// Add Storage Service - Local for development, Azure for production
builder.Services.AddScoped<IStorageService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var useAzureStorage = configuration.GetValue<bool>("UseAzureStorage");
    var logger = provider.GetRequiredService<ILoggerFactory>();
    
    if (useAzureStorage)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage") 
                              ?? Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Storage connection string is required when UseAzureStorage is true. Set it in appsettings or AZURE_STORAGE_CONNECTION_STRING environment variable");
        }
        var azureLogger = logger.CreateLogger<AzureStorageService>();
        return new AzureStorageService(connectionString, azureLogger);
    }
    else
    {
        var localPath = configuration.GetValue<string>("LocalStoragePath") ?? "LocalStorage";
        var localLogger = logger.CreateLogger<LocalStorageService>();
        return new LocalStorageService(localLogger, localPath);
    }
});

// Add File Upload Service
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Add Publicidad Storage Service (para imágenes de anuncios)
builder.Services.AddScoped<IPublicidadStorageService, PublicidadStorageService>();

// Add Auto Service
builder.Services.AddScoped<IAutoService, AutoService>();

// Add Authentication Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Soporte Service
builder.Services.AddScoped<ISoporteService, SoporteService>();

// Add Banderines Service
builder.Services.AddScoped<IBanderinesService, BanderinesService>();

// Add Tasa Cambio Service (Singleton para caché compartido)
builder.Services.AddSingleton<ITasaCambioService, TasaCambioService>();

// Add Ventas Externas Service
builder.Services.AddScoped<IVentasExternasService, VentasExternasService>();

// Add Email Service para notificaciones
builder.Services.AddScoped<IEmailService, EmailService>();

// Add Application Insights Service
builder.Services.AddScoped<IApplicationInsightsService, ApplicationInsightsService>();

// Add Image Processing Service para publicidad
builder.Services.AddScoped<IImageProcessingService, ImageProcessingService>();

// Add HttpClient Factory para TasaCambioService
builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Inicializar PrecioHelper con el servicio de tasa de cambio
using (var scope = app.Services.CreateScope())
{
    var tasaCambioService = scope.ServiceProvider.GetRequiredService<ITasaCambioService>();
    AutoClick.Helpers.PrecioHelper.Initialize(tasaCambioService);
    
    // Obtener tasa inicial de forma asíncrona
    _ = Task.Run(async () => 
    {
        try
        {
            var tasa = await tasaCambioService.ObtenerTasaCambioUSDaCRC();
            AutoClick.Helpers.PrecioHelper.ActualizarTasaCacheada(tasa);
        }
        catch (Exception ex)
        {
            // Log error pero no bloquear inicio de app
            Console.WriteLine($"Error al obtener tasa inicial: {ex.Message}");
        }
    });
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Force UTF-8 encoding ONLY for HTML responses (not for static files like CSS/JS)
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        // Only set UTF-8 for HTML responses, don't override CSS, JS, or other static files
        if (context.Response.ContentType?.StartsWith("text/html") == true && 
            !context.Response.ContentType.Contains("charset"))
        {
            context.Response.ContentType = "text/html; charset=utf-8";
        }
        return Task.CompletedTask;
    });
    await next();
});

app.UseStaticFiles();

app.UseRouting();

// Add authentication middleware BEFORE authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Map API controllers
app.MapControllers();

// Create admin user if it doesn't exist
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    
    try
    {
        // Check if admin user exists
        var adminExists = await authService.EmailExistsAsync("admin@gmail.com");
        if (!adminExists)
        {
            Console.WriteLine("[INIT] Creating admin user...");
            var adminUser = new Usuario
            {
                Email = "admin@gmail.com",
                Nombre = "Admin",
                Apellidos = "User",
                NumeroTelefono = "12345678",
                NombreAgencia = null
            };
            
            var result = await authService.RegisterAsync(adminUser, "prueba123");
            if (result.Success)
            {
                Console.WriteLine("[INIT] Admin user created successfully!");
            }
            else
            {
                Console.WriteLine($"[INIT] Failed to create admin user: {result.Message}");
            }
        }
        else
        {
            Console.WriteLine("[INIT] Admin user already exists");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[INIT] Error initializing admin user: {ex.Message}");
    }
});

app.Run();
