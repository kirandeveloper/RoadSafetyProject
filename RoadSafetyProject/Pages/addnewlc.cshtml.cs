using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RoadSafetyProject.Data;
using RoadSafetyProject.Models;
using System.Linq;

namespace RoadSafetyProject.Pages
{
    public class addnewlcModel : PageModel
    {
        private readonly RspMasterRepository _repo;
        private readonly IWebHostEnvironment _env;

        public addnewlcModel(IConfiguration config, IWebHostEnvironment env)
        {
            var connectionString = config.GetConnectionString("OracleDb");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'OracleDb' was not found in appsettings.json. " +
                    "Check the 'ConnectionStrings' section and the key name.");

            _repo = new RspMasterRepository(connectionString);
            _env = env;
        }

        // Renders the page. The form/table are populated client-side via AJAX.
        public void OnGet()
        {
        }

        // GET ?handler=List  -> table data for the "view" grid
        public JsonResult OnGetList(string division = "", string status = "")
        {
            var items = _repo.GetAll();

            if (!string.IsNullOrWhiteSpace(division))
            {
                items = items
                    .Where(x => x.Division == division)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                items = items
                    .Where(x => x.LcStatus == status)
                    .ToList();
            }

            return new JsonResult(new
            {
                success = true,
                data = items
            });
        }

        // GET ?handler=Item&id=5  -> single record, used to populate the form for Edit
        public JsonResult OnGetItem(int id)
        {
            var item = _repo.GetById(id);
            if (item == null)
                return new JsonResult(new { success = false, message = "Record not found." }) { StatusCode = 404 };

            return new JsonResult(new { success = true, data = item });
        }

        // POST ?handler=Save  -> inserts when Id == 0, updates otherwise
        [ValidateAntiForgeryToken]
        public JsonResult OnPostSave([FromBody] RspMasterDto dto)
        {
            if (dto == null)
                return new JsonResult(new { success = false, message = "No data received." }) { StatusCode = 400 };

            try
            {
                var entity = dto.ToEntity();
                var id = _repo.Save(entity);
                return new JsonResult(new { success = true, id, message = entity.Id > 0 ? "Record updated." : "Record saved." });
            }
            catch (Exception ex)
            {
                // TEMP DIAGNOSTIC — reveals the exact line/field behind the FormatException.
                // Revert this to "Save failed: " + ex.Message before deploying; ex.ToString()
                // can include internal details you don't want a client to see in production.
                return new JsonResult(new { success = false, message = "Save failed: " + ex.ToString() }) { StatusCode = 500 };
            }
        }

        // POST ?handler=Delete&id=5
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


        // POST ?handler=UploadPdf&id=5
        // Saves the uploaded LC PDF to wwwroot/pdf/LC as LC-<LC_NO>.pdf.
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostUploadPdf(int id, IFormFile pdfFile)
        {
            if (id <= 0)
                return new JsonResult(new { success = false, message = "Invalid LC record." }) { StatusCode = 400 };

            if (pdfFile == null || pdfFile.Length == 0)
                return new JsonResult(new { success = false, message = "Please select a PDF file." }) { StatusCode = 400 };

            if (!string.Equals(Path.GetExtension(pdfFile.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
                return new JsonResult(new { success = false, message = "Only PDF files are allowed." }) { StatusCode = 400 };

            const long maxBytes = 20L * 1024L * 1024L;
            if (pdfFile.Length > maxBytes)
                return new JsonResult(new { success = false, message = "PDF size must not exceed 20 MB." }) { StatusCode = 400 };

            var item = _repo.GetById(id);
            if (item == null)
                return new JsonResult(new { success = false, message = "LC record not found." }) { StatusCode = 404 };

            if (string.IsNullOrWhiteSpace(item.LcNo))
                return new JsonResult(new { success = false, message = "LC No. is required before uploading the PDF." }) { StatusCode = 400 };

            var webRoot = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");

            var folder = Path.Combine(webRoot, "pdf", "LC");
            Directory.CreateDirectory(folder);

            var baseLc = item.LcNo.Trim();
            if (!baseLc.StartsWith("LC-", StringComparison.OrdinalIgnoreCase))
                baseLc = "LC-" + baseLc;

            var safeName = Regex.Replace(baseLc, @"[^A-Za-z0-9_-]", "_") + ".pdf";
            var fullPath = Path.Combine(folder, safeName);

            await using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await pdfFile.CopyToAsync(stream);
            }

            _repo.UpdatePdfFileName(id, safeName);

            return new JsonResult(new
            {
                success = true,
                fileName = safeName,
                url = "/pdf/LC/" + Uri.EscapeDataString(safeName),
                message = "LC PDF uploaded successfully."
            });
        }

        public JsonResult OnGetDivisionCounts()
        {
            var data = _repo.GetDivisionCounts();

            return new JsonResult(data);
        }

    }

    /// <summary>
    /// Flat DTO that mirrors the form field names exactly (camelCase, as posted from JS),
    /// so JSON binding needs no extra attributes. Converts to/from the RSP_MASTER entity.
    /// Date-typed DB columns are received as free text from the textareas and parsed here;
    /// unparsable text is stored as NULL for that date column.
    /// </summary>
    public class RspMasterDto
    {
        public int id { get; set; }
        public string lcStatus { get; set; }
        public int? srNo { get; set; }
        public string projectName { get; set; }
        public string nameOfWorks { get; set; }
        public string roadCategory { get; set; }
        public string robRubWork { get; set; }
        public string statusOfWorks { get; set; }
        public string yearSanction { get; set; }
        public string lcNo { get; set; }
        public string locationKm { get; set; }
        public string division { get; set; }
        public string section { get; set; }
        public string spanArrangement { get; set; }
        public string skewAngle { get; set; }
        public string distanceLc { get; set; }
        public string stateApproval { get; set; }
        public string dprConsultancy { get; set; }
        public string executiveAgency { get; set; }
        public string gad { get; set; }
        public string gadRemark { get; set; }
        public string checkedReceived { get; set; }
        public string checkedReceivedRemark { get; set; }
        public string sanctionedCont { get; set; }
        public string sanctionedDe { get; set; }
        public string tenderStatus { get; set; }
        public string sec7a { get; set; }
        public string sec20aGazette { get; set; }
        public string sec20aPaper { get; set; }
        public string sec20b { get; set; }
        public string sec20c { get; set; }
        public string sec20d { get; set; }
        public string sec20eGazette { get; set; }
        public string sec20ePaper { get; set; }
        public string sec20f { get; set; }
        public string landRemark { get; set; }
        public string utilSnt { get; set; }
        public string utilTrd { get; set; }
        public string utilElectrical { get; set; }
        public string substructure { get; set; }
        public string superstructure { get; set; }
        public string commissioning { get; set; }
        public string exitsGad { get; set; }
        public string exitsGadRemark { get; set; }
        public string designField { get; set; }
        public string exitNo { get; set; }
        public string noExit { get; set; }
        public string nocClosingLc { get; set; }
        public string soa { get; set; }

        public RspMaster ToEntity()
        {
            return new RspMaster
            {
                Id = id,
                LcStatus = string.IsNullOrWhiteSpace(lcStatus) ? "Sanction" : lcStatus,
                SrNo = srNo,
                ProjectName = projectName,
                NameOfWorks = nameOfWorks,
                RoadCategory = roadCategory,
                RobRubWork = robRubWork,
                StatusOfWorks = statusOfWorks,
                YearOfSanction = yearSanction,
                LcNo = lcNo,
                LocationKm = locationKm,
                Division = division,
                SectionName = section,
                SpanArrangement = spanArrangement,
                SkewAngle = skewAngle,
                DistanceExistingLc = distanceLc,
                StateAuthorityApproval = stateApproval,
                DprConsultancy = dprConsultancy,
                ExecutiveAgency = executiveAgency,
                Gad = gad,
                GadRemark = gadRemark,
                CheckedReceived = checkedReceived,
                CheckedReceivedRemark = checkedReceivedRemark,
                SanctionedCont = sanctionedCont,
                SanctionedDe = sanctionedDe,
                TenderStatus = tenderStatus,
                Lc7a37a = sec7a,
                Gazette20A = sec20aGazette,
                Paper20A = sec20aPaper,
                Form20B = sec20b,
                Form20C = sec20c,
                Form20D = sec20d,
                Gazette20E = sec20eGazette,
                Paper20E = sec20ePaper,
                Form20F = sec20f,
                Remark = landRemark,
                StStatus = utilSnt,
                TrdStatus = utilTrd,
                ElectricalG = utilElectrical,
                SubstructureStatus = substructure,
                SuperStructureStatus = superstructure,
                CommissioningStatus = commissioning,
                GadExit = exitsGad,
                GadExitRemark = exitsGadRemark,
                ExitNo = exitNo,
                NoExit = noExit,
                Design = designField,
                NocClosingLc = nocClosingLc,
                Soa = soa
            };
        }
    }
}