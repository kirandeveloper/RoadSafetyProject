using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RoadSafetyProject.Data;
using RoadSafetyProject.Models;

namespace RoadSafetyProject.Pages
{
    public class addnewlcModel : PageModel
    {
        private readonly RspMasterRepository _repo;

        public addnewlcModel(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("OracleDb");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'OracleDb' was not found in appsettings.json. " +
                    "Check the 'ConnectionStrings' section and the key name.");

            _repo = new RspMasterRepository(connectionString);
        }

        // Renders the page. The form/table are populated client-side via AJAX.
        public void OnGet()
        {
        }

        // GET ?handler=List  -> table data for the "view" grid
        public JsonResult OnGetList()
        {
            var items = _repo.GetAll();
            return new JsonResult(new { success = true, data = items });
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
                return new JsonResult(new { success = false, message = "Save failed: " + ex.Message }) { StatusCode = 500 };
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
        public int? srNo { get; set; }
        public int? yearSanction { get; set; }
        public string lcNo { get; set; }
        public string locationKm { get; set; }
        public string division { get; set; }
        public string section { get; set; }
        public string spanArrangement { get; set; }
        public decimal? skewAngle { get; set; }
        public string distanceLc { get; set; }
        public string stateApproval { get; set; }
        public string dprConsultancy { get; set; }
        public string executiveAgency { get; set; }
        public string gad { get; set; }
        public string checkedReceived { get; set; }
        public decimal? sanctionedCont { get; set; }
        public decimal? sanctionedDe { get; set; }
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
                SrNo = srNo,
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
                CheckedReceived = checkedReceived,
                SanctionedCont = sanctionedCont,
                SanctionedDe = sanctionedDe,
                TenderStatus = tenderStatus,
                Lc7a37a = ParseDate(sec7a),
                Gazette20A = ParseDate(sec20aGazette),
                Paper20A = ParseDate(sec20aPaper),
                Form20B = ParseDate(sec20b),
                Form20C = ParseDate(sec20c),
                Form20D = ParseDate(sec20d),
                Gazette20E = ParseDate(sec20eGazette),
                Paper20E = ParseDate(sec20ePaper),
                Form20F = ParseDate(sec20f),
                Remark = landRemark,
                StStatus = utilSnt,
                TrdStatus = utilTrd,
                ElectricalG = utilElectrical,
                SubstructureStatus = substructure,
                SuperStructureStatus = superstructure,
                CommissioningStatus = commissioning,
                GadExit = exitsGad,
                ExitNo = exitNo,
                NoExit = noExit,
                Design = designField,
                NocClosingLc = nocClosingLc,
                Soa = soa
            };
        }

        private static DateTime? ParseDate(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            return DateTime.TryParse(text, out var dt) ? dt : (DateTime?)null;
        }
    }
}