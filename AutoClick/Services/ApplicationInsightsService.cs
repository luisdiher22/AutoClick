using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Azure.Identity;
using Azure;

namespace AutoClick.Services
{
    /// <summary>
    /// Servicio para obtener métricas de Application Insights usando Azure Monitor Query API
    /// </summary>
    public class ApplicationInsightsService : IApplicationInsightsService
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<ApplicationInsightsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly LogsQueryClient? _logsQueryClient;
        private readonly string? _workspaceId;

        public ApplicationInsightsService(
            TelemetryClient telemetryClient,
            ILogger<ApplicationInsightsService> logger,
            IConfiguration configuration)
        {
            _telemetryClient = telemetryClient;
            _logger = logger;
            _configuration = configuration;

            // Obtener Workspace ID de configuración
            _workspaceId = _configuration["ApplicationInsights:WorkspaceId"];

            // Inicializar LogsQueryClient solo si hay WorkspaceId configurado
            if (!string.IsNullOrEmpty(_workspaceId))
            {
                try
                {
                    // Usar DefaultAzureCredential para autenticación
                    // Esto funciona con Managed Identity en Azure o credenciales locales en desarrollo
                    _logsQueryClient = new LogsQueryClient(new DefaultAzureCredential());
                    _logger.LogInformation("LogsQueryClient inicializado correctamente con Workspace ID: {WorkspaceId}", _workspaceId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al inicializar LogsQueryClient. Las consultas de métricas no estarán disponibles.");
                    _logsQueryClient = null;
                }
            }
            else
            {
                _logger.LogWarning("ApplicationInsights:WorkspaceId no configurado. Configure este valor en appsettings.json para habilitar consultas de métricas.");
            }
        }

        /// <summary>
        /// Obtiene el total de visitas (page views) en un período de tiempo usando KQL
        /// </summary>
        public async Task<int> GetPageViewsAsync(TimeSpan timeSpan)
        {
            // Validar que el cliente esté disponible
            if (_logsQueryClient == null || string.IsNullOrEmpty(_workspaceId))
            {
                _logger.LogWarning("LogsQueryClient no está disponible. Retornando 0.");
                return 0;
            }

            try
            {
                // Construir la consulta KQL
                // AppPageViews es la tabla de Application Insights para page views
                var query = @"
                    AppPageViews
                    | where TimeGenerated > ago({0}d)
                    | summarize TotalViews = sum(ItemCount)
                ";

                // Convertir TimeSpan a días
                int days = (int)Math.Ceiling(timeSpan.TotalDays);
                query = string.Format(query, days);

                _logger.LogDebug("Ejecutando consulta KQL: {Query}", query);

                // Ejecutar la consulta
                Response<LogsQueryResult> response = await _logsQueryClient.QueryWorkspaceAsync(
                    _workspaceId,
                    query,
                    new QueryTimeRange(timeSpan));

                // Verificar si hay resultados
                if (response.Value.Status == LogsQueryResultStatus.Success)
                {
                    var table = response.Value.Table;
                    
                    if (table.Rows.Count > 0)
                    {
                        // Obtener el valor de TotalViews (primera columna, primera fila)
                        var totalViews = table.Rows[0][0];
                        
                        // Convertir a int (puede ser long o int dependiendo del resultado)
                        if (totalViews != null)
                        {
                            int result = Convert.ToInt32(totalViews);
                            _logger.LogInformation("Total de visitas obtenidas: {TotalViews} (últimos {Days} días)", result, days);
                            return result;
                        }
                    }
                    
                    _logger.LogInformation("No se encontraron datos de visitas para el período especificado");
                    return 0;
                }
                else
                {
                    _logger.LogError("Error en consulta KQL. Status: {Status}", response.Value.Status);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pageviews de Application Insights");
                return 0;
            }
        }

        /// <summary>
        /// Obtiene las visitas del mes actual (últimos 30 días)
        /// </summary>
        public async Task<int> GetMonthlyPageViewsAsync()
        {
            return await GetPageViewsAsync(TimeSpan.FromDays(30));
        }

        /// <summary>
        /// Obtiene las visitas de la semana actual (últimos 7 días)
        /// </summary>
        public async Task<int> GetWeeklyPageViewsAsync()
        {
            return await GetPageViewsAsync(TimeSpan.FromDays(7));
        }

        /// <summary>
        /// Obtiene las visitas del día actual (últimas 24 horas)
        /// </summary>
        public async Task<int> GetDailyPageViewsAsync()
        {
            return await GetPageViewsAsync(TimeSpan.FromDays(1));
        }

        /// <summary>
        /// Trackea un evento personalizado
        /// </summary>
        public void TrackEvent(string eventName, Dictionary<string, string>? properties = null)
        {
            _telemetryClient.TrackEvent(eventName, properties);
        }

        /// <summary>
        /// Trackea una página vista
        /// </summary>
        public void TrackPageView(string pageName)
        {
            _telemetryClient.TrackPageView(pageName);
        }
    }
}
