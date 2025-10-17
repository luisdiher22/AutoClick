using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "AutoClick.Session";
});

// Add Entity Framework - SQLite for development, SQL Server for production
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use SQLite for local development to preserve Azure credits
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        // Use SQL Server for production (Azure)
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

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

// Add Auto Service
builder.Services.AddScoped<IAutoService, AutoService>();

// Add Authentication Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Soporte Service
builder.Services.AddScoped<ISoporteService, SoporteService>();

// Add Banderines Service
builder.Services.AddScoped<IBanderinesService, BanderinesService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
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
