using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace AutoClick.Pages
{
    [Authorize]
    [RequestSizeLimit(150_000_000)] // 150 MB - permite hasta 100 MB de imágenes + overhead
    [RequestFormLimits(MultipartBodyLengthLimit = 150_000_000)]
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

        [BindProperty(SupportsGet = false)]
        public List<string>? BanderinesSeleccionados { get; set; }

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

                if (Request.Form.ContainsKey("Formulario.ValorFiscal"))
                {
                    var vfStr = Request.Form["Formulario.ValorFiscal"].ToString();
                    // Remove currency symbols, dots (thousands separator), and whitespace
                    var cleanVf = System.Text.RegularExpressions.Regex.Replace(vfStr, @"[^\d]", "");
                    if (decimal.TryParse(cleanVf, out decimal valorFiscal))
                    {
                        Formulario.ValorFiscal = valorFiscal;
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

                // DEBUG: Log para verificar qué banderines se reciben
                Console.WriteLine($"BanderinesSeleccionados Count: {BanderinesSeleccionados?.Count ?? 0}");
                if (BanderinesSeleccionados != null && BanderinesSeleccionados.Any())
                {
                    Console.WriteLine($"Banderines recibidos: {string.Join(", ", BanderinesSeleccionados)}");
                }
                
                // DEBUG: Verificar también en Request.Form directamente
                if (Request.Form.ContainsKey("BanderinesSeleccionados"))
                {
                    var banderinesFromForm = Request.Form["BanderinesSeleccionados"].ToArray();
                    Console.WriteLine($"BanderinesSeleccionados en Request.Form: {string.Join(", ", banderinesFromForm)}");
                }
                else
                {
                    Console.WriteLine("BanderinesSeleccionados NO está en Request.Form");
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
                    // Retornar JSON con el ID del auto para el frontend
                    return new JsonResult(new { autoId = auto.Id, success = true });
                }
                else
                {
                    var auto = await CrearAutoAsync(userEmail);
                    // Retornar JSON con el ID del auto para el frontend
                    return new JsonResult(new { autoId = auto.Id, success = true });
                }
            }
            catch (DbUpdateException ex)
            {
                // Log completo del error para debugging
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                
                // Verificar si es un error de placa duplicada
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                if (errorMessage.Contains("IX_Autos_PlacaVehiculo") || 
                    errorMessage.Contains("duplicate key") ||
                    errorMessage.Contains("Cannot insert duplicate key"))
                {
                    Console.WriteLine("Error detectado: Placa duplicada");
                    return new JsonResult(new { error = "Ya existe un anuncio registrado con esta placa. Por favor, verifica que hayas ingresado la placa correctamente." }) { StatusCode = 400 };
                }
                
                // Otro error de base de datos
                Console.WriteLine("Error de base de datos no identificado");
                return new JsonResult(new { error = "Error al guardar en la base de datos. Por favor, intenta nuevamente." }) { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                // Error general
                return new JsonResult(new { error = "Ocurrió un error al crear el anuncio. Por favor, intenta nuevamente o contacta a soporte." }) { StatusCode = 500 };
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
                catch (FileUploadException fex)
                {
                    // Error de validación de tamaño - mensaje específico para el usuario
                    throw new Exception(fex.Message, fex);
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
                ValorFiscal = Formulario.ValorFiscal,

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
                BanderinesAdquiridos = BanderinesSeleccionados != null && BanderinesSeleccionados.Any() 
                    ? string.Join(",", BanderinesSeleccionados) 
                    : null,

                // Multimedia - usar las URLs subidas
                ImagenesUrls = System.Text.Json.JsonSerializer.Serialize(imagenesUrls),
                VideosUrls = Formulario.VideosUrls ?? "[]",
                ImagenPrincipal = imagenPrincipal ?? "",

                // Metadatos
                EmailPropietario = userEmail,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now,
                // Todos los autos se crean inactivos:
                // - Planes de pago (1-4): se activan cuando el webhook confirme el pago exitoso
                // - Plan gratuito (5): se activa cuando un administrador lo apruebe
                Activo = false
            };

            // DEBUG: Log antes de guardar
            Console.WriteLine($"AUTO CREADO - BanderinesAdquiridos: {auto.BanderinesAdquiridos ?? "NULL"}");
            Console.WriteLine($"AUTO CREADO - PlanVisibilidad: {auto.PlanVisibilidad}");

            try
            {
                _context.Autos.Add(auto);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"Auto guardado con ID: {auto.Id}");
                
                // Si es un plan gratuito (PlanVisibilidad = 5), crear una solicitud de pre-aprobación
                if (auto.PlanVisibilidad == 5)
                {
                    Console.WriteLine("Creando solicitud de pre-aprobación para plan gratuito...");
                    
                    // Obtener datos del usuario de la base de datos
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
                    
                    var solicitud = new SolicitudPreAprobacion
                    {
                        Nombre = usuario?.Nombre ?? "Usuario",
                        Apellidos = usuario?.Apellidos ?? "AutoClick",
                        Telefono = usuario?.NumeroTelefono ?? "No proporcionado",
                        Email = userEmail,
                        AutoId = auto.Id,
                        FechaSolicitud = DateTime.Now,
                        Procesada = false,
                        Notas = $"Solicitud de aprobación para anuncio gratuito: {auto.Marca} {auto.Modelo} {auto.Ano}, Placa: {auto.PlacaVehiculo}"
                    };
                    
                    _context.SolicitudesPreAprobacion.Add(solicitud);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"Solicitud de pre-aprobación creada con ID: {solicitud.Id} para usuario: {usuario?.Nombre} {usuario?.Apellidos}");
                }
                else
                {
                    Console.WriteLine($"No se crea solicitud. Plan: {auto.PlanVisibilidad}");
                }
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
                catch (FileUploadException fex)
                {
                    // Error de validación de tamaño - relanzar con mensaje específico
                    throw new Exception(fex.Message, fex);
                }
                catch (Exception ex)
                {
                    // Error general al subir imágenes
                    throw new Exception("Error al subir las imágenes. Por favor, intenta de nuevo.", ex);
                }
            }

            // Información básica del vehículo
            auto.Marca = Formulario.Marca;
            auto.Modelo = Formulario.Modelo;
            auto.Ano = Formulario.Ano;
            auto.PlacaVehiculo = Formulario.PlacaVehiculo;
            auto.Precio = Formulario.Precio;
            auto.Divisa = Formulario.Divisa;
            auto.ValorFiscal = Formulario.ValorFiscal;

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
            auto.BanderinesAdquiridos = BanderinesSeleccionados != null && BanderinesSeleccionados.Any()
                ? string.Join(",", BanderinesSeleccionados)
                : null;

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