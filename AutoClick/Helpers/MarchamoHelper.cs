using System;

namespace AutoClick.Helpers
{
    /// <summary>
    /// Helper para calcular estimaciones de marchamo y costos de traspaso en Costa Rica
    /// ADVERTENCIA: Estos son cálculos aproximados. Los valores oficiales deben consultarse con el INS y abogados autorizados.
    /// </summary>
    public static class MarchamoHelper
    {
        /// <summary>
        /// Calcula el marchamo total estimado (impuesto + seguro + timbre)
        /// </summary>
        /// <param name="valorFiscal">Valor fiscal del vehículo en colones</param>
        /// <param name="anio">Año del vehículo</param>
        /// <returns>Monto total estimado del marchamo en colones</returns>
        public static decimal CalcularMarchamoTotal(decimal valorFiscal, int anio)
        {
            if (valorFiscal <= 0) return 0;

            // 1. Impuesto de circulación (aproximadamente 2.5% del valor fiscal)
            decimal impuestoCirculacion = valorFiscal * 0.025m;

            // 2. Seguro Obligatorio (INS) - varía según edad del vehículo
            decimal seguroObligatorio = CalcularSeguroObligatorio(valorFiscal, anio);

            // 3. Timbre de Circulación (monto fijo aproximado)
            decimal timbreCirculacion = 1050m; // Monto aproximado

            // Total
            decimal total = impuestoCirculacion + seguroObligatorio + timbreCirculacion;

            return Math.Round(total, 2);
        }

        /// <summary>
        /// Calcula solo el impuesto de circulación
        /// </summary>
        public static decimal CalcularImpuestoCirculacion(decimal valorFiscal)
        {
            if (valorFiscal <= 0) return 0;
            return Math.Round(valorFiscal * 0.025m, 2);
        }

        /// <summary>
        /// Calcula el seguro obligatorio según el valor fiscal y año del vehículo
        /// El seguro es progresivo según el valor del vehículo
        /// </summary>
        private static decimal CalcularSeguroObligatorio(decimal valorFiscal, int anio)
        {
            if (valorFiscal <= 0) return 0;

            // El seguro es aproximadamente 1.5% - 2% del valor fiscal
            // Para vehículos nuevos es mayor, para vehículos viejos es menor
            int antiguedad = DateTime.Now.Year - anio;

            decimal porcentajeSeguro;
            if (antiguedad <= 3)
            {
                // Vehículos nuevos: seguro más alto
                porcentajeSeguro = 0.020m; // 2%
            }
            else if (antiguedad <= 7)
            {
                // Vehículos semi-nuevos
                porcentajeSeguro = 0.018m; // 1.8%
            }
            else if (antiguedad <= 12)
            {
                // Vehículos de mediana edad
                porcentajeSeguro = 0.015m; // 1.5%
            }
            else
            {
                // Vehículos viejos
                porcentajeSeguro = 0.012m; // 1.2%
            }

            decimal seguro = valorFiscal * porcentajeSeguro;

            // Seguro mínimo aproximado
            if (seguro < 15000m)
                seguro = 15000m;

            // Seguro máximo aproximado
            if (seguro > 500000m)
                seguro = 500000m;

            return Math.Round(seguro, 2);
        }

        /// <summary>
        /// Calcula el desglose completo del marchamo
        /// </summary>
        public static MarchamoDesglose CalcularMarchamoDesglose(decimal valorFiscal, int anio)
        {
            if (valorFiscal <= 0)
            {
                return new MarchamoDesglose
                {
                    ImpuestoCirculacion = 0,
                    SeguroObligatorio = 0,
                    TimbreCirculacion = 0,
                    Total = 0
                };
            }

            var desglose = new MarchamoDesglose
            {
                ImpuestoCirculacion = CalcularImpuestoCirculacion(valorFiscal),
                SeguroObligatorio = CalcularSeguroObligatorio(valorFiscal, anio),
                TimbreCirculacion = 1050m
            };

            desglose.Total = desglose.ImpuestoCirculacion + desglose.SeguroObligatorio + desglose.TimbreCirculacion;

            return desglose;
        }

