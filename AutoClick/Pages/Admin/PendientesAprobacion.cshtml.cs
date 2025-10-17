using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace AutoClick.Pages.Admin
{
    public class PendientesAprobacionModel : PageModel
    {
        public List<PendingApprovalItem> PendingApprovals { get; set; }

        public void OnGet()
        {
            // TODO: Replace with actual database query
            PendingApprovals = new List<PendingApprovalItem>
            {
                new PendingApprovalItem { Code = "#20796", Title = "Lancia Delta Integrale ex Works, Unique", Date = "7/27/13" },
                new PendingApprovalItem { Code = "#20796", Title = "Lotus 51 FF1600", Date = "5/27/15" },
                new PendingApprovalItem { Code = "#20796", Title = "AIM Mychron3 XG LOG", Date = "7/18/17" },
                new PendingApprovalItem { Code = "#20796", Title = "Mitsubishi Lancer Evo Berg Monster", Date = "1/31/14" },
                new PendingApprovalItem { Code = "#20796", Title = "2009 Aston Martin Vantage GT4", Date = "9/4/12" },
                new PendingApprovalItem { Code = "#20796", Title = "Porsche 2012 GT3 Cup ", Date = "4/4/18" },
                new PendingApprovalItem { Code = "#20796", Title = "1957 Hillman Minx Touring Saloon", Date = "6/19/14" },
                new PendingApprovalItem { Code = "#20796", Title = "V8 PROTYP", Date = "8/16/13" },
                new PendingApprovalItem { Code = "#20796", Title = "1959 Jaguar MK1 FIA", Date = "7/11/19" },
                new PendingApprovalItem { Code = "#20796", Title = "Reynard FF89 FF1600", Date = "8/2/19" },
                new PendingApprovalItem { Code = "#20796", Title = "1965 Ford Lotus Cortina MKI", Date = "9/23/16" },
                new PendingApprovalItem { Code = "#20796", Title = "Aston Martin Vantage V12 GT3", Date = "5/27/15" }
            };
        }

        public IActionResult OnPostApprove(string code)
        {
            // TODO: Implement approval logic
            return RedirectToPage();
        }

        public IActionResult OnPostReject(string code)
        {
            // TODO: Implement rejection logic
            return RedirectToPage();
        }
    }

    public class PendingApprovalItem
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
    }
}
