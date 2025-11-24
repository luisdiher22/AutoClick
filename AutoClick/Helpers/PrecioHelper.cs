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
        /// Obtiene la tasa de cambio actual USD a CRC
        /// </summary>
        public static decimal TasaCambio => _tasaCacheada;
        
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
        
        /// <summary>
        /// Calcula la cuota mensual estimada de un préstamo
        /// Prima fija: 20% del valor
        /// Plazo: 84 meses
        /// Tasa de interés: 8% anual
        /// </summary>
        public static decimal CalcularCuotaMensual(decimal precio, string divisa)
        {
            // Convertir precio a CRC si está en USD
            decimal precioEnCRC = ConvertirACRC(precio, divisa);
            
            // Prima del 20%
            decimal prima = precioEnCRC * 0.20m;
            decimal montoFinanciado = precioEnCRC - prima;
            
            // Tasa de interés mensual (8% anual / 12 meses)
            decimal tasaMensual = 0.08m / 12m;
            
            // Plazo en meses
            int plazoMeses = 84;
            
            // Fórmula de cuota: P * [r(1+r)^n] / [(1+r)^n - 1]
            // Donde: P = monto financiado, r = tasa mensual, n = plazo en meses
            decimal potencia = (decimal)Math.Pow((double)(1 + tasaMensual), plazoMeses);
            decimal cuota = montoFinanciado * (tasaMensual * potencia) / (potencia - 1);
            
            return cuota;
        }
        
        /// <summary>
        /// Formatea la cuota mensual estimada
        /// </summary>
        public static string FormatearCuotaMensual(decimal precio, string divisa)
        {
            decimal cuota = CalcularCuotaMensual(precio, divisa);
            return $"₡{cuota:N0}/mes";
        }
    }
}
