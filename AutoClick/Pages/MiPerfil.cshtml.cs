using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages
{
    [Authorize]
    public class MiPerfilModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MiPerfilModel(ApplicationDbContext context)
        {
            _context = context;
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
        [Required(ErrorMessage = "El distrito es obligatorio")]
        [Display(Name = "Distrito")]
        public string? Distrito { get; set; }

        [BindProperty]
        [Display(Name = "Dirección exacta")]
        public string? DireccionExacta { get; set; }

        // File upload properties
        [BindProperty]
        public IFormFile? LogoFile1 { get; set; }

        [BindProperty]
        public IFormFile? LogoFile2 { get; set; }

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

                if (!ModelState.IsValid)
                {
                    ErrorMessage = "Por favor corrija los errores en el formulario";
                    await LoadProfileDataAsync(userEmail);
                    return Page();
                }

                // Handle file uploads
                await ProcessFileUploads();

                // Save profile data
                await SaveProfileDataAsync(userEmail);

                StatusMessage = "Perfil actualizado correctamente";
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile: {ex.Message}");
                ErrorMessage = "Hubo un error al guardar el perfil. Por favor intente nuevamente.";
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
                    
                    // Set some default values for demo - in a real app these would come from an extended user profile
                    CedulaJuridica = usuario.EsAgencia ? "3-008-4534245" : "";
                    Provincia = "San José";
                    Canton = "San José";
                    Distrito = "Carmen";
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
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logos");
            
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            if (LogoFile1 != null && LogoFile1.Length > 0)
            {
                await SaveUploadedFile(LogoFile1, uploadsPath, "logo1");
            }

            if (LogoFile2 != null && LogoFile2.Length > 0)
            {
                await SaveUploadedFile(LogoFile2, uploadsPath, "logo2");
            }
        }

        private async Task SaveUploadedFile(IFormFile file, string uploadsPath, string prefix)
        {
            var fileName = $"{prefix}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update LogoUrl property
            if (prefix == "logo1")
            {
                LogoUrl = $"/uploads/logos/{fileName}";
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