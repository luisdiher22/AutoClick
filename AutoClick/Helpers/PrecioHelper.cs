using AutoClick.Models;
using AutoClick.Services;

namespace AutoClick.Helpers
{
    /// <summary>
    /// Helper estático para formatear precios con conversión automática de divisas
    /// </summary>
    public static class PrecioHelper
    {
        private static ITasaCambioService? _tasaCambioService;
        private static decimal _tasaCacheada = 510m; // Tasa por defecto
        
        /// <summary>
        /// Inicializa el helper con el servicio de tasa de cambio
        /// Debe ser llamado al inicio de la aplicación
        /// </summary>
        public static void Initialize(ITasaCambioService tasaCambioService)
        {
            _tasaCambioService = tasaCambioService;
        }
        
        /// <summary>
        /// Actualiza la tasa cacheada (llamado periódicamente desde el servicio)
        /// </summary>
        public static void ActualizarTasaCacheada(decimal nuevaTasa)
        {
            if (nuevaTasa > 0)
            {
                _tasaCacheada = nuevaTasa;
            }
        }
        
        /// <summary>
        /// Formatea un precio siempre en CRC, convirtiendo desde USD si es necesario
        /// </summary>
        public static string FormatearPrecioEnCRC(decimal precio, string divisa)
        {
            decimal precioEnCRC = divisa?.ToUpper() == "USD" 
                ? precio * _tasaCacheada
                : precio;
            
            return $"₡{precioEnCRC:N0}";
        }
        
        /// <summary>
        /// Formatea un precio mostrando la divisa original (USD o CRC)
        /// </summary>
        public static string FormatearPrecioConDivisa(decimal precio, string divisa)
        {
            return divisa?.ToUpper() == "USD" 
                ? $"${precio:N0}" 
                : $"₡{precio:N0}";
        }
        
        /// <summary>
        /// Obtiene el precio convertido a CRC (sin formato)
        /// </summary>
        public static decimal ConvertirACRC(decimal precio, string divisa)
        {
            return divisa?.ToUpper() == "USD" 
                ? precio * _tasaCacheada
                : precio;
        }
    }
}
