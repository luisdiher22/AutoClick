using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages
{
    [Authorize]
    public class AnunciarMiAutoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        [BindProperty]
        public AnuncioFormulario Formulario { get; set; } = new();

        [BindProperty]
        public int SeccionActual { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int? Edit { get; set; }

        public bool IsEditMode => Edit.HasValue;

        public const int TOTAL_SECCIONES = 8;

        public AnunciarMiAutoModel(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Console.WriteLine($"=== OnGetAsync called ===");
            Console.WriteLine($"Edit parameter: {Edit}");
            Console.WriteLine($"Edit.HasValue: {Edit.HasValue}");
            Console.WriteLine($"Query string: {Request.QueryString}");
            
            // Also check if the edit parameter is in query string manually
            if (Request.Query.ContainsKey("edit"))
            {
                Console.WriteLine($"Edit found in query: {Request.Query["edit"]}");
            }
            else
            {
                Console.WriteLine("Edit parameter NOT found in query string");
            }
            
            // Verificar si el usuario está autenticado (mismo sistema que MisAnuncios)
            if (!User.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine("User not authenticated");
                return RedirectToPage("/Auth");
            }

            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? User.FindFirst("email")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                
            Console.WriteLine($"User email: {userEmail}");
                
            if (string.IsNullOrEmpty(userEmail))
            {
                Console.WriteLine("No valid user email found");
                return RedirectToPage("/Auth");
            }

            // Check for edit mode - try both the bound property and query string
            int? editId = Edit;
            if (!editId.HasValue && Request.Query.ContainsKey("edit"))
            {
                if (int.TryParse(Request.Query["edit"], out int parsedId))
                {
                    editId = parsedId;
                    Edit = parsedId; // Set the bound property too
                    Console.WriteLine($"Edit ID obtained from query string: {parsedId}");
                }
            }

            // If edit mode, load existing auto data
            if (editId.HasValue)
            {
                Console.WriteLine($"Edit mode detected, loading auto data for ID: {editId.Value}...");
                await LoadAutoForEditAsync(editId.Value, userEmail);
            }
            else
            {
                Console.WriteLine("Create mode, initializing empty form");
                // Initialize form if needed only in create mode
                if (Formulario == null)
                {
                    Formulario = new AnuncioFormulario();
                }
            }

            return Page();
        }

        private async Task LoadAutoForEditAsync(int autoId, string userEmail)
        {
            Console.WriteLine($"=== LoadAutoForEditAsync called with ID: {autoId}, Email: {userEmail} ===");
            
            try
            {
                var auto = await _context.Autos.FirstOrDefaultAsync(a => a.Id == autoId && a.EmailPropietario == userEmail);
                
                if (auto == null)
                {
                    Console.WriteLine("Auto not found or doesn't belong to user");
                    return;
                }

                Console.WriteLine($"Auto found: {auto.Marca} {auto.Modelo} {auto.Ano}");

                // Populate the form with existing data
                Formulario = new AnuncioFormulario
                {
                    // Datos básicos del vehículo
                    Marca = auto.Marca,
                    Modelo = auto.Modelo,
                    Año = auto.Ano,
                    Placa = auto.PlacaVehiculo,
                    Precio = auto.ValorFiscal,
                    Carroceria = auto.Carroceria,
                    Combustible = auto.Combustible,
                    Cilindrada = !string.IsNullOrEmpty(auto.Cilindrada) && int.TryParse(auto.Cilindrada, out int cilindrada) ? cilindrada : 0,
                    Kilometraje = auto.Kilometraje,
                    Transmision = auto.Transmision,
                    Traccion = auto.Traccion,
                    Pasajeros = auto.NumeroPasajeros,
                    Puertas = auto.NumeroPuertas,
                    ColorExterior = auto.ColorExterior,
                    ColorInterior = auto.ColorInterior,
                    Condicion = auto.Condicion,

                    // Información del formulario adicional
                    ImpuestosAlDia = true, // Los autos existentes se asumen al día
                    RecibeVehiculos = false, // Default
                    Descripcion = $"Vendo {auto.NombreCompleto} en excelentes condiciones", // Description por defecto

                    // Ubicación
                    Provincia = auto.Provincia,
                    Canton = auto.Canton,
                    UbicacionExacta = auto.UbicacionExacta,

                    // Plan y configuración
                    PlanVisibilidad = auto.PlanVisibilidad.ToString(),

                    // Deserializar extras si existen
                    ExtrasExterior = DeserializeStringList(auto.ExtrasExterior),
                    ExtrasInterior = DeserializeStringList(auto.ExtrasInterior),
                    ExtrasMultimedia = DeserializeStringList(auto.ExtrasMultimedia),
                    ExtrasSeguridad = DeserializeStringList(auto.ExtrasSeguridad),
                    ExtrasRendimiento = DeserializeStringList(auto.ExtrasRendimiento),
                    ExtrasAntiRobo = DeserializeStringList(auto.ExtrasAntiRobo)
                };

                Console.WriteLine($"Formulario populated with:");
                Console.WriteLine($"  Marca: '{Formulario.Marca}'");
                Console.WriteLine($"  Modelo: '{Formulario.Modelo}'");
                Console.WriteLine($"  Año: '{Formulario.Año}'");
                Console.WriteLine($"  Precio: '{Formulario.Precio}'");
                Console.WriteLine($"  Placa: '{Formulario.Placa}'");
                Console.WriteLine($"  Carroceria: '{Formulario.Carroceria}'");
                Console.WriteLine($"  Combustible: '{Formulario.Combustible}'");
                Console.WriteLine($"  Descripcion: '{Formulario.Descripcion}'");
                Console.WriteLine($"  ExtrasExterior: [{string.Join(", ", Formulario.ExtrasExterior ?? new List<string>())}]");
                Console.WriteLine($"  ExtrasInterior: [{string.Join(", ", Formulario.ExtrasInterior ?? new List<string>())}]");
                Console.WriteLine($"  ExtrasMultimedia: [{string.Join(", ", Formulario.ExtrasMultimedia ?? new List<string>())}]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading auto for edit: {ex.Message}");
                // Keep empty form if error occurs
            }
        }

        private List<string> DeserializeStringList(string? jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Verificar si el usuario está autenticado (mismo sistema que MisAnuncios)
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Auth");
            }

            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? User.FindFirst("email")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Auth");
            }

            // Handle form submission and section navigation
            if (Request.Form.ContainsKey("accion"))
            {
                var accion = Request.Form["accion"].ToString();
                
                switch (accion)
                {
                    case "siguiente":
                        return OnPostSiguienteSeccion();
                    case "anterior":
                        return OnPostSeccionAnterior();
                    case "finalizar":
                        return await OnPostFinalizarAsync();
                    default:
                        return Page();
                }
            }

            return Page();
        }

        public IActionResult OnPostSiguienteSeccion()
        {
            // Validate current section before moving to next
            if (ValidarSeccionActual())
            {
                if (SeccionActual < TOTAL_SECCIONES)
                {
                    SeccionActual++;
                }
            }
            
            return Page();
        }

        public IActionResult OnPostSeccionAnterior()
        {
            if (SeccionActual > 1)
            {
                SeccionActual--;
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostFinalizarAsync()
        {
            // Get user email using Claims (consistent with MisAnuncios)
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? User.FindFirst("email")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            
            Console.WriteLine("=== OnPostFinalizarAsync called ===");
            Console.WriteLine($"UserEmail: {userEmail}");
            Console.WriteLine($"Edit mode: {Edit.HasValue}");
            Console.WriteLine($"Formulario.Marca: {Formulario.Marca}");
            Console.WriteLine($"ValidarFormularioCompleto: {ValidarFormularioCompleto()}");
            
            if (ValidarFormularioCompleto())
            {
                try
                {
                    if (Edit.HasValue)
                    {
                        Console.WriteLine($"=== Updating auto ID: {Edit.Value} in database ===");
                        // Actualizar el auto existente en la base de datos
                        var auto = await ActualizarAutoAsync(Edit.Value, userEmail!);
                        
                        Console.WriteLine($"=== Auto updated successfully! ID: {auto.Id} ===");
                        TempData["MensajeExito"] = $"¡Su anuncio ha sido actualizado exitosamente!";
                        return RedirectToPage("/MisAnuncios");
                    }
                    else
                    {
                        Console.WriteLine("=== Creating new auto in database ===");
                        // Crear nuevo auto en la base de datos
                        var auto = await CrearAutoAsync(userEmail!);
                        
                        Console.WriteLine($"=== Auto created successfully! ID: {auto.Id} ===");
                        TempData["MensajeExito"] = $"¡Su anuncio ha sido publicado exitosamente! ID: {auto.Id}";
                        return RedirectToPage("/Index");
                    }
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe un vehículo registrado con la placa"))
                {
                    // Error específico de placa duplicada
                    Console.WriteLine($"=== ERROR: Placa duplicada - {Formulario.Placa} ===");
                    ModelState.AddModelError("Formulario.Placa", ex.Message);
                    SeccionActual = 1; // Volver a la primera sección donde está la placa
                    TempData["MensajeError"] = ex.Message;
                    return Page();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== ERROR {(Edit.HasValue ? "updating" : "creating")} auto: {ex.Message} ===");
                    Console.WriteLine($"=== Stack trace: {ex.StackTrace} ===");
                    TempData["MensajeError"] = $"Error al {(Edit.HasValue ? "actualizar" : "publicar")} el anuncio: {ex.Message}";
                    return Page();
                }
            }
            else
            {
                Console.WriteLine("=== Form validation failed ===");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }
            
            return Page();
        }

        private async Task<Auto> CrearAutoAsync(string userEmail)
        {
            // Subir archivos primero
            var imagenesUrls = new List<string>();
            var videosUrls = new List<string>();
            
            Console.WriteLine($"=== PROCESANDO ARCHIVOS ===");
            Console.WriteLine($"Formulario.Fotos count: {Formulario.Fotos?.Count ?? 0}");
            
            if (Formulario.Fotos != null && Formulario.Fotos.Any())
            {
                Console.WriteLine($"Subiendo {Formulario.Fotos.Count} imágenes...");
                foreach (var foto in Formulario.Fotos)
                {
                    Console.WriteLine($"Archivo: {foto.FileName}, Tamaño: {foto.Length} bytes");
                }
                imagenesUrls = await _fileUploadService.UploadFilesAsync(Formulario.Fotos, "autos-images");
                Console.WriteLine($"URLs generadas: {string.Join(", ", imagenesUrls)}");
            }
            else
            {
                Console.WriteLine("No se recibieron archivos de imagen");
            }            if (Formulario.Video != null)
            {
                var videoUrl = await _fileUploadService.UploadFileAsync(Formulario.Video, "autos-videos");
                videosUrls.Add(videoUrl);
            }

            // Crear el auto
            var auto = new Auto
            {
                // Datos del vehículo
                Marca = Formulario.Marca,
                Modelo = Formulario.Modelo,
                Ano = Formulario.Año,
                PlacaVehiculo = string.IsNullOrWhiteSpace(Formulario.Placa) ? null : Formulario.Placa.Trim(),
                Carroceria = Formulario.Carroceria,
                Combustible = Formulario.Combustible,
                Cilindrada = Formulario.Cilindrada.ToString(),
                ColorExterior = Formulario.ColorExterior,
                ColorInterior = Formulario.ColorInterior,
                NumeroPuertas = Formulario.Puertas,
                NumeroPasajeros = Formulario.Pasajeros,
                Transmision = Formulario.Transmision,
                Traccion = Formulario.Traccion,
                Kilometraje = Formulario.Kilometraje,
                Condicion = Formulario.Condicion,
                ValorFiscal = Formulario.Precio,

                // Equipamiento (convertir lista a JSON)
                ExtrasExterior = JsonSerializer.Serialize(Formulario.ExtrasExterior ?? new List<string>()),
                ExtrasInterior = JsonSerializer.Serialize(Formulario.ExtrasInterior ?? new List<string>()),
                ExtrasMultimedia = JsonSerializer.Serialize(Formulario.ExtrasMultimedia ?? new List<string>()),
                ExtrasSeguridad = JsonSerializer.Serialize(Formulario.ExtrasSeguridad ?? new List<string>()),
                ExtrasRendimiento = JsonSerializer.Serialize(Formulario.ExtrasRendimiento ?? new List<string>()),
                ExtrasAntiRobo = JsonSerializer.Serialize(Formulario.ExtrasAntiRobo ?? new List<string>()),

                // Ubicación
                Provincia = Formulario.Provincia,
                Canton = Formulario.Canton ?? string.Empty,
                UbicacionExacta = Formulario.UbicacionExacta ?? string.Empty,

                // Plan y configuración
                PlanVisibilidad = string.IsNullOrEmpty(Formulario.PlanVisibilidad) ? 1 : int.Parse(Formulario.PlanVisibilidad),
                BanderinAdquirido = Formulario.Tags?.Count ?? 0,

                // Multimedia
                ImagenesUrls = JsonSerializer.Serialize(imagenesUrls),
                VideosUrls = JsonSerializer.Serialize(videosUrls),
                ImagenPrincipal = imagenesUrls.FirstOrDefault() ?? string.Empty,

                // Usuario propietario
                EmailPropietario = userEmail,
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow,
                Activo = true
            };

            _context.Autos.Add(auto);
            
            try
            {
                await _context.SaveChangesAsync();
                return auto;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("UNIQUE constraint failed: Autos.PlacaVehiculo") == true)
            {
                // Error específico: placa duplicada (solo si la placa no es nula/vacía)
                if (!string.IsNullOrWhiteSpace(Formulario.Placa))
                {
                    var mensaje = $"Ya existe un vehículo registrado con la placa '{Formulario.Placa}'. Por favor, verifica que la placa sea correcta.";
                    Console.WriteLine($"=== ERROR: Placa duplicada - {Formulario.Placa} ===");
                    throw new InvalidOperationException(mensaje, ex);
                }
                else
                {
                    // Esto no debería ocurrir con placas nulas, pero por seguridad
                    var mensaje = "Error inesperado al guardar el vehículo. Por favor, intenta nuevamente.";
                    Console.WriteLine($"=== ERROR: Constraint violation with null/empty plate ===");
                    throw new InvalidOperationException(mensaje, ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR creating auto: {ex.Message} ===");
                Console.WriteLine($"=== Stack trace: {ex.StackTrace} ===");
                throw;
            }
        }

        private async Task<Auto> ActualizarAutoAsync(int autoId, string userEmail)
        {
            // Find the existing auto and verify ownership
            var auto = await _context.Autos.FirstOrDefaultAsync(a => a.Id == autoId && a.EmailPropietario == userEmail);
            
            if (auto == null)
            {
                throw new InvalidOperationException("No se encontró el anuncio o no tiene permisos para editarlo.");
            }

            // Process file uploads if needed
            var imagenesUrls = auto.ImagenesUrlsList; // Keep existing images
            var videosUrls = auto.VideosUrlsList;     // Keep existing videos
            
            Console.WriteLine($"=== ACTUALIZANDO ARCHIVOS ===");
            Console.WriteLine($"Formulario.Fotos count: {Formulario.Fotos?.Count ?? 0}");
            
            // Only upload new images if provided
            if (Formulario.Fotos != null && Formulario.Fotos.Any())
            {
                Console.WriteLine($"Subiendo {Formulario.Fotos.Count} imágenes nuevas...");
                var nuevasImagenesUrls = await _fileUploadService.UploadFilesAsync(Formulario.Fotos, "autos-images");
                imagenesUrls.AddRange(nuevasImagenesUrls);
                Console.WriteLine($"Nuevas URLs generadas: {string.Join(", ", nuevasImagenesUrls)}");
            }
            
            // Only upload new video if provided
            if (Formulario.Video != null)
            {
                var videoUrl = await _fileUploadService.UploadFileAsync(Formulario.Video, "autos-videos");
                videosUrls.Add(videoUrl);
            }

            try
            {
                // Update the auto with new data
                auto.Marca = Formulario.Marca;
                auto.Modelo = Formulario.Modelo;
                auto.Ano = Formulario.Año;
                auto.PlacaVehiculo = string.IsNullOrWhiteSpace(Formulario.Placa) ? null : Formulario.Placa.Trim();
                auto.Carroceria = Formulario.Carroceria;
                auto.Combustible = Formulario.Combustible;
                auto.Cilindrada = Formulario.Cilindrada.ToString();
                auto.ColorExterior = Formulario.ColorExterior;
                auto.ColorInterior = Formulario.ColorInterior;
                auto.NumeroPuertas = Formulario.Puertas;
                auto.NumeroPasajeros = Formulario.Pasajeros;
                auto.Transmision = Formulario.Transmision;
                auto.Traccion = Formulario.Traccion;
                auto.Kilometraje = Formulario.Kilometraje;
                auto.Condicion = Formulario.Condicion;
                auto.ValorFiscal = Formulario.Precio;

                // Update equipment (convert lists to JSON)
                auto.ExtrasExterior = JsonSerializer.Serialize(Formulario.ExtrasExterior ?? new List<string>());
                auto.ExtrasInterior = JsonSerializer.Serialize(Formulario.ExtrasInterior ?? new List<string>());
                auto.ExtrasMultimedia = JsonSerializer.Serialize(Formulario.ExtrasMultimedia ?? new List<string>());
                auto.ExtrasSeguridad = JsonSerializer.Serialize(Formulario.ExtrasSeguridad ?? new List<string>());
                auto.ExtrasRendimiento = JsonSerializer.Serialize(Formulario.ExtrasRendimiento ?? new List<string>());
                auto.ExtrasAntiRobo = JsonSerializer.Serialize(Formulario.ExtrasAntiRobo ?? new List<string>());

                // Update location
                auto.Provincia = Formulario.Provincia;
                auto.Canton = Formulario.Canton ?? string.Empty;
                auto.UbicacionExacta = Formulario.UbicacionExacta ?? string.Empty;

                // Update plan and configuration
                auto.PlanVisibilidad = string.IsNullOrEmpty(Formulario.PlanVisibilidad) ? auto.PlanVisibilidad : int.Parse(Formulario.PlanVisibilidad);
                auto.BanderinAdquirido = Formulario.Tags?.Count ?? auto.BanderinAdquirido;

                // Update multimedia
                auto.ImagenesUrls = JsonSerializer.Serialize(imagenesUrls);
                auto.VideosUrls = JsonSerializer.Serialize(videosUrls);
                auto.ImagenPrincipal = imagenesUrls.FirstOrDefault() ?? auto.ImagenPrincipal;

                // Update timestamps
                auto.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return auto;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("UNIQUE constraint failed: Autos.PlacaVehiculo") == true)
            {
                // Error específico: placa duplicada (solo si la placa no es nula/vacía)
                if (!string.IsNullOrWhiteSpace(Formulario.Placa))
                {
                    var mensaje = $"Ya existe otro vehículo registrado con la placa '{Formulario.Placa}'. Por favor, verifica que la placa sea correcta.";
                    Console.WriteLine($"=== ERROR: Placa duplicada al actualizar - {Formulario.Placa} ===");
                    throw new InvalidOperationException(mensaje, ex);
                }
                else
                {
                    var mensaje = "Error inesperado al actualizar el vehículo. Por favor, intenta nuevamente.";
                    Console.WriteLine($"=== ERROR: Constraint violation with null/empty plate during update ===");
                    throw new InvalidOperationException(mensaje, ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR updating auto: {ex.Message} ===");
                Console.WriteLine($"=== Stack trace: {ex.StackTrace} ===");
                throw;
            }
        }

        private bool ValidarSeccionActual()
        {
            switch (SeccionActual)
            {
                case 1: // Datos del vehículo
                    return ValidarDatosVehiculo();
                case 2: // Equipamiento
                    return true; // Equipment is optional
                case 3: // Ubicación
                    return ValidarUbicacion();
                case 4: // Planes de visibilidad
                    return true; // Visibility plans are optional
                case 5: // Tags
                    return true; // Tags are optional
                case 6: // Pago
                    return ValidarPago();
                case 7: // Archivos
                    return ValidarArchivos();
                case 8: // Confirmación
                    return true;
                default:
                    return true;
            }
        }



        private bool ValidarDatosVehiculo()
        {
            Console.WriteLine("=== VALIDANDO DATOS DEL VEHICULO ===");
            Console.WriteLine($"Marca: '{Formulario.Marca}'");
            Console.WriteLine($"Modelo: '{Formulario.Modelo}'");
            Console.WriteLine($"Año: {Formulario.Año}");
            Console.WriteLine($"Placa: '{Formulario.Placa}' (opcional)");
            Console.WriteLine($"Carroceria: '{Formulario.Carroceria}'");
            Console.WriteLine($"Combustible: '{Formulario.Combustible}'");
            Console.WriteLine($"Cilindrada: {Formulario.Cilindrada}");
            
            var esValido = true;

            if (string.IsNullOrWhiteSpace(Formulario.Marca))
            {
                ModelState.AddModelError("Formulario.Marca", "La marca es requerida");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.Modelo))
            {
                ModelState.AddModelError("Formulario.Modelo", "El modelo es requerido");
                esValido = false;
            }

            if (Formulario.Año == 0)
            {
                ModelState.AddModelError("Formulario.Año", "El año es requerido");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.Carroceria))
            {
                ModelState.AddModelError("Formulario.Carroceria", "El tipo de carrocería es requerido");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.Combustible))
            {
                ModelState.AddModelError("Formulario.Combustible", "El tipo de combustible es requerido");
                esValido = false;
            }

            if (Formulario.Cilindrada == 0)
            {
                ModelState.AddModelError("Formulario.Cilindrada", "La cilindrada es requerida");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.ColorExterior))
            {
                ModelState.AddModelError("Formulario.ColorExterior", "El color exterior es requerido");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.ColorInterior))
            {
                ModelState.AddModelError("Formulario.ColorInterior", "El color interior es requerido");
                esValido = false;
            }

            if (Formulario.Puertas == 0)
            {
                ModelState.AddModelError("Formulario.Puertas", "El número de puertas es requerido");
                esValido = false;
            }

            if (Formulario.Pasajeros == 0)
            {
                ModelState.AddModelError("Formulario.Pasajeros", "El número de pasajeros es requerido");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.Transmision))
            {
                ModelState.AddModelError("Formulario.Transmision", "El tipo de transmisión es requerido");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.Traccion))
            {
                ModelState.AddModelError("Formulario.Traccion", "El tipo de tracción es requerido");
                esValido = false;
            }

            if (Formulario.Kilometraje < 0)
            {
                ModelState.AddModelError("Formulario.Kilometraje", "El kilometraje debe ser un número válido");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.Condicion))
            {
                ModelState.AddModelError("Formulario.Condicion", "La condición del vehículo es requerida");
                esValido = false;
            }

            if (Formulario.Precio <= 0)
            {
                ModelState.AddModelError("Formulario.Precio", "El precio debe ser mayor a cero");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(Formulario.Descripcion))
            {
                ModelState.AddModelError("Formulario.Descripcion", "La descripción del vehículo es requerida");
                esValido = false;
            }

            Console.WriteLine($"=== RESULTADO VALIDACION: {esValido} ===");
            if (!esValido)
            {
                Console.WriteLine("=== ERRORES ENCONTRADOS ===");
                foreach (var error in ModelState)
                {
                    if (error.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Campo: {error.Key}, Errores: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }

            return esValido;
        }

        private bool ValidarUbicacion()
        {
            var esValido = true;

            if (string.IsNullOrWhiteSpace(Formulario.Provincia))
            {
                ModelState.AddModelError("Formulario.Provincia", "La provincia es requerida");
                esValido = false;
            }

            return esValido;
        }

        private bool ValidarPago()
        {
            // Por ahora, permitir continuar sin validación estricta de pago
            // hasta que se implemente el sistema de pagos completo
            return true;
        }

        private bool ValidarArchivos()
        {
            // For now, file upload validation is optional
            // In a real application, you would validate uploaded files here
            return true;
        }

        private bool ValidarFormularioCompleto()
        {
            return ValidarDatosVehiculo() && 
                   ValidarUbicacion() && 
                   ValidarPago() && 
                   ValidarArchivos();
        }
    }

    public class AnuncioFormulario
    {

        // Datos del Vehículo
        [Required(ErrorMessage = "La marca es requerida")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es requerido")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año es requerido")]
        [Range(1990, 2025, ErrorMessage = "Año inválido")]
        public int Año { get; set; }

        [Display(Name = "Placa del vehículo (opcional para autos nuevos)")]
        public string? Placa { get; set; }

        [Required(ErrorMessage = "El tipo de carrocería es requerido")]
        public string Carroceria { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de combustible es requerido")]
        public string Combustible { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cilindrada es requerida")]
        [Range(1, 10000, ErrorMessage = "Cilindrada inválida")]
        public int Cilindrada { get; set; }

        [Required(ErrorMessage = "El color exterior es requerido")]
        public string ColorExterior { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color interior es requerido")]
        public string ColorInterior { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de puertas es requerido")]
        [Range(2, 5, ErrorMessage = "Número de puertas inválido")]
        public int Puertas { get; set; }

        [Required(ErrorMessage = "El número de pasajeros es requerido")]
        [Range(2, 15, ErrorMessage = "Número de pasajeros inválido")]
        public int Pasajeros { get; set; }

        [Required(ErrorMessage = "El tipo de transmisión es requerido")]
        public string Transmision { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de tracción es requerido")]
        public string Traccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El kilometraje es requerido")]
        [Range(0, 999999, ErrorMessage = "Kilometraje inválido")]
        public int Kilometraje { get; set; }

        [Required(ErrorMessage = "La condición del vehículo es requerida")]
        public string Condicion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(1, 999999999, ErrorMessage = "Precio inválido")]
        public decimal Precio { get; set; }

        public bool PrecioNegociable { get; set; }

        public bool ImpuestosAlDia { get; set; } = true;

        public bool RecibeVehiculos { get; set; }

        [Required(ErrorMessage = "La descripción del vehículo es requerida")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        // Equipamiento - Los nombres deben coincidir con el JavaScript
        public List<string> ExtrasExterior { get; set; } = new();
        public List<string> ExtrasInterior { get; set; } = new();
        public List<string> ExtrasMultimedia { get; set; } = new();
        public List<string> ExtrasSeguridad { get; set; } = new();
        public List<string> ExtrasRendimiento { get; set; } = new();
        public List<string> ExtrasAntiRobo { get; set; } = new();

        // Ubicación
        [Required(ErrorMessage = "La provincia es requerida")]
        public string Provincia { get; set; } = string.Empty;

        public string Canton { get; set; } = string.Empty;

        public string UbicacionExacta { get; set; } = string.Empty;

        public bool GuardarDireccion { get; set; }

        // Planes de Visibilidad
        public string PlanVisibilidad { get; set; } = string.Empty;

        // Tags
        public List<string> Tags { get; set; } = new();

        // Método de Pago
        public string MetodoPago { get; set; } = "tarjeta";

        public string NumeroTarjeta { get; set; } = string.Empty;

        public string NombreTarjeta { get; set; } = string.Empty;

        public string FechaExpiracion { get; set; } = string.Empty;

        public string CVV { get; set; } = string.Empty;

        public bool GuardarMetodoPago { get; set; }

        [Required(ErrorMessage = "Debe aceptar los términos y condiciones")]
        public bool AceptoTerminos { get; set; }

        // Archivos
        public List<IFormFile> Fotos { get; set; } = new();

        public IFormFile? Video { get; set; }
    }
}