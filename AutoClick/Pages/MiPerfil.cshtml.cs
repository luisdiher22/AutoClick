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

        // File upload properties
        [BindProperty]
        public IFormFile? LogoFile1 { get; set; }

        public string? LogoUrl { get; set; }
        public string? ImagenPerfilUrl { get; set; }
        public string? ImagenBannerUrl { get; set; }

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

                if (LogoFile1 != null)
                {
                }

                // Save profile data and handle file uploads in one transaction
                await SaveProfileAndFilesAsync(userEmail);

                StatusMessage = "Perfil actualizado correctamente";
                await LoadProfileDataAsync(userEmail); // Reload to show updated logo
                return Page();
            }
            catch (Exception ex)
            {
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

                StatusMessage = "¡Gracias por suscribirse! Recibirá las últimas noticias y promociones.";
                Email = string.Empty;
                
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Hubo un error al procesar su suscripción. Por favor intente nuevamente.";
                return Page();
            }
        }

        private async Task LoadProfileDataAsync(string userEmail)
        {
            try
            {
                
                // Force reload from database without tracking
                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == userEmail);
                    
                if (usuario != null)
                {
                    
                    // Basic user information
                    NombreEmpresa = usuario.NombreAMostrar;
                    NombreComercial = usuario.EsAgencia ? usuario.NombreAgencia : $"{usuario.Nombre} {usuario.Apellidos}";
                    RepresentanteLegal = $"{usuario.Nombre} {usuario.Apellidos}";
                    Telefono1 = usuario.NumeroTelefono;
                    WhatsApp = usuario.NumeroTelefono;
                    LogoUrl = usuario.LogoUrl;
                    ImagenPerfilUrl = usuario.ImagenPerfilUrl;
                    ImagenBannerUrl = usuario.ImagenBannerUrl;
                    
                    // Load actual values from database
                    CedulaJuridica = usuario.CedulaJuridica;
                    Provincia = usuario.Provincia;
                    Canton = usuario.Canton;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                // Set default values if loading fails
                NombreEmpresa = "Mi Perfil";
                NombreComercial = "Mi Perfil";
            }
        }

        private async Task SaveProfileAndFilesAsync(string userEmail)
        {
            try
            {
                
                // Get user from database
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (usuario == null)
                {
                    return;
                }

                // 1. Upload logo to Azure if provided
                if (LogoFile1 != null && LogoFile1.Length > 0)
                {
                    
                    var connectionString = _configuration.GetConnectionString("AzureStorage");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        var logoUrl = await UploadToAzureBlobAsync(LogoFile1, "logosusuario", "logo1");
                        
                        if (!string.IsNullOrEmpty(logoUrl))
                        {
                            usuario.LogoUrl = logoUrl;
                            LogoUrl = logoUrl;
                        }
                    }
                }

                // 2. Update other profile fields
                if (!string.IsNullOrWhiteSpace(NombreComercial))
                {
                    if (usuario.EsAgencia)
                    {
                        usuario.NombreAgencia = NombreComercial;
                    }
                    else
                    {
                        var nameParts = NombreComercial.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (nameParts.Length >= 2)
                        {
                            usuario.Nombre = nameParts[0];
                            usuario.Apellidos = string.Join(" ", nameParts.Skip(1));
                        }
                    }
                }

                // Update agency-specific fields
                usuario.CedulaJuridica = CedulaJuridica;
                usuario.Provincia = Provincia;
                usuario.Canton = Canton;
                
                if (!string.IsNullOrWhiteSpace(Telefono1))
                {
                    usuario.NumeroTelefono = Telefono1;
                }

                // 3. Mark entity as modified to ensure EF tracks the changes
                _context.Entry(usuario).State = EntityState.Modified;
                
                // 4. Save all changes in one transaction
                var changesSaved = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task ProcessFileUploads()
        {
            try
            {
                
                var connectionString = _configuration.GetConnectionString("AzureStorage");
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    return;
                }

                // Upload LogoFile1 to logosusuario container (this becomes the profile picture)
                if (LogoFile1 != null && LogoFile1.Length > 0)
                {
                    
                    var logoUrl = await UploadToAzureBlobAsync(LogoFile1, "logosusuario", "logo1");
                    
                    if (!string.IsNullOrEmpty(logoUrl))
                    {
                        LogoUrl = logoUrl;
                        
                        // Update user's profile picture in database
                        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                            ?? User.FindFirst("email")?.Value 
                            ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                            
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
                            if (usuario != null)
                            {
                                usuario.LogoUrl = logoUrl;
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                        }
                        
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string> UploadToAzureBlobAsync(IFormFile file, string containerName, string prefix)
        {
            try
            {
                
                var connectionString = _configuration.GetConnectionString("AzureStorage");
                
                // Create BlobServiceClient
                var blobServiceClient = new BlobServiceClient(connectionString);
                
                // Get container client (create if not exists)
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                
                // Generate unique blob name
                var fileName = $"{prefix}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var blobClient = containerClient.GetBlobClient(fileName);
                
                // Set content type based on file extension
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                };
                
                // Upload file
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobUploadOptions
                    {
                        HttpHeaders = blobHttpHeaders
                    });
                }
                
                // Return blob URL
                var blobUrl = blobClient.Uri.ToString();
                
                return blobUrl;
            }
            catch (Exception ex)
            {
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
                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}