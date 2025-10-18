using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace AutoClick.Services
{
    public interface IVentasExternasService
    {
        Task<(bool success, string message, int recordsImported)> ImportarExcelAsync(Stream fileStream, string fileName);
        Task<Dictionary<string, decimal>> ObtenerPromediosPorModeloYAno(string modelo, int año);
        Task<int> ObtenerTotalRegistros();
        Task<List<VentaExterna>> ObtenerVentasPorModeloYAno(string modelo, int año);
    }

    public class VentasExternasService : IVentasExternasService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VentasExternasService> _logger;

        public VentasExternasService(ApplicationDbContext context, ILogger<VentasExternasService> logger)
        {
            _context = context;
            _logger = logger;
            
            // Configurar licencia de EPPlus (no comercial)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<(bool success, string message, int recordsImported)> ImportarExcelAsync(Stream fileStream, string fileName)
        {
            try
            {
                // Validar extensión del archivo
                var extension = Path.GetExtension(fileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    return (false, "El archivo debe ser un archivo Excel válido (.xlsx o .xls)", 0);
                }

                var ventasImportadas = new List<VentaExterna>();

                using (var package = new ExcelPackage(fileStream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    
                    if (worksheet == null)
                    {
                        return (false, "El archivo Excel no contiene hojas de cálculo", 0);
                    }

                    var rowCount = worksheet.Dimension?.Rows ?? 0;
                    
                    if (rowCount <= 1)
                    {
                        return (false, "El archivo Excel está vacío o solo contiene encabezados", 0);
                    }

                    // Mapeo de columnas esperadas (puedes ajustar según el formato real del Excel)
                    // Asumiendo que la primera fila contiene los encabezados
                    var headers = new Dictionary<string, int>();
                    
                    if (worksheet.Dimension == null)
                    {
                        return (false, "El archivo Excel está vacío", 0);
                    }
                    
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        var headerValue = worksheet.Cells[1, col].Text?.Trim().ToUpper() ?? "";
                        headers[headerValue] = col;
                    }

                    // Validar que existan las columnas mínimas requeridas
                    // Ya no validamos columnas requeridas porque ahora todo es opcional
                    // Solo verificamos que haya al menos algunas columnas
                    if (headers.Count == 0)
                    {
                        return (false, "El archivo Excel no contiene columnas válidas", 0);
                    }

                    // Leer datos desde la fila 2 (después de los encabezados)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var venta = new VentaExterna
                            {
                                Link = GetStringValue(worksheet, row, headers, "LINK"),
                                Marca = GetStringValue(worksheet, row, headers, "MARCA"),
                                Modelo = GetStringValue(worksheet, row, headers, "MODELO"),
                                Año = GetIntValue(worksheet, row, headers, "AÑO"),
                                Kilometraje = GetIntValue(worksheet, row, headers, "KILOMETRAJE"),
                                PrecioVenta = GetDecimalValue(worksheet, row, headers, "PRECIO DE VENTA"),
                                Placa = GetStringValue(worksheet, row, headers, "PLACA"),
                                ValorFiscal = GetDecimalValue(worksheet, row, headers, "VALOR FISCAL"),
                                PromedioValorMercado = GetDecimalValue(worksheet, row, headers, "PROMEDIO VALOR MERCADO"),
                                PromedioValorFiscal = GetDecimalValue(worksheet, row, headers, "PROMEDIO VALOR FISCAL"),
                                FechaImportacion = DateTime.Now
                            };

                            // Agregar el registro aunque tenga campos vacíos
                            // Solo validamos que al menos tenga algún dato
                            if (!string.IsNullOrWhiteSpace(venta.Marca) || 
                                !string.IsNullOrWhiteSpace(venta.Modelo) ||
                                venta.PrecioVenta.HasValue)
                            {
                                ventasImportadas.Add(venta);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Error al procesar la fila {row}: {ex.Message}");
                            // Continuar con la siguiente fila
                        }
                    }
                }

                if (ventasImportadas.Count == 0)
                {
                    return (false, "No se pudieron importar registros válidos del archivo Excel", 0);
                }

                // Iniciar transacción para eliminar todos los registros existentes e insertar los nuevos
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Eliminar todos los registros existentes
                        await _context.Database.ExecuteSqlRawAsync("DELETE FROM VentasExternas");
                        
                        // Insertar los nuevos registros
                        await _context.VentasExternas.AddRangeAsync(ventasImportadas);
                        await _context.SaveChangesAsync();
                        
                        await transaction.CommitAsync();
                        
                        _logger.LogInformation($"Se importaron exitosamente {ventasImportadas.Count} registros de ventas externas");
                        return (true, $"Se importaron exitosamente {ventasImportadas.Count} registros", ventasImportadas.Count);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error al guardar los datos en la base de datos");
                        return (false, $"Error al guardar los datos: {ex.Message}", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar el archivo Excel");
                return (false, $"Error al procesar el archivo: {ex.Message}", 0);
            }
        }

        public async Task<Dictionary<string, decimal>> ObtenerPromediosPorModeloYAno(string modelo, int año)
        {
            try
            {
                var ventas = await _context.VentasExternas
                    .Where(v => v.Modelo != null && v.Modelo.ToLower() == modelo.ToLower() && v.Año == año && v.PrecioVenta.HasValue)
                    .ToListAsync();

                if (!ventas.Any())
                {
                    return new Dictionary<string, decimal>();
                }

                return new Dictionary<string, decimal>
                {
                    { "PromedioPrecioVenta", ventas.Average(v => v.PrecioVenta!.Value) },
                    { "CantidadRegistros", ventas.Count },
                    { "PrecioMinimo", ventas.Min(v => v.PrecioVenta!.Value) },
                    { "PrecioMaximo", ventas.Max(v => v.PrecioVenta!.Value) }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al calcular promedios para {modelo} {año}");
                return new Dictionary<string, decimal>();
            }
        }

        public async Task<int> ObtenerTotalRegistros()
        {
            return await _context.VentasExternas.CountAsync();
        }

        public async Task<List<VentaExterna>> ObtenerVentasPorModeloYAno(string modelo, int año)
        {
            return await _context.VentasExternas
                .Where(v => v.Modelo != null && v.Modelo.ToLower() == modelo.ToLower() && v.Año == año)
                .OrderBy(v => v.PrecioVenta)
                .ToListAsync();
        }

        // Métodos auxiliares para parsear valores del Excel
        private static string? GetStringValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, string columnName)
        {
            if (!headers.ContainsKey(columnName))
                return null;

            var value = worksheet.Cells[row, headers[columnName]].Text?.Trim();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static int? GetIntValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, string columnName)
        {
            if (!headers.ContainsKey(columnName))
                return null;

            var value = worksheet.Cells[row, headers[columnName]].Text?.Trim();
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Limpiar el valor: remover comas, puntos (excepto el último que puede ser decimal), espacios
            value = value.Replace(",", "").Replace(" ", "").Replace(".", "");

            if (int.TryParse(value, out var result))
                return result;

            return null;
        }

        private static decimal? GetDecimalValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, string columnName)
        {
            if (!headers.ContainsKey(columnName))
                return null;

            var value = worksheet.Cells[row, headers[columnName]].Text?.Trim();
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Limpiar el valor: remover símbolos de moneda comunes, espacios, y comas de miles
            value = value
                .Replace("$", "")
                .Replace("₡", "")
                .Replace("¢", "")
                .Replace("€", "")
                .Replace("£", "")
                .Replace("₹", "")
                .Replace("¥", "")
                .Replace(",", "")
                .Replace(" ", "")
                .Trim();

            // Intentar parsear como decimal
            if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;

            return null;
        }
    }
}
