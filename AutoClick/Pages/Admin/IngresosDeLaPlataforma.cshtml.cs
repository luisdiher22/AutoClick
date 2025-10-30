using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using System.Globalization;

namespace AutoClick.Pages.Admin
{
    public class IngresosDeLaPlataformaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IngresosDeLaPlataformaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Precios de planes (PlanVisibilidad: 0 = Gratis, 1-5 = Pagados)
        private static readonly Dictionary<int, decimal> PreciosPlanes = new Dictionary<int, decimal>
        {
            { 0, 0 },      // Gratis / Pendiente
            { 1, 2500 },   // Plan Básico
            { 2, 9500 },   // Plan Estándar
            { 3, 12900 },  // Plan Premium
            { 4, 47500 },  // Plan Elite
            { 5, 0 }       // Plan Platinum (no definido)
        };

        // Precios de banderines (BanderinAdquirido: 0 = Sin banderín, 1-10 = Con banderín)
        private const decimal PrecioBanderin = 2950;

        public List<IngresoMensual> IngresosMensuales { get; set; } = new List<IngresoMensual>();
        public decimal TotalIngresos { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Verificar si el usuario es administrador
            var isAdminClaim = User.FindFirst("IsAdmin");
            if (isAdminClaim?.Value != "true")
            {
                return RedirectToPage("/Index");
            }

            await CalcularIngresosMensualesAsync();

            return Page();
        }

        private async Task CalcularIngresosMensualesAsync()
        {
            // Obtener todos los autos con planes y banderines
            var autos = await _context.Autos
                .Where(a => a.PlanVisibilidad > 0 || a.BanderinAdquirido > 0)
                .ToListAsync();

            // Obtener empresas de publicidad activas
            var empresasPublicidad = await _context.EmpresasPublicidad
                .Where(e => e.Activa)
                .ToListAsync();

            // Calcular ingresos para los últimos 12 meses
            var fechaActual = DateTime.Now;
            IngresosMensuales = new List<IngresoMensual>();

            for (int i = 0; i < 12; i++)
            {
                var mesAnalisis = fechaActual.AddMonths(-i);
                var inicioDelMes = new DateTime(mesAnalisis.Year, mesAnalisis.Month, 1);
                var finDelMes = inicioDelMes.AddMonths(1).AddDays(-1);

                // Calcular ingresos por planes (autos creados en ese mes con plan pagado)
                var ingresosPlanes = autos
                    .Where(a => a.FechaCreacion.Year == mesAnalisis.Year && 
                               a.FechaCreacion.Month == mesAnalisis.Month &&
                               a.PlanVisibilidad > 0)
                    .Sum(a => PreciosPlanes.ContainsKey(a.PlanVisibilidad) ? PreciosPlanes[a.PlanVisibilidad] : 0);

                // Calcular ingresos por banderines (autos creados en ese mes con banderín)
                var ingresosBanderines = autos
                    .Where(a => a.FechaCreacion.Year == mesAnalisis.Year && 
                               a.FechaCreacion.Month == mesAnalisis.Month &&
                               a.BanderinAdquirido > 0)
                    .Count() * PrecioBanderin;

                // Calcular ingresos por publicidad (empresas que iniciaron en ese mes)
                // Asumimos un precio mensual de publicidad de 50,000 CRC
                var ingresosPublicidad = empresasPublicidad
                    .Where(e => e.FechaInicio.Year == mesAnalisis.Year && 
                               e.FechaInicio.Month == mesAnalisis.Month)
                    .Count() * 50000;

                var ingresoMensual = new IngresoMensual
                {
                    Mes = mesAnalisis.ToString("MMMM", new CultureInfo("es-ES")),
                    Ano = mesAnalisis.Year,
                    IngresosBanderines = ingresosBanderines,
                    IngresosPlanes = ingresosPlanes,
                    IngresosPublicidad = ingresosPublicidad,
                    TotalIngresos = ingresosBanderines + ingresosPlanes // Solo banderines + planes
                };

                IngresosMensuales.Add(ingresoMensual);
            }

            TotalIngresos = IngresosMensuales.Sum(i => i.TotalIngresos);
        }
    }

    public class IngresoMensual
    {
        public string Mes { get; set; } = string.Empty;
        public int Ano { get; set; }
        public decimal IngresosBanderines { get; set; }
        public decimal IngresosPlanes { get; set; }
        public decimal IngresosPublicidad { get; set; }
        public decimal TotalIngresos { get; set; }

        public string MesCapitalizado => 
            Mes.Length > 0 ? char.ToUpper(Mes[0]) + Mes.Substring(1) : Mes;
        
        public string IngresosBanderinesFormateado => $"₡{IngresosBanderines:N0}";
        public string IngresosPlanesFormateado => $"₡{IngresosPlanes:N0}";
        public string IngresosPublicidadFormateado => $"₡{IngresosPublicidad:N0}";
        public string TotalIngresosFormateado => $"₡{TotalIngresos:N0}";
    }
}