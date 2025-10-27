using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class VentasExternas
{
    public int Id { get; set; }

    public string? Link { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public int? Año { get; set; }

    public int? Kilometraje { get; set; }

    public decimal? PrecioVenta { get; set; }

    public string? Placa { get; set; }

    public decimal? ValorFiscal { get; set; }

    public decimal? PromedioValorMercado { get; set; }

    public decimal? PromedioValorFiscal { get; set; }

    public DateTime FechaImportacion { get; set; }
}
