using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RoadSafetyProject.Data;

namespace RoadSafetyProject.Pages
{
    public class viewalllcModel : PageModel
    {
        private readonly RspMasterRepository _repo;

        public viewalllcModel(IConfiguration config)
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

        // GET ?handler=List -> every RSP_MASTER row, for the DataTable to page/search/export
        // client-side. All records are returned in one shot (no server paging) since the
        // export buttons (Excel/PDF/Print) need the complete data set available in the browser.
        public JsonResult OnGetList()
        {
            try
            {
                var items = _repo.GetAll();
                return new JsonResult(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Failed to load records: " + ex.Message }) { StatusCode = 500 };
            }
        }
    }
}
