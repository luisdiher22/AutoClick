using AutoClick.Models;

namespace AutoClick.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(Usuario usuario, string password);
    Task LogoutAsync();
    Task<bool> VerifyPasswordAsync(string email, string password);
    Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
    Task<Usuario?> GetUserByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Usuario? User { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}