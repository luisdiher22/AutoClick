using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new AuthResult 
                { 
                    Success = false, 
                    Message = "Email y contraseña son requeridos.",
                    Errors = new List<string> { "Campos obligatorios faltantes" }
                };
            }

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLower());
            
            if (user == null)
            {
                return new AuthResult 
                { 
                    Success = false, 
                    Message = $"Usuario no encontrado con email: {email}",
                    Errors = new List<string> { "Usuario no encontrado" }
                };
            }
            
            var inputHash = HashPassword(password);
            var passwordMatches = VerifyPassword(password, user.Contrasena);
            
            if (!passwordMatches)
            {
                return new AuthResult 
                { 
                    Success = false, 
                    Message = $"Contraseña incorrecta. Hash generado: {inputHash.Substring(0, 10)}... vs Almacenado: {user.Contrasena.Substring(0, 10)}...",
                    Errors = new List<string> { "Contraseña incorrecta" }
                };
            }

            // Crear claims para el usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.NombreCompleto),
                new Claim("DisplayName", user.NombreAMostrar)
            };

            if (user.EsAgencia)
            {
                claims.Add(new Claim("IsAgency", "true"));
                claims.Add(new Claim("AgencyName", user.NombreAgencia!));
            }

            if (user.EsAdministrador)
            {
                claims.Add(new Claim("IsAdmin", "true"));
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return new AuthResult 
            { 
                Success = true, 
                Message = "Login exitoso",
                User = user
            };
        }
        catch (Exception ex)
        {
            return new AuthResult 
            { 
                Success = false, 
                Message = "Error interno del servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<AuthResult> RegisterAsync(Usuario usuario, string password)
    {
        try
        {
            // Validaciones
            var errors = new List<string>();

            if (string.IsNullOrEmpty(usuario.Email))
                errors.Add("El email es requerido");
            else if (await EmailExistsAsync(usuario.Email))
                errors.Add("Este email ya está registrado");

            if (string.IsNullOrEmpty(password) || password.Length < 6)
                errors.Add("La contraseña debe tener al menos 6 caracteres");

            if (string.IsNullOrEmpty(usuario.Nombre))
                errors.Add("El nombre es requerido");

            if (string.IsNullOrEmpty(usuario.Apellidos))
                errors.Add("Los apellidos son requeridos");

            if (string.IsNullOrEmpty(usuario.NumeroTelefono))
                errors.Add("El número de teléfono es requerido");

            if (errors.Any())
            {
                return new AuthResult 
                { 
                    Success = false, 
                    Message = "Datos de registro inválidos",
                    Errors = errors
                };
            }

            // Hashear la contraseña
            usuario.Email = usuario.Email.ToLower();
            usuario.Contrasena = HashPassword(password);

            // Guardar usuario
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new AuthResult 
            { 
                Success = true, 
                Message = "Usuario registrado exitosamente",
                User = usuario
            };
        }
        catch (Exception ex)
        {
            return new AuthResult 
            { 
                Success = false, 
                Message = "Error al registrar usuario",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task LogoutAsync()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<bool> VerifyPasswordAsync(string email, string password)
    {
        var user = await _context.Usuarios.FindAsync(email.ToLower());
        return user != null && VerifyPassword(password, user.Contrasena);
    }

    public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _context.Usuarios.FindAsync(email.ToLower());
            if (user == null || !VerifyPassword(currentPassword, user.Contrasena))
            {
                return false;
            }

            user.Contrasena = HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Usuario?> GetUserByEmailAsync(string email)
    {
        return await _context.Usuarios.FindAsync(email.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email.ToLower());
    }

    // Métodos privados para hashing de contraseñas
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "SALT_KEY_AUTOCLICK"));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }
}