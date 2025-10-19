namespace AutoClick.Models
{
    /// <summary>
    /// Tipos de clasificación de oferta basados en la comparación con el valor de mercado
    /// </summary>
    public enum TipoOferta
    {
        ExcelenteOferta,  // Morado - Precio significativamente por debajo del promedio
        BuenaOferta,      // Verde - Precio cerca del promedio o ligeramente por debajo
        OfertaJusta,      // Amarillo - Precio ligeramente por arriba del promedio
        MalaOferta        // Rojo - Precio significativamente por arriba del promedio
    }

    /// <summary>
    /// Modelo que contiene el análisis completo del valor de mercado de un vehículo
    /// </summary>
    public class ValorMercadoAnalisis
    {
        /// <summary>
        /// Indica si hay datos suficientes para realizar el análisis
        /// </summary>
        public bool TieneDatos { get; set; }

        /// <summary>
        /// Mensaje cuando no hay datos disponibles
        /// </summary>
        public string? MensajeSinDatos { get; set; }

        /// <summary>
        /// Precio promedio del mercado para este modelo y año (en CRC)
        /// </summary>
        public decimal PromedioPrecio { get; set; }

        /// <summary>
        /// Precio mínimo encontrado en el mercado (en CRC)
        /// </summary>
        public decimal PrecioMinimo { get; set; }

        /// <summary>
        /// Precio máximo encontrado en el mercado (en CRC)
        /// </summary>
        public decimal PrecioMaximo { get; set; }

        /// <summary>
        /// Cantidad de registros utilizados para calcular el análisis
        /// </summary>
        public int CantidadRegistros { get; set; }

        /// <summary>
        /// Precio del vehículo actual (en CRC)
        /// </summary>
        public decimal PrecioActual { get; set; }

        /// <summary>
        /// Diferencia entre el precio actual y el promedio del mercado (en CRC)
        /// </summary>
        public decimal DiferenciaConPromedio { get; set; }

        /// <summary>
        /// Porcentaje de diferencia con respecto al promedio (valores negativos = por debajo del promedio)
        /// </summary>
        public decimal PorcentajeDiferencia { get; set; }

        /// <summary>
        /// Clasificación de la oferta
        /// </summary>
        public TipoOferta TipoOferta { get; set; }

        /// <summary>
        /// Descripción textual de la clasificación
        /// </summary>
        public string DescripcionOferta { get; set; } = string.Empty;

        /// <summary>
        /// Color asociado a la clasificación (para el UI)
        /// </summary>
        public string ColorOferta { get; set; } = string.Empty;

        /// <summary>
        /// Posición porcentual del precio actual en el rango min-max (0-100)
        /// </summary>
        public decimal PosicionEnRango { get; set; }
    }
}
