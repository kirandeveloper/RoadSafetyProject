using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RoadSafetyProject.Data;

namespace RoadSafetyProject.Pages
{
    public class CompletedLCModel : PageModel
    {
        private readonly RspMasterRepository _repo;

        public CompletedLCModel(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("OracleDb");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'OracleDb' was not found in appsettings.json. " +
                    "Check the 'ConnectionStrings' section and the key name.");

            _repo = new RspMasterRepository(connectionString);
        }

        // Renders the page shell. The table itself is populated client-side via AJAX.
        public void OnGet()
        {
        }

        // GET ?handler=List -> only records marked Status = "Completed" (via the
        // "Complete LC" button on the viewalllc page), for the DataTable to
        // page/search/export client-side.
        public JsonResult OnGetList()
        {
            try
            {
                var items = _repo.GetAll()
                    .Where(x => !string.IsNullOrWhiteSpace(x.Status) &&
                                x.Status.Trim().Equals("Completed", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return new JsonResult(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Failed to load records: " + ex.Message }) { StatusCode = 500 };
            }
        }
    }
}
