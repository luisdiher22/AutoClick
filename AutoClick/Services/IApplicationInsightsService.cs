namespace AutoClick.Services
{
    /// <summary>
    /// Interfaz para el servicio de Application Insights
    /// </summary>
    public interface IApplicationInsightsService
    {
        /// <summary>
        /// Obtiene el total de visitas en un período de tiempo
        /// </summary>
        Task<int> GetPageViewsAsync(TimeSpan timeSpan);
        
        /// <summary>
        /// Obtiene las visitas del mes actual
        /// </summary>
        Task<int> GetMonthlyPageViewsAsync();
        
        /// <summary>
        /// Obtiene las visitas de la semana actual
        /// </summary>
        Task<int> GetWeeklyPageViewsAsync();
        
        /// <summary>
        /// Obtiene las visitas del día actual
        /// </summary>
        Task<int> GetDailyPageViewsAsync();
    }
}
