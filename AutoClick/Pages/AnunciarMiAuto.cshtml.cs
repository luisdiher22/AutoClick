using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;

namespace AutoClick.Pages
{
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
            Console.WriteLine("=== OnPostAsync called ===");
            Console.WriteLine($"Request handler: {(Request.Form.ContainsKey("handler") ? Request.Form["handler"] : "No handler")}");
            
            // Si no hay handler específico, usar el método general
            if (!Request.Form.ContainsKey("handler"))
            {
                return Page();
            }
            
            var handler = Request.Form["handler"].ToString();
            Console.WriteLine($"Handler value: {handler}");
            
            if (handler.Equals("Finalizar", StringComparison.OrdinalIgnoreCase))
            {
                return await OnPostFinalizarAsync();
            }
            
            return Page();
        }

        // Endpoint para verificar si una placa ya existe
        public IActionResult OnGetVerificarPlaca(string placa, string? editId)
        {
            Console.WriteLine($"=== Verificando placa: {placa} ===");
            
            if (string.IsNullOrWhiteSpace(placa))
            {
                return new JsonResult(new { existe = false });
            }

            // Normalizar la placa (mayúsculas y sin espacios)
            var placaNormalizada = placa.Trim().ToUpper();

            // Buscar si existe la placa en la base de datos
            var placaExiste = _context.Autos
                .Where(a => a.PlacaVehiculo != null && a.PlacaVehiculo.ToUpper() == placaNormalizada)
                .Where(a => a.Activo == true) // Solo verificar autos activos
                .Any();

            // Si estamos editando, excluir el auto actual de la búsqueda
            if (!string.IsNullOrEmpty(editId) && int.TryParse(editId, out int id))
            {
                placaExiste = _context.Autos
                    .Where(a => a.PlacaVehiculo != null && a.PlacaVehiculo.ToUpper() == placaNormalizada)
                    .Where(a => a.Activo == true)
                    .Where(a => a.Id != id) // Excluir el auto que se está editando
                    .Any();
            }

            Console.WriteLine($"Placa {placaNormalizada} existe: {placaExiste}");

            return new JsonResult(new { existe = placaExiste });
        }

        public async Task<IActionResult> OnPostFinalizarAsync()
        {
            try
            {
                Console.WriteLine("=== OnPostFinalizarAsync called ===");
                
                // Debug: Check all form keys
                Console.WriteLine("=== Form Keys ===");
                foreach (var key in Request.Form.Keys)
                {
                    Console.WriteLine($"Form key: {key}, Value: {Request.Form[key]}");
                }
                
                // Debug: Check files specifically
                Console.WriteLine("=== Form Files ===");
                Console.WriteLine($"Request.Form.Files.Count: {Request.Form.Files.Count}");
                foreach (var file in Request.Form.Files)
                {
                    Console.WriteLine($"Form file: {file.Name}, FileName: {file.FileName}, Size: {file.Length}");
                }
                
                Console.WriteLine($"Fotos count: {Fotos?.Count ?? 0}");
                
                if (Fotos != null)
                {
                    foreach (var foto in Fotos)
                    {
                        Console.WriteLine($"File: {foto.FileName}, Size: {foto.Length}");
                    }
                }
                else
                {
                    Console.WriteLine("Fotos is null");
                }
                
                var userEmail = User?.Identity?.Name;
                Console.WriteLine($"User email: {userEmail}");
                
                // Para testing, usar un email por defecto si no hay usuario autenticado
                if (string.IsNullOrEmpty(userEmail))
                {
                    Console.WriteLine("No user email found, using default test email");
                    userEmail = "test@autoclick.cr"; // Email por defecto para testing
                }

                Console.WriteLine($"Edit mode: {IsEditMode}, Edit value: {Edit}");
                Console.WriteLine($"Formulario data: Marca={Formulario.Marca}, Modelo={Formulario.Modelo}, Precio={Formulario.Precio}");

                if (!string.IsNullOrEmpty(Edit) && int.TryParse(Edit, out int editId))
                {
                    Console.WriteLine($"Updating auto with ID: {editId}");
                    var auto = await ActualizarAutoAsync(editId, userEmail);
                    Console.WriteLine($"Auto updated successfully: {auto.Id}");
                    return RedirectToPage("/Index");
                }
                else
                {
                    Console.WriteLine("Creating new auto");
                    var auto = await CrearAutoAsync(userEmail);
                    Console.WriteLine($"Auto created successfully: {auto.Id}");
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnPostFinalizarAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Error al procesar el formulario: " + ex.Message);
                return Page();
            }
        }

        private async Task<Auto> CrearAutoAsync(string userEmail)
        {
            Console.WriteLine("=== Processing images ===");
            
            // Procesar imágenes primero
            var imagenesUrls = new List<string>();
            string? imagenPrincipal = null;
            
            if (Fotos != null && Fotos.Any())
            {
                Console.WriteLine($"Found {Fotos.Count} files to upload");
                
                try
                {
                    // Subir todas las imágenes
                    var uploadedUrls = await _fileUploadService.UploadFilesAsync(Fotos, "autos");
                    imagenesUrls = uploadedUrls;
                    
                    // La primera imagen será la principal
                    imagenPrincipal = imagenesUrls.FirstOrDefault();
                    
                    Console.WriteLine($"Successfully uploaded {imagenesUrls.Count} images");
                    Console.WriteLine($"Main image: {imagenPrincipal}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error uploading images: {ex.Message}");
                    throw new Exception("Error al subir las imágenes. Por favor, intenta de nuevo.", ex);
                }
            }
            else
            {
                Console.WriteLine("No files found to upload");
            }

            Console.WriteLine("=== Creating auto record in database ===");
            
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
                Console.WriteLine("Saving auto to database...");
                await _context.SaveChangesAsync();
                Console.WriteLine($"Auto created successfully with ID: {auto.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new Exception("Error al guardar en la base de datos. Por favor, intenta de nuevo.", ex);
            }
            
            Console.WriteLine($"Images URLs: {auto.ImagenesUrls}");
            Console.WriteLine($"Main image: {auto.ImagenPrincipal}");
            
            return auto;
        }

        private async Task<Auto> ActualizarAutoAsync(int id, string userEmail)
        {
            var auto = _context.Autos.FirstOrDefault(a => a.Id == id);
            if (auto == null)
            {
                throw new InvalidOperationException("Auto no encontrado");
            }

            Console.WriteLine("=== Processing images for update ===");
            
            // Procesar nuevas imágenes si se proporcionaron
            if (Fotos != null && Fotos.Any())
            {
                Console.WriteLine($"Found {Fotos.Count} new files to upload");
                
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
                    
                    Console.WriteLine($"Successfully uploaded {newImageUrls.Count} new images");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error uploading new images: {ex.Message}");
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
            
            Console.WriteLine($"Auto updated. Images URLs: {auto.ImagenesUrls}");
            Console.WriteLine($"Main image: {auto.ImagenPrincipal}");
            
            return auto;
        }
    }
}