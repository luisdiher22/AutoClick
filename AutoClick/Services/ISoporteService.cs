using AutoClick.Models;

namespace AutoClick.Services;

public interface ISoporteService
{
    // Métodos para Reclamos
    Task<List<Reclamo>> GetReclamosAsync();
    Task<List<Reclamo>> GetReclamosPorEstadoAsync(EstadoReclamo estado);
    Task<List<Reclamo>> GetReclamosPorFiltroAsync(string? tipoProblema = null, EstadoReclamo? estado = null, string? prioridad = null);
    Task<(List<Reclamo> items, int totalCount)> GetReclamosPaginadosAsync(int pagina, int tamañoPagina, string? tipoProblema = null, EstadoReclamo? estado = null, string? prioridad = null);
    Task<Reclamo?> GetReclamoByIdAsync(int id);
    Task<int> CrearReclamoAsync(Reclamo reclamo);
    Task<bool> ActualizarReclamoAsync(Reclamo reclamo);
    Task<bool> ResponderReclamoAsync(int reclamoId, string respuesta, string emailAdmin);
    Task<bool> CambiarEstadoReclamoAsync(int reclamoId, EstadoReclamo nuevoEstado);
    Task<bool> CambiarPrioridadReclamoAsync(int reclamoId, string nuevaPrioridad);
    
    // Métodos para Mensajes
    Task<List<Mensaje>> GetMensajesAsync();
    Task<List<Mensaje>> GetMensajesPorEstadoAsync(EstadoMensaje estado);
    Task<List<Mensaje>> GetMensajesPorFiltroAsync(string? tipoConsulta = null, EstadoMensaje? estado = null, string? prioridad = null);
    Task<(List<Mensaje> items, int totalCount)> GetMensajesPaginadosAsync(int pagina, int tamañoPagina, string? tipoConsulta = null, EstadoMensaje? estado = null, string? prioridad = null);
    Task<Mensaje?> GetMensajeByIdAsync(int id);
    Task<int> CrearMensajeAsync(Mensaje mensaje);
    Task<bool> ActualizarMensajeAsync(Mensaje mensaje);
    Task<bool> ResponderMensajeAsync(int mensajeId, string respuesta, string emailAdmin);
    Task<bool> CambiarEstadoMensajeAsync(int mensajeId, EstadoMensaje nuevoEstado);
    Task<bool> CambiarPrioridadMensajeAsync(int mensajeId, string nuevaPrioridad);
    
    // Métodos para estadísticas del dashboard
    Task<int> GetReclamosPendientesCountAsync();
    Task<int> GetMensajesNoLeidosCountAsync();
    Task<Dictionary<string, int>> GetEstadisticasReclamosAsync();
    Task<Dictionary<string, int>> GetEstadisticasMensajesAsync();
    
    // Métodos para obtener opciones de formularios
    Task<List<string>> GetTiposProblemaAsync();
    Task<List<string>> GetTiposConsultaAsync();
    List<string> GetPrioridades();
}