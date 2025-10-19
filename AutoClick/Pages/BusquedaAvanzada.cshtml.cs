using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AutoClick.Pages
{
    public class DropdownOptions
    {
        public List<string> Provincias { get; set; } = new();
        public List<string> Cantones { get; set; } = new();
        public List<string> Marcas { get; set; } = new();
        public List<string> Modelos { get; set; } = new();
        public List<string> TiposCarroceria { get; set; } = new();
        public List<string> TiposCombustible { get; set; } = new();
        public List<string> Transmisiones { get; set; } = new();
        public List<string> Condiciones { get; set; } = new();
        public List<int> Anos { get; set; } = new();
    }

    public class BusquedaAvanzadaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public BusquedaAvanzadaModel(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IList<Auto> Autos { get; set; } = new List<Auto>();
        
        // Filter Parameters
        [BindProperty(SupportsGet = true)]
        public string? Province { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Canton { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Brand { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Model { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MinPrice { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MaxPrice { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MinKm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MaxKm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MinYear { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MaxYear { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? BodyType { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? FuelType { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Transmission { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Condition { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; } = "recent";
        
        [BindProperty(SupportsGet = true)]
        public new int Page { get; set; } = 1;
        
        public int PageSize { get; set; } = 8; // 2 columns x 4 rows
        public int TotalPages { get; set; }
        public int CurrentPage => Page;
        public int TotalCars { get; set; }

        // Applied Filters for Display
        public List<string> AppliedFilters { get; set; } = new List<string>();

        // Dropdown Options
        public List<string> AvailableProvinces { get; set; } = new List<string>();
        public List<string> AvailableCantons { get; set; } = new List<string>();
        public List<string> AvailableBrands { get; set; } = new List<string>();
        public List<string> AvailableModels { get; set; } = new List<string>();
        public List<string> AvailableBodyTypes { get; set; } = new List<string>();
        public List<string> AvailableFuelTypes { get; set; } = new List<string>();
        public List<string> AvailableTransmissions { get; set; } = new List<string>();
        public List<string> AvailableConditions { get; set; } = new List<string>();
        public List<int> AvailableYears { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {
            // Load dropdown options (con caché optimizado)
            await LoadDropdownOptions();

            // Crear consulta base optimizada con AsNoTracking desde el inicio
            var query = _context.Autos
                .AsNoTracking()
                .Where(a => a.Activo); // Solo autos activos

            // Build applied filters list for UI
            BuildAppliedFiltersList();

            // Apply filters de forma optimizada
            if (!string.IsNullOrEmpty(Search))
                query = query.Where(a => a.Marca.Contains(Search) || a.Modelo.Contains(Search));
                
            if (!string.IsNullOrEmpty(Province))
                query = query.Where(a => a.Provincia == Province); // Usar == en lugar de Contains para mejor performance
                
            if (!string.IsNullOrEmpty(Canton))
                query = query.Where(a => a.Canton == Canton); // Usar == en lugar de Contains
                
            if (!string.IsNullOrEmpty(Brand))
                query = query.Where(a => a.Marca == Brand);
                
            if (!string.IsNullOrEmpty(Model))
                query = query.Where(a => a.Modelo == Model);
                
            if (MinPrice.HasValue)
                query = query.Where(a => a.ValorFiscal >= MinPrice.Value);
                
            if (MaxPrice.HasValue)
                query = query.Where(a => a.ValorFiscal <= MaxPrice.Value);
                
            if (MinYear.HasValue)
                query = query.Where(a => a.Ano >= MinYear.Value);
                
            if (MaxYear.HasValue)
                query = query.Where(a => a.Ano <= MaxYear.Value);
                
            if (!string.IsNullOrEmpty(BodyType))
                query = query.Where(a => a.Carroceria == BodyType);
                
            if (!string.IsNullOrEmpty(FuelType))
                query = query.Where(a => a.Combustible == FuelType);
                
            if (!string.IsNullOrEmpty(Transmission))
                query = query.Where(a => a.Transmision == Transmission);
                
            if (!string.IsNullOrEmpty(Condition))
                query = query.Where(a => a.Condicion == Condition);
                
            if (MinKm.HasValue)
                query = query.Where(a => a.Kilometraje >= MinKm.Value);
                
            if (MaxKm.HasValue)
                query = query.Where(a => a.Kilometraje <= MaxKm.Value);

            // Apply sorting
            query = SortBy switch
            {
                "price-asc" => query.OrderBy(a => a.ValorFiscal),
                "price-desc" => query.OrderByDescending(a => a.ValorFiscal),
                "year" => query.OrderByDescending(a => a.Ano),
                "brand" => query.OrderBy(a => a.Marca).ThenBy(a => a.Modelo),
                _ => query.OrderByDescending(a => a.Id) // Default: most recent
            };

            // Count total cars for pagination (con AsNoTracking para performance)
            TotalCars = await query.AsNoTracking().CountAsync();
            TotalPages = (int)Math.Ceiling((double)TotalCars / PageSize);

            // Apply pagination (con AsNoTracking para performance)
            Autos = await query
                .AsNoTracking()
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // If no autos in database, use sample data
            if (!Autos.Any())
            {
                var sampleAutos = GetSampleSearchResults();
                
                // Apply filters to sample data
                sampleAutos = ApplyFiltersToSampleData(sampleAutos);
                
                TotalCars = sampleAutos.Count;
                TotalPages = (int)Math.Ceiling((double)TotalCars / PageSize);
                
                Autos = sampleAutos
                    .Skip((Page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
        }

        private void BuildAppliedFiltersList()
        {
            AppliedFilters.Clear();
            
            if (!string.IsNullOrEmpty(Province))
                AppliedFilters.Add(Province);
                
            if (!string.IsNullOrEmpty(Canton))
                AppliedFilters.Add(Canton);
                
            if (!string.IsNullOrEmpty(Brand))
                AppliedFilters.Add(Brand);
                
            if (!string.IsNullOrEmpty(Model))
                AppliedFilters.Add(Model);
                
            if (!string.IsNullOrEmpty(BodyType))
                AppliedFilters.Add(BodyType);
                
            if (!string.IsNullOrEmpty(FuelType))
                AppliedFilters.Add(FuelType);
                
            if (!string.IsNullOrEmpty(Transmission))
                AppliedFilters.Add(Transmission);
                
            if (!string.IsNullOrEmpty(Condition))
                AppliedFilters.Add(Condition);
                
            if (MinPrice.HasValue || MaxPrice.HasValue)
            {
                var priceRange = "";
                if (MinPrice.HasValue) priceRange += $"₡{MinPrice:N0}";
                if (MinPrice.HasValue && MaxPrice.HasValue) priceRange += " - ";
                if (MaxPrice.HasValue) priceRange += $"₡{MaxPrice:N0}";
                if (!string.IsNullOrEmpty(priceRange))
                    AppliedFilters.Add(priceRange);
            }
            
            if (MinKm.HasValue || MaxKm.HasValue)
            {
                var kmRange = "";
                if (MinKm.HasValue) kmRange += $"{MinKm:N0} km";
                if (MinKm.HasValue && MaxKm.HasValue) kmRange += " - ";
                if (MaxKm.HasValue) kmRange += $"{MaxKm:N0} km";
                if (!string.IsNullOrEmpty(kmRange))
                    AppliedFilters.Add(kmRange);
            }
            
            if (MinYear.HasValue || MaxYear.HasValue)
            {
                var yearRange = "";
                if (MinYear.HasValue) yearRange += $"{MinYear}";
                if (MinYear.HasValue && MaxYear.HasValue) yearRange += " - ";
                if (MaxYear.HasValue) yearRange += $"{MaxYear}";
                if (!string.IsNullOrEmpty(yearRange))
                    AppliedFilters.Add(yearRange);
            }
        }

        private List<Auto> ApplyFiltersToSampleData(List<Auto> autos)
        {
            if (!string.IsNullOrEmpty(Brand))
                autos = autos.Where(a => a.Marca == Brand).ToList();
                
            if (!string.IsNullOrEmpty(Model))
                autos = autos.Where(a => a.Modelo == Model).ToList();
                
            if (MinPrice.HasValue)
                autos = autos.Where(a => a.ValorFiscal >= MinPrice.Value).ToList();
                
            if (MaxPrice.HasValue)
                autos = autos.Where(a => a.ValorFiscal <= MaxPrice.Value).ToList();
                
            if (MinYear.HasValue)
                autos = autos.Where(a => a.Ano >= MinYear.Value).ToList();
                
            if (MaxYear.HasValue)
                autos = autos.Where(a => a.Ano <= MaxYear.Value).ToList();
                
            if (!string.IsNullOrEmpty(BodyType))
                autos = autos.Where(a => a.Carroceria == BodyType).ToList();
                
            if (!string.IsNullOrEmpty(FuelType))
                autos = autos.Where(a => a.Combustible == FuelType).ToList();
                
            if (!string.IsNullOrEmpty(Transmission))
                autos = autos.Where(a => a.Transmision == Transmission).ToList();
                
            if (!string.IsNullOrEmpty(Condition))
                autos = autos.Where(a => a.Condicion == Condition).ToList();
                
            if (MinKm.HasValue)
                autos = autos.Where(a => a.Kilometraje >= MinKm.Value).ToList();
                
            if (MaxKm.HasValue)
                autos = autos.Where(a => a.Kilometraje <= MaxKm.Value).ToList();

            // Apply sorting
            autos = SortBy switch
            {
                "price-asc" => autos.OrderBy(a => a.ValorFiscal).ToList(),
                "price-desc" => autos.OrderByDescending(a => a.ValorFiscal).ToList(),
                "year" => autos.OrderByDescending(a => a.Ano).ToList(),
                "brand" => autos.OrderBy(a => a.Marca).ThenBy(a => a.Modelo).ToList(),
                _ => autos.OrderByDescending(a => a.Id).ToList()
            };

            return autos;
        }

        private async Task LoadDropdownOptions()
        {
            // Intentar obtener desde caché primero
            var cacheKey = "busqueda-dropdown-options";
            if (!_cache.TryGetValue(cacheKey, out DropdownOptions? cachedOptions))
            {
                try
                {
                    // Load from database if available
                    var hasData = await _context.Autos.AsNoTracking().AnyAsync();
                    
                    if (hasData)
                    {
                        // Hacer UNA sola consulta optimizada para obtener todos los datos necesarios
                        var autosData = await _context.Autos
                            .AsNoTracking()
                            .Where(a => a.Activo) // Solo autos activos
                            .Select(a => new {
                                a.Provincia,
                                a.Canton,
                                a.Marca,
                                a.Modelo,
                                a.Carroceria,
                                a.Combustible,
                                a.Transmision,
                                a.Condicion,
                                a.Ano
                            })
                            .ToListAsync();

                        // Procesar en memoria para evitar múltiples consultas a BD
                        cachedOptions = new DropdownOptions
                        {
                            Provincias = autosData
                                .Where(a => !string.IsNullOrEmpty(a.Provincia))
                                .Select(a => a.Provincia)
                                .Distinct()
                                .OrderBy(p => p)
                                .ToList(),
                            Cantones = autosData
                                .Where(a => !string.IsNullOrEmpty(a.Canton))
                                .Select(a => a.Canton)
                                .Distinct()
                                .OrderBy(c => c)
                                .ToList(),
                            Marcas = autosData
                                .Where(a => !string.IsNullOrEmpty(a.Marca))
                                .Select(a => a.Marca)
                                .Distinct()
                                .OrderBy(b => b)
                                .ToList(),
                            Modelos = autosData
                                .Where(a => !string.IsNullOrEmpty(a.Modelo))
                                .Select(a => a.Modelo)
                                .Distinct()
                                .OrderBy(m => m)
                                .ToList(),
                            TiposCarroceria = autosData
                                .Where(a => !string.IsNullOrEmpty(a.Carroceria))
                                .Select(a => a.Carroceria)
                                .Distinct()
                                .OrderBy(bt => bt)
                                .ToList(),
                            TiposCombustible = autosData
                                .Where(a => !string.IsNullOrEmpty(a.Combustible))
                                .Select(a => a.Combustible)
                                .Distinct()
                                .OrderBy(ft => ft)
                                .ToList(),
                            Transmisiones = autosData
                                .Where(a => !string.IsNullOrEmpty(a.Transmision))
                                .Select(a => a.Transmision)
                                .Distinct()
                                .OrderBy(t => t)
                                .ToList(),
                            Condiciones = autosData
                                .Where(a => !string.IsNullOrEmpty(a.Condicion))
                                .Select(a => a.Condicion)
                                .Distinct()
                                .OrderBy(c => c)
                                .ToList(),
                            Anos = autosData
                                .Where(a => a.Ano > 0)
                                .Select(a => a.Ano)
                                .Distinct()
                                .OrderByDescending(y => y)
                                .ToList()
                        };

                        // Guardar en caché por 15 minutos
                        _cache.Set(cacheKey, cachedOptions, TimeSpan.FromMinutes(15));
                    }
                    else
                    {
                        // Use sample data options
                        LoadSampleDropdownOptions();
                        return;
                    }
                }
                catch
                {
                    // Fallback to sample data
                    LoadSampleDropdownOptions();
                    return;
                }
            }

            // Usar datos del caché si están disponibles
            if (cachedOptions != null)
            {
                AvailableProvinces = cachedOptions.Provincias;
                AvailableBrands = cachedOptions.Marcas;
                AvailableBodyTypes = cachedOptions.TiposCarroceria;
                AvailableFuelTypes = cachedOptions.TiposCombustible;
                AvailableTransmissions = cachedOptions.Transmisiones;
                AvailableConditions = cachedOptions.Condiciones;
                AvailableYears = cachedOptions.Anos;

                // Filtrar cantones basados en provincia seleccionada
                AvailableCantons = string.IsNullOrEmpty(Province) 
                    ? cachedOptions.Cantones 
                    : cachedOptions.Cantones; // TODO: Implementar filtro provincia-cantón

                // Filtrar modelos basados en marca seleccionada  
                AvailableModels = string.IsNullOrEmpty(Brand) 
                    ? cachedOptions.Modelos 
                    : cachedOptions.Modelos; // TODO: Implementar filtro marca-modelo
            }
        }

        private void LoadSampleDropdownOptions()
        {
            AvailableProvinces = new List<string>
            {
                "San José", "Alajuela", "Cartago", "Heredia", "Guanacaste", "Puntarenas", "Limón"
            };

            AvailableCantons = new List<string>();
            if (string.IsNullOrEmpty(Province) || Province == "San José")
                AvailableCantons.AddRange(new[] { "San José", "Escazú", "Desamparados", "Puriscal", "Tarrazú", "Aserrí", "Mora", "Goicoechea", "Santa Ana", "Alajuelita", "Vázquez de Coronado", "Acosta", "Tibás", "Moravia", "Montes de Oca", "Turrubares", "Dota", "Curridabat" });
            
            if (string.IsNullOrEmpty(Province) || Province == "Alajuela")
                AvailableCantons.AddRange(new[] { "Alajuela", "San Ramón", "Grecia", "San Mateo", "Atenas", "Naranjo", "Palmares", "Poás", "Orotina", "San Carlos", "Zarcero", "Valverde Vega", "Upala", "Los Chiles", "Guatuso" });

            if (string.IsNullOrEmpty(Province))
                AvailableCantons = AvailableCantons.Distinct().OrderBy(c => c).ToList();

            AvailableBrands = new List<string>
            {
                "Audi", "BMW", "Mercedes-Benz", "Toyota", "Honda", "Nissan", "Hyundai", "Kia", 
                "Chevrolet", "Ford", "Volkswagen", "Mazda", "Subaru", "Mitsubishi", "Peugeot", 
                "Renault", "Jeep", "Land Rover", "Volvo", "Acura", "Infiniti", "Lexus"
            };

            AvailableModels = new List<string>();
            if (string.IsNullOrEmpty(Brand))
            {
                AvailableModels.AddRange(new[] { "A3", "A4", "A6", "Q3", "Q5", "Q7", "X1", "X3", "X5", "Serie 3", "Serie 5", "C-Class", "E-Class", "GLC", "GLE", "Corolla", "Camry", "RAV4", "Highlander", "Civic", "Accord", "CR-V", "Pilot" });
            }
            else if (Brand == "BMW")
            {
                AvailableModels.AddRange(new[] { "X1", "X3", "X5", "X6", "X7", "Serie 1", "Serie 2", "Serie 3", "Serie 4", "Serie 5", "Serie 6", "Serie 7", "Z4", "i3", "i8" });
            }
            else if (Brand == "Audi")
            {
                AvailableModels.AddRange(new[] { "A1", "A3", "A4", "A5", "A6", "A7", "A8", "Q2", "Q3", "Q5", "Q7", "Q8", "TT", "R8", "e-tron" });
            }
            else if (Brand == "Mercedes-Benz")
            {
                AvailableModels.AddRange(new[] { "A-Class", "B-Class", "C-Class", "CLA", "CLS", "E-Class", "S-Class", "GLA", "GLB", "GLC", "GLE", "GLS", "G-Class", "SLK", "SL" });
            }
            else if (Brand == "Toyota")
            {
                AvailableModels.AddRange(new[] { "Corolla", "Camry", "Avalon", "Prius", "RAV4", "Highlander", "4Runner", "Sequoia", "Sienna", "Tacoma", "Tundra", "Land Cruiser" });
            }

            AvailableBodyTypes = new List<string>
            {
                "Sedán", "Hatchback", "SUV", "Pickup", "Coupé", "Convertible", "Station Wagon", "Van", "Crossover"
            };

            AvailableFuelTypes = new List<string>
            {
                "Gasolina", "Diésel", "Híbrido", "Eléctrico", "GLP", "Gas Natural"
            };

            AvailableTransmissions = new List<string>
            {
                "Manual", "Automática", "CVT", "Semi-automática"
            };

            AvailableConditions = new List<string>
            {
                "Excelente", "Muy Buena", "Buena", "Regular", "Necesita Reparación"
            };

            AvailableYears = Enumerable.Range(1990, DateTime.Now.Year - 1989).OrderByDescending(y => y).ToList();
        }

        private List<Auto> GetSampleSearchResults()
        {
            return new List<Auto>
            {
                new Auto
                {
                    Id = 1,
                    Marca = "BMW",
                    Modelo = "X5M",
                    Ano = 2025,
                    ValorFiscal = 170000,
                    ImagenPrincipal = "https://placehold.co/415x262/333333/FFFFFF?text=BMW+X5M",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "San José",
                    Canton = "Escazú",
                    PlacaVehiculo = "BMW001",
                    Condicion = "Excelente",
                    EmailPropietario = "bmw@dealer.com"
                },
                new Auto
                {
                    Id = 2,
                    Marca = "Audi",
                    Modelo = "Q5 Sportback",
                    Ano = 2022,
                    ValorFiscal = 75000,
                    ImagenPrincipal = "https://placehold.co/415x262/8B0000/FFFFFF?text=Audi+Q5",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "Alajuela",
                    Canton = "Alajuela",
                    PlacaVehiculo = "AUD002",
                    Condicion = "Muy Buena",
                    EmailPropietario = "audi@dealer.com"
                },
                new Auto
                {
                    Id = 3,
                    Marca = "Mercedes-Benz",
                    Modelo = "GLE 450",
                    Ano = 2023,
                    ValorFiscal = 85000,
                    ImagenPrincipal = "https://placehold.co/415x262/000000/FFFFFF?text=Mercedes+GLE",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "San José",
                    Canton = "Santa Ana",
                    PlacaVehiculo = "MER003",
                    Condicion = "Excelente",
                    EmailPropietario = "mercedes@dealer.com"
                }
            };
        }
    }
}