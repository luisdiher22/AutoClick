using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Services;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class QuickLoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public QuickLoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "admin@gmail.com";

        [BindProperty]
        [Required]
        public string Password { get; set; } = "prueba123";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _authService.LoginAsync(Email, Password);
                
                if (result.Success)
                {
                    ViewData["Message"] = "Login successful! Redirecting...";
                    return RedirectToPage("/Index");
                }
                else
                {
                    ViewData["Message"] = $"Login failed: {result.Message}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ViewData["Message"] = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}