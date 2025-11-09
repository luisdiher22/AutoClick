using System.Security.Cryptography;
using System.Text;

namespace AutoClick.Helpers;

/// <summary>
/// Helper para hashear y verificar contraseñas
/// Usa el mismo algoritmo que AuthService (SHA256 + Salt)
/// </summary>
public static class PasswordHelper
{
    private const string SALT_KEY = "SALT_KEY_AUTOCLICK";

    /// <summary>
    /// Hashea una contraseña usando SHA256 y el salt del sistema
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + SALT_KEY));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verifica si una contraseña coincide con su hash
    /// </summary>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }
}
