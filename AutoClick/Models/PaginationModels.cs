namespace AutoClick.Models;

public class AutoFiltros
{
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? Provincia { get; set; }
    public string? Canton { get; set; }
    public decimal? PrecioMin { get; set; }
    public decimal? PrecioMax { get; set; }
    public int? AnoMin { get; set; }
    public int? AnoMax { get; set; }
    public int? KilometrajeMin { get; set; }
    public int? KilometrajeMax { get; set; }
    public string? Carroceria { get; set; }
    public string? Combustible { get; set; }
    public string? Transmision { get; set; }
    public string? Condicion { get; set; }
    public bool SoloDestacados { get; set; } = false;
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}