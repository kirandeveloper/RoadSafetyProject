using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RoadSafetyProject.Data;

namespace RoadSafetyProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RspMasterRepository _repo;

        public IndexModel(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("OracleDb");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'OracleDb' was not found.");

            _repo = new RspMasterRepository(connectionString);
        }

        public void OnGet()
        {
        }

        public JsonResult OnGetList(string division = "", string status = "")
        {
            var items = _repo.GetAll();

            if (!string.IsNullOrWhiteSpace(division))
            {
                items = items
                    .Where(x => !string.IsNullOrWhiteSpace(x.Division) &&
                                x.Division.Equals(division, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                items = items
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.GetType().GetProperty("LcStatus") != null
                                ? (string)x.GetType().GetProperty("LcStatus")!.GetValue(x)
                                : x.GetType().GetProperty("LC_STATUS") != null
                                    ? (string)x.GetType().GetProperty("LC_STATUS")!.GetValue(x)
                                    : x.GetType().GetProperty("Status") != null
                                        ? (string)x.GetType().GetProperty("Status")!.GetValue(x)
                                        : null)
                    )
                    .Where(x =>
                    {
                        var p = x.GetType().GetProperty("LcStatus")
                             ?? x.GetType().GetProperty("LC_STATUS")
                             ?? x.GetType().GetProperty("Status");

                        var value = p?.GetValue(x)?.ToString();

                        return string.Equals(value, status, StringComparison.OrdinalIgnoreCase);
                    })
                    .ToList();
            }

            return new JsonResult(new
            {
                success = true,
                data = items
            });
        }

        public JsonResult OnGetDivisionCounts()
        {
            var data = _repo.GetDivisionCounts();
            return new JsonResult(data);
        }
    }
}