using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace AutoClick.Pages
{
    [Authorize]
    public class AnunciarMiAutoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public AnunciarMiAutoModel(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [BindProperty]
        public Auto Formulario { get; set; } = new Auto();

        [BindProperty]
        public int SeccionActual { get; set; } = 1;

        [BindProperty]
        public string? Edit { get; set; }

        [BindProperty]
        public List<IFormFile>? Fotos { get; set; }

        public bool IsEditMode => !string.IsNullOrEmpty(Edit);

        public List<(string name, string displayName, string file)> Tags { get; set; } = new()
        {
            ("nuevo", "Nuevo", "Nuevo.gif"),
            ("poco_uso", "Poco Uso", "POCO USO.gif"),
            ("negociable", "Negociable", "Negociable.gif"),
            ("full_extras", "Full Extras", "FULL EXTRAS.gif"),
            ("recibo_vehiculo", "Recibo Vehículo", "RECIBO VEHICULO.gif"),
            ("financiamiento", "Financiamiento", "FINANCIAMIENTO.gif"),
            ("traspaso_incluido", "Traspaso Incluido", "TRASPASO INCLUIDO.gif"),
            ("precio_fijo", "Precio Fijo", "PRECIO FIJO.gif")
        };

        public void OnGet(string? edit)
        {
            // Force UTF-8 encoding
            Response.ContentType = "text/html; charset=utf-8";

            Edit = edit;
            if (!string.IsNullOrEmpty(edit) && int.TryParse(edit, out int id))
            {
                var auto = _context.Autos.FirstOrDefault(a => a.Id == id);
                if (auto != null)
                {
                    Formulario = auto;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {

            // Si no hay handler específico, usar el método general
            if (!Request.Form.ContainsKey("handler"))
            {
                return Page();
            }

            var handler = Request.Form["handler"].ToString();

            if (handler.Equals("Finalizar", StringComparison.OrdinalIgnoreCase))
            {
                return await OnPostFinalizarAsync();
            }

            return Page();
        }

        // Endpoint para verificar si una placa ya existe
        // NOTA: La validación de placa duplicada ha sido removida, 
        // ya que las placas pueden estar duplicadas en la base de datos
        public IActionResult OnGetVerificarPlaca(string placa, string? editId)
        {

            // Siempre retornar que la placa no existe (no validar duplicados)
            return new JsonResult(new { existe = false });
        }

        public async Task<IActionResult> OnPostFinalizarAsync()
        {
            try
            {
                // Fix: Manually parse Price and Mileage to handle formatting (e.g. "$25.000" or "25.000")
                // This bypasses the model binding which fails on formatted strings
                if (Request.Form.ContainsKey("Formulario.Precio"))
                {
                    var precioStr = Request.Form["Formulario.Precio"].ToString();
                    // Remove currency symbols, dots (thousands separator), and whitespace
                    var cleanPrecio = System.Text.RegularExpressions.Regex.Replace(precioStr, @"[^\d]", "");
                    if (decimal.TryParse(cleanPrecio, out decimal precio))
                    {
                        Formulario.Precio = precio;
                    }
                }

                if (Request.Form.ContainsKey("Formulario.Kilometraje"))
                {
                    var kmStr = Request.Form["Formulario.Kilometraje"].ToString();
                    // Remove non-digits
                    var cleanKm = System.Text.RegularExpressions.Regex.Replace(kmStr, @"[^\d]", "");
                    if (int.TryParse(cleanKm, out int km))
                    {
                        Formulario.Kilometraje = km;
                    }
                }

                if (Fotos != null)
                {
                    foreach (var foto in Fotos)
                    {
                    }
                }
                else
                {
                }

                var userEmail = User?.Identity?.Name;

                // Verificar que el usuario esté autenticado
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToPage("/Auth");
                }

                // Procesar y serializar los extras de los checkboxes a JSON
                SerializarExtrasFromForm();

                if (!string.IsNullOrEmpty(Edit) && int.TryParse(Edit, out int editId))
                {
                    var auto = await ActualizarAutoAsync(editId, userEmail);
                    return Redirect("/");
                }
                else
                {
                    var auto = await CrearAutoAsync(userEmail);
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el formulario: " + ex.Message);
                return Page();
            }
        }

        private async Task<Auto> CrearAutoAsync(string userEmail)
        {

            // Procesar imágenes primero
            var imagenesUrls = new List<string>();
            string? imagenPrincipal = null;

            if (Fotos != null && Fotos.Any())
            {

                try
                {
                    // Subir todas las imágenes
                    var uploadedUrls = await _fileUploadService.UploadFilesAsync(Fotos, "autos");
                    imagenesUrls = uploadedUrls;

                    // La primera imagen será la principal
                    imagenPrincipal = imagenesUrls.FirstOrDefault();

                }
                catch (Exception ex)
                {
                    throw new Exception("Error al subir las imágenes. Por favor, intenta de nuevo.", ex);
                }
            }
            else
            {
            }


            var auto = new Auto
            {
                // Información básica del vehículo
                Marca = Formulario.Marca,
                Modelo = Formulario.Modelo,
                Ano = Formulario.Ano,
                PlacaVehiculo = Formulario.PlacaVehiculo,
                Precio = Formulario.Precio,
                Divisa = Formulario.Divisa,

                // Especificaciones técnicas
                Carroceria = Formulario.Carroceria,
                Combustible = Formulario.Combustible,
                Cilindrada = Formulario.Cilindrada,

                // Colores
                ColorExterior = Formulario.ColorExterior,
                ColorInterior = Formulario.ColorInterior,

                // Características físicas
                NumeroPuertas = Formulario.NumeroPuertas,
                NumeroPasajeros = Formulario.NumeroPasajeros,
                Transmision = Formulario.Transmision,
                Traccion = Formulario.Traccion,
                Kilometraje = Formulario.Kilometraje,
                Condicion = Formulario.Condicion,

                // Extras
                ExtrasExterior = Formulario.ExtrasExterior,
                ExtrasInterior = Formulario.ExtrasInterior,
                ExtrasMultimedia = Formulario.ExtrasMultimedia,
                ExtrasSeguridad = Formulario.ExtrasSeguridad,
                ExtrasRendimiento = Formulario.ExtrasRendimiento,
                ExtrasAntiRobo = Formulario.ExtrasAntiRobo,

                // Ubicación
                Provincia = Formulario.Provincia,
                Canton = Formulario.Canton,
                UbicacionExacta = Formulario.UbicacionExacta,

                // Descripción
                Descripcion = Formulario.Descripcion,

                // Configuración del anuncio
                PlanVisibilidad = Formulario.PlanVisibilidad,
                BanderinAdquirido = Formulario.BanderinAdquirido,

                // Multimedia - usar las URLs subidas
                ImagenesUrls = System.Text.Json.JsonSerializer.Serialize(imagenesUrls),
                VideosUrls = Formulario.VideosUrls ?? "[]",
                ImagenPrincipal = imagenPrincipal ?? "",

                // Metadatos
                EmailPropietario = userEmail,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now,
                Activo = true
            };

            try
            {
                _context.Autos.Add(auto);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar en la base de datos. Por favor, intenta de nuevo.", ex);
            }

            return auto;
        }

        private async Task<Auto> ActualizarAutoAsync(int id, string userEmail)
        {
            var auto = _context.Autos.FirstOrDefault(a => a.Id == id);
            if (auto == null)
            {
                throw new InvalidOperationException("Auto no encontrado");
            }


            // Procesar nuevas imágenes si se proporcionaron
            if (Fotos != null && Fotos.Any())
            {

                try
                {
                    // Subir nuevas imágenes
                    var newImageUrls = await _fileUploadService.UploadFilesAsync(Fotos, "autos");

                    // Obtener imágenes existentes
                    var existingImages = auto.ImagenesUrlsList;

                    // Agregar las nuevas imágenes a las existentes
                    existingImages.AddRange(newImageUrls);

                    // Actualizar las URLs de imágenes
                    auto.ImagenesUrls = System.Text.Json.JsonSerializer.Serialize(existingImages);

                    // Si no hay imagen principal, usar la primera nueva
                    if (string.IsNullOrEmpty(auto.ImagenPrincipal))
                    {
                        auto.ImagenPrincipal = newImageUrls.FirstOrDefault() ?? "";
                    }

                }
                catch (Exception ex)
                {
                }
            }

            // Información básica del vehículo
            auto.Marca = Formulario.Marca;
            auto.Modelo = Formulario.Modelo;
            auto.Ano = Formulario.Ano;
            auto.PlacaVehiculo = Formulario.PlacaVehiculo;
            auto.Precio = Formulario.Precio;
            auto.Divisa = Formulario.Divisa;

            // Especificaciones técnicas
            auto.Carroceria = Formulario.Carroceria;
            auto.Combustible = Formulario.Combustible;
            auto.Cilindrada = Formulario.Cilindrada;

            // Colores
            auto.ColorExterior = Formulario.ColorExterior;
            auto.ColorInterior = Formulario.ColorInterior;

            // Características físicas
            auto.NumeroPuertas = Formulario.NumeroPuertas;
            auto.NumeroPasajeros = Formulario.NumeroPasajeros;
            auto.Transmision = Formulario.Transmision;
            auto.Traccion = Formulario.Traccion;
            auto.Kilometraje = Formulario.Kilometraje;
            auto.Condicion = Formulario.Condicion;

            // Extras
            auto.ExtrasExterior = Formulario.ExtrasExterior;
            auto.ExtrasInterior = Formulario.ExtrasInterior;
            auto.ExtrasMultimedia = Formulario.ExtrasMultimedia;
            auto.ExtrasSeguridad = Formulario.ExtrasSeguridad;
            auto.ExtrasRendimiento = Formulario.ExtrasRendimiento;
            auto.ExtrasAntiRobo = Formulario.ExtrasAntiRobo;

            // Ubicación
            auto.Provincia = Formulario.Provincia;
            auto.Canton = Formulario.Canton;
            auto.UbicacionExacta = Formulario.UbicacionExacta;

            // Descripción
            auto.Descripcion = Formulario.Descripcion;

            // Configuración del anuncio
            auto.PlanVisibilidad = Formulario.PlanVisibilidad;
            auto.BanderinAdquirido = Formulario.BanderinAdquirido;

            // No sobreescribir multimedia si no se proporcionaron nuevos archivos
            if (Formulario.Fotos == null || !Formulario.Fotos.Any())
            {
                // Solo actualizar si se proporciona información multimedia en el formulario
                if (!string.IsNullOrEmpty(Formulario.VideosUrls))
                {
                    auto.VideosUrls = Formulario.VideosUrls;
                }
            }

            // Metadatos
            auto.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return auto;
        }

        /// <summary>
        /// Serializa los arrays de checkboxes del formulario a JSON strings.
        /// ASP.NET Core recibe múltiples checkboxes con el mismo nombre como un array,
        /// pero el modelo espera un string JSON. Este método realiza la conversión.
        /// </summary>
        private void SerializarExtrasFromForm()
        {

            // Lista de todas las propiedades de extras que deben ser serializadas
            var extrasProperties = new Dictionary<string, string>
            {
                { "Formulario.ExtrasExterior", "ExtrasExterior" },
                { "Formulario.ExtrasInterior", "ExtrasInterior" },
                { "Formulario.ExtrasMultimedia", "ExtrasMultimedia" },
                { "Formulario.ExtrasSeguridad", "ExtrasSeguridad" },
                { "Formulario.ExtrasRendimiento", "ExtrasRendimiento" },
                { "Formulario.ExtrasAntiRobo", "ExtrasAntiRobo" }
            };

            foreach (var kvp in extrasProperties)
            {
                var propertyKey = kvp.Key;
                var propertyName = kvp.Value;


                if (Request.Form.ContainsKey(propertyKey))
                {
                    // Obtener todos los valores seleccionados para este grupo de checkboxes
                    var selectedValues = Request.Form[propertyKey].ToList();


                    // Serializar a JSON
                    var jsonString = JsonSerializer.Serialize(selectedValues);

                    // Asignar al modelo usando reflection o switch
                    AsignarExtraAlFormulario(propertyName, jsonString);
                }
                else
                {
                    // Si no hay checkboxes seleccionados, asignar array vacío
                    var emptyJson = "[]";
                    AsignarExtraAlFormulario(propertyName, emptyJson);
                }
            }

        }

        private void AsignarExtraAlFormulario(string propertyName, string jsonValue)
        {
            switch (propertyName)
            {
                case "ExtrasExterior":
                    Formulario.ExtrasExterior = jsonValue;
                    break;
                case "ExtrasInterior":
                    Formulario.ExtrasInterior = jsonValue;
                    break;
                case "ExtrasMultimedia":
                    Formulario.ExtrasMultimedia = jsonValue;
                    break;
                case "ExtrasSeguridad":
                    Formulario.ExtrasSeguridad = jsonValue;
                    break;
                case "ExtrasRendimiento":
                    Formulario.ExtrasRendimiento = jsonValue;
                    break;
                case "ExtrasAntiRobo":
                    Formulario.ExtrasAntiRobo = jsonValue;
                    break;
            }
        }
    }
}