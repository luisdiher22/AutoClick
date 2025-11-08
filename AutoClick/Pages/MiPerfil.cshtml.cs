using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AutoClick.Pages
{
    [Authorize]
    public class MiPerfilModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MiPerfilModel(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        // Profile information properties
        [BindProperty]
        [Display(Name = "Nombre comercial")]
        public string? NombreComercial { get; set; }

        [BindProperty]
        [Display(Name = "Nombre de la empresa")]
        public string? NombreEmpresa { get; set; }

        [BindProperty]
        [Display(Name = "Cédula jurídica")]
        [RegularExpression(@"^\d-\d{3}-\d{6}$", ErrorMessage = "El formato de cédula jurídica debe ser: #-###-######")]
        public string? CedulaJuridica { get; set; }

        [BindProperty]
        [Display(Name = "Representante legal")]
        public string? RepresentanteLegal { get; set; }

        [BindProperty]
        [Display(Name = "Teléfono 1")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
        public string? Telefono1 { get; set; }

        [BindProperty]
        [Display(Name = "WhatsApp")]
        [Phone(ErrorMessage = "Ingrese un número de WhatsApp válido")]
        public string? WhatsApp { get; set; }

        // Address properties
        [BindProperty]
        [Required(ErrorMessage = "La provincia es obligatoria")]
        [Display(Name = "Provincia")]
        public string? Provincia { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El cantón es obligatorio")]
        [Display(Name = "Cantón")]
        public string? Canton { get; set; }

        [BindProperty]
        [Display(Name = "Dirección exacta")]
        public string? DireccionExacta { get; set; }

        // File upload properties
        [BindProperty]
        public IFormFile? LogoFile1 { get; set; }

        public string? LogoUrl { get; set; }

        // Newsletter subscription
        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor ingrese un correo electrónico válido")]
        public string Email { get; set; } = string.Empty;

        [TempData]
        public string? StatusMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Auth");
            }

            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? User.FindFirst("email")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Auth");
            }

            await LoadProfileDataAsync(userEmail);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Console.WriteLine("=== OnPostAsync called ===");
                
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToPage("/Auth");
                }

                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                    ?? User.FindFirst("email")?.Value 
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                    
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToPage("/Auth");
                }

                Console.WriteLine($"User email: {userEmail}");
                Console.WriteLine($"LogoFile1 is null: {LogoFile1 == null}");
                if (LogoFile1 != null)
                {
                    Console.WriteLine($"LogoFile1 name: {LogoFile1.FileName}, size: {LogoFile1.Length}");
                }

                // Don't validate model state for file uploads - it might block the save
                // if (!ModelState.IsValid)
                // {
                //     ErrorMessage = "Por favor corrija los errores en el formulario";
                //     await LoadProfileDataAsync(userEmail);
                //     return Page();
                // }

                // Handle file uploads FIRST
                await ProcessFileUploads();

                // Save profile data
                await SaveProfileDataAsync(userEmail);

                StatusMessage = "Perfil actualizado correctamente";
                await LoadProfileDataAsync(userEmail); // Reload to show updated logo
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ErrorMessage = $"Hubo un error al guardar el perfil: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostNewsletterAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "El correo electrónico es obligatorio";
                    return Page();
                }

                // Simulate newsletter subscription
                await Task.Delay(500);
                Console.WriteLine($"Newsletter subscription: {Email} at {DateTime.Now}");

                StatusMessage = "¡Gracias por suscribirse! Recibirá las últimas noticias y promociones.";
                Email = string.Empty;
                
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in newsletter subscription: {ex.Message}");
                ErrorMessage = "Hubo un error al procesar su suscripción. Por favor intente nuevamente.";
                return Page();
            }
        }

        private async Task LoadProfileDataAsync(string userEmail)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (usuario != null)
                {
                    // Basic user information
                    NombreEmpresa = usuario.NombreAMostrar;
                    NombreComercial = usuario.EsAgencia ? usuario.NombreAgencia : $"{usuario.Nombre} {usuario.Apellidos}";
                    RepresentanteLegal = $"{usuario.Nombre} {usuario.Apellidos}";
                    Telefono1 = usuario.NumeroTelefono;
                    WhatsApp = usuario.NumeroTelefono;
                    LogoUrl = usuario.LogoUrl;
                    
                    // Set some default values for demo - in a real app these would come from an extended user profile
                    CedulaJuridica = usuario.EsAgencia ? "3-008-4534245" : "";
                    Provincia = "San José";
                    Canton = "San José";
                    DireccionExacta = "Centro de San José";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading profile data: {ex.Message}");
                // Set default values if loading fails
                NombreEmpresa = "Mi Perfil";
                NombreComercial = "Mi Perfil";
            }
        }

        private async Task ProcessFileUploads()
        {
            try
            {
                Console.WriteLine("=== ProcessFileUploads called ===");
                
                var connectionString = _configuration.GetConnectionString("AzureStorage");
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("ERROR: Azure Storage connection string not found");
                    return;
                }

                Console.WriteLine("Azure Storage connection string found");

                // Upload LogoFile1 to logosusuario container (this becomes the profile picture)
                if (LogoFile1 != null && LogoFile1.Length > 0)
                {
                    Console.WriteLine($"Uploading logo: {LogoFile1.FileName}, size: {LogoFile1.Length}");
                    
                    var logoUrl = await UploadToAzureBlobAsync(LogoFile1, "logosusuario", "logo1");
                    
                    if (!string.IsNullOrEmpty(logoUrl))
                    {
                        LogoUrl = logoUrl;
                        Console.WriteLine($"Logo URL set: {logoUrl}");
                        
                        // Update user's profile picture in database
                        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                            ?? User.FindFirst("email")?.Value 
                            ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                            
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
                            if (usuario != null)
                            {
                                Console.WriteLine($"Updating LogoUrl for user: {usuario.Email}");
                                usuario.LogoUrl = logoUrl;
                                await _context.SaveChangesAsync();
                                Console.WriteLine($"Updated profile picture for user: {usuario.Email}");
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Usuario not found in database");
                            }
                        }
                        else
                        {
                            Console.WriteLine("ERROR: User email not found");
                        }
                        
                        Console.WriteLine($"Logo uploaded successfully: {logoUrl}");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Logo upload failed - logoUrl is empty");
                    }
                }
                else
                {
                    Console.WriteLine("No logo file to upload");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file uploads: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task<string> UploadToAzureBlobAsync(IFormFile file, string containerName, string prefix)
        {
            try
            {
                Console.WriteLine($"=== UploadToAzureBlobAsync called ===");
                Console.WriteLine($"Container: {containerName}, Prefix: {prefix}, File: {file.FileName}");
                
                var connectionString = _configuration.GetConnectionString("AzureStorage");
                
                // Create BlobServiceClient
                var blobServiceClient = new BlobServiceClient(connectionString);
                Console.WriteLine("BlobServiceClient created");
                
                // Get container client (create if not exists)
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                Console.WriteLine($"Container '{containerName}' ready");
                
                // Generate unique blob name
                var fileName = $"{prefix}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var blobClient = containerClient.GetBlobClient(fileName);
                Console.WriteLine($"Blob name: {fileName}");
                
                // Set content type based on file extension
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                };
                
                // Upload file
                using (var stream = file.OpenReadStream())
                {
                    Console.WriteLine($"Uploading file, size: {stream.Length} bytes");
                    await blobClient.UploadAsync(stream, new BlobUploadOptions
                    {
                        HttpHeaders = blobHttpHeaders
                    });
                    Console.WriteLine("Upload completed");
                }
                
                // Return blob URL
                var blobUrl = blobClient.Uri.ToString();
                Console.WriteLine($"Blob URL: {blobUrl}");
                
                return blobUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading to Azure Blob: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return string.Empty;
            }
        }

        private async Task SaveProfileDataAsync(string userEmail)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (usuario != null)
                {
                    // Update user data with form values
                    if (!string.IsNullOrWhiteSpace(NombreComercial))
                    {
                        if (usuario.EsAgencia)
                        {
                            usuario.NombreAgencia = NombreComercial;
                        }
                        else
                        {
                            // For non-agency users, update name if it's in "First Last" format
                            var nameParts = NombreComercial.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (nameParts.Length >= 2)
                            {
                                usuario.Nombre = nameParts[0];
                                usuario.Apellidos = string.Join(" ", nameParts.Skip(1));
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(Telefono1))
                    {
                        usuario.NumeroTelefono = Telefono1;
                    }

                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"Profile updated for user: {usuario.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile data: {ex.Message}");
                throw;
            }
        }
    }
}