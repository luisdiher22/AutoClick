using AutoClick.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicidadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PublicidadController> _logger;

        public PublicidadController(ApplicationDbContext context, ILogger<PublicidadController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("registrar-clic/{id}")]
        public async Task<IActionResult> RegistrarClic(int id)
        {
            try
            {
                var anuncio = await _context.AnunciosPublicidad.FindAsync(id);
                if (anuncio == null)
                {
                    return NotFound();
                }

                anuncio.NumeroClics++;
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al registrar clic para anuncio {id}");
                return StatusCode(500, new { success = false, message = "Error al registrar el clic" });
            }
        }
    }
}
