using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using System.Security.Claims;

namespace AutoClick.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoritosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Favoritos/Toggle
    [HttpPost("Toggle")]
    public async Task<IActionResult> ToggleFavorito([FromBody] ToggleFavoritoRequest request)
    {
        try
        {
            // Obtener el email del usuario autenticado
            var emailUsuario = User.Identity?.IsAuthenticated == true
                ? User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value
                : request.EmailUsuario;

            if (string.IsNullOrEmpty(emailUsuario))
            {
                return BadRequest(new { success = false, message = "Usuario no autenticado" });
            }

            // Verificar si ya existe el favorito
            var favoritoExistente = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.EmailUsuario == emailUsuario && f.AutoId == request.AutoId);

            if (favoritoExistente != null)
            {
                // Si existe, lo eliminamos
                _context.Favoritos.Remove(favoritoExistente);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, isFavorite = false, message = "Favorito eliminado" });
            }
            else
            {
                // Si no existe, lo creamos
                var nuevoFavorito = new Favorito
                {
                    EmailUsuario = emailUsuario,
                    AutoId = request.AutoId,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Favoritos.Add(nuevoFavorito);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, isFavorite = true, message = "Favorito agregado" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // GET: api/Favoritos/Check/{autoId}
    [HttpGet("Check/{autoId}")]
    public async Task<IActionResult> CheckFavorito(int autoId, [FromQuery] string? emailUsuario = null)
    {
        try
        {
            // Obtener el email del usuario autenticado o del query string
            var email = User.Identity?.IsAuthenticated == true
                ? User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value
                : emailUsuario;

            if (string.IsNullOrEmpty(email))
            {
                return Ok(new { isFavorite = false });
            }

            var isFavorite = await _context.Favoritos
                .AnyAsync(f => f.EmailUsuario == email && f.AutoId == autoId);

            return Ok(new { isFavorite });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // GET: api/Favoritos/GetUserFavorites
    [HttpGet("GetUserFavorites")]
    public async Task<IActionResult> GetUserFavorites([FromQuery] string? emailUsuario = null)
    {
        try
        {
            // Obtener el email del usuario autenticado o del query string
            var email = User.Identity?.IsAuthenticated == true
                ? User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value
                : emailUsuario;

            if (string.IsNullOrEmpty(email))
            {
                return Ok(new { favoritos = new List<int>() });
            }

            var favoritosIds = await _context.Favoritos
                .Where(f => f.EmailUsuario == email)
                .Select(f => f.AutoId)
                .ToListAsync();

            return Ok(new { favoritos = favoritosIds });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // GET: api/Favoritos/GetFavoriteAutos
    [HttpGet("GetFavoriteAutos")]
    public async Task<IActionResult> GetFavoriteAutos([FromQuery] string? emailUsuario = null)
    {
        try
        {
            // Obtener el email del usuario autenticado o del query string
            var email = User.Identity?.IsAuthenticated == true
                ? User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value
                : emailUsuario;

            if (string.IsNullOrEmpty(email))
            {
                return Ok(new { autos = new List<Auto>() });
            }

            var autosGuardados = await _context.Favoritos
                .Where(f => f.EmailUsuario == email)
                .Include(f => f.Auto)
                .Where(f => f.Auto != null && f.Auto.Activo && f.Auto.PlanVisibilidad > 0) // Excluir pendientes
                .Select(f => f.Auto)
                .ToListAsync();

            return Ok(new { autos = autosGuardados });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }
}

// Modelo para la request
public class ToggleFavoritoRequest
{
    public int AutoId { get; set; }
    public string? EmailUsuario { get; set; }
}