        /// <summary>
        /// Calcula los costos totales de traspaso
        /// </summary>
        /// <param name="valorVenta">Precio de venta del vehículo en colones</param>
        /// <param name="incluyeAbogado">Si incluye honorarios de abogado</param>
        /// <returns>Monto total estimado de costos de traspaso</returns>
        public static decimal CalcularCostosTraspaso(decimal valorVenta, bool incluyeAbogado = true)
        {
            if (valorVenta <= 0) return 0;

            // 1. Impuesto de traspaso (2.5% del valor de venta)
            decimal impuestoTraspaso = valorVenta * 0.025m;

            // 2. Timbres y gastos registrales (aproximado)
            decimal timbres = 8000m;

            // 3. Honorarios de abogado (aproximado 1% del valor, mínimo 50,000)
            decimal honorariosAbogado = 0m;
            if (incluyeAbogado)
            {
                honorariosAbogado = Math.Max(50000m, valorVenta * 0.01m);
                // Máximo razonable de 250,000 colones
                if (honorariosAbogado > 250000m)
                    honorariosAbogado = 250000m;
            }

            // Total
            decimal total = impuestoTraspaso + timbres + honorariosAbogado;

            return Math.Round(total, 2);
        }

        /// <summary>
        /// Calcula el desglose completo de costos de traspaso
        /// </summary>
        public static TraspasoCostos CalcularTraspasoDesglose(decimal valorVenta)
        {
            if (valorVenta <= 0)
            {
                return new TraspasoCostos
                {
                    ImpuestoTraspaso = 0,
                    Timbres = 0,
                    HonorariosAbogado = 0,
                    Total = 0
                };
            }

            var costos = new TraspasoCostos
            {
                ImpuestoTraspaso = Math.Round(valorVenta * 0.025m, 2),
                Timbres = 8000m,
                HonorariosAbogado = Math.Round(Math.Max(50000m, Math.Min(250000m, valorVenta * 0.01m)), 2)
            };

            costos.Total = costos.ImpuestoTraspaso + costos.Timbres + costos.HonorariosAbogado;

            return costos;
        }

        /// <summary>
        /// Formatea un monto en colones con símbolo
        /// </summary>
        public static string FormatearColones(decimal monto)
        {
            return $"₡{monto:N0}";
        }
    }

    /// <summary>
    /// Clase para el desglose del marchamo
    /// </summary>
    public class MarchamoDesglose
    {
        public decimal ImpuestoCirculacion { get; set; }
        public decimal SeguroObligatorio { get; set; }
        public decimal TimbreCirculacion { get; set; }
        public decimal Total { get; set; }

        public string ImpuestoCirculacionFormateado => MarchamoHelper.FormatearColones(ImpuestoCirculacion);
        public string SeguroObligatorioFormateado => MarchamoHelper.FormatearColones(SeguroObligatorio);
        public string TimbreCirculacionFormateado => MarchamoHelper.FormatearColones(TimbreCirculacion);
        public string TotalFormateado => MarchamoHelper.FormatearColones(Total);
    }

    /// <summary>
    /// Clase para los costos de traspaso
    /// </summary>
    public class TraspasoCostos
    {
        public decimal ImpuestoTraspaso { get; set; }
        public decimal Timbres { get; set; }
        public decimal HonorariosAbogado { get; set; }
        public decimal Total { get; set; }

        public string ImpuestoTraspasoFormateado => MarchamoHelper.FormatearColones(ImpuestoTraspaso);
        public string TimbresFormateado => MarchamoHelper.FormatearColones(Timbres);
        public string HonorariosAbogadoFormateado => MarchamoHelper.FormatearColones(HonorariosAbogado);
        public string TotalFormateado => MarchamoHelper.FormatearColones(Total);
    }
}
