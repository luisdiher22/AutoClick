using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace AutoClick.Pages;

public class HashCalculatorModel : PageModel
{
    public string Result { get; set; } = "";

    public void OnGet()
    {
        Result = "Click el botón para calcular hash";
    }

    public IActionResult OnPost()
    {
        try
        {
            string password = "prueba123";
            string salt = "SALT_KEY_AUTOCLICK";
            
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            var hash = Convert.ToBase64String(hashedBytes);
            
            Result = $"Contraseña: {password}\n";
            Result += $"Salt: {salt}\n";
            Result += $"Hash: {hash}\n\n";
            Result += $"SQL INSERT:\n";
            Result += $"INSERT INTO Usuarios (Email, Nombre, Apellidos, NumeroTelefono, Contrasena) VALUES ('admin@gmail.com', 'Admin', 'User', '12345678', '{hash}');";
        }
        catch (Exception ex)
        {
            Result = $"Error: {ex.Message}";
        }
        
        return Page();
    }
}