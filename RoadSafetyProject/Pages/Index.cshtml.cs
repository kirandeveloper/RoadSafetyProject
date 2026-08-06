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

        // GET ?handler=DivisionCounts -> Sanctioned / UnSanctioned record counts per
        // division, computed from the database (via _repo.GetAll()) for the "Summery
        // Division Wise LC" cards at the top of the page. Matches each record's free-text
        // Division value against either the short code (e.g. "BSL") or the full name
        // (e.g. "Bhusawal") so it works regardless of which convention was used when
        // the record was saved.
        private static readonly (string Key, string Code, string Name)[] Divisions = new[]
        {
            ("Mumbai", "BB",  "Mumbai"),
            ("BSL",    "BSL", "Bhusawal"),
            ("NGP",    "NGP", "Nagpur"),
            ("Pune",   "PA",  "Pune"),
            ("SUR",    "SUR", "Solapur"),
        };

        private static bool MatchesDivision(string division, string code, string name)
        {
            if (string.IsNullOrWhiteSpace(division)) return false;
            var v = division.Trim();
            return v.Equals(code, StringComparison.OrdinalIgnoreCase)
                || v.Equals(name, StringComparison.OrdinalIgnoreCase)
                || v.Contains(name, StringComparison.OrdinalIgnoreCase)
                || v.Contains(code, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsUnSanction(string lcStatus) =>
            !string.IsNullOrWhiteSpace(lcStatus) &&
            lcStatus.Trim().Equals("UnSanction", StringComparison.OrdinalIgnoreCase);

        public JsonResult OnGetDivisionCounts()
        {
            var items = _repo.GetAll();

            var data = Divisions.Select(d => new
            {
                key = d.Key,
                sanctioned = items.Count(x => MatchesDivision(x.Division, d.Code, d.Name) && !IsUnSanction(x.LcStatus)),
                unSanctioned = items.Count(x => MatchesDivision(x.Division, d.Code, d.Name) && IsUnSanction(x.LcStatus))
            });

            return new JsonResult(new { success = true, data });
        }

        // GET ?handler=Item&id=5  -> single record (kept here in case the "Edit" button
        // on the Saved LC Records grid is later wired to pre-fill a form on this page).
        public JsonResult OnGetItem(int id)
        {
            var item = _repo.GetById(id);
            if (item == null)
                return new JsonResult(new { success = false, message = "Record not found." }) { StatusCode = 404 };

            return new JsonResult(new { success = true, data = item });
        }

        // POST ?handler=Delete&id=5  -> used by the "Delete" button on the
        // Saved LC Records grid merged in from the Add New LC page.
        [ValidateAntiForgeryToken]
        public JsonResult OnPostDelete(int id)
        {
            try
            {
                var deleted = _repo.Delete(id);
                return new JsonResult(new { success = deleted, message = deleted ? "Record deleted." : "Record not found." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Delete failed: " + ex.Message }) { StatusCode = 500 };
            }
        }
    }
}