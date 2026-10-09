using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoadSafetyProject.Models;
using RoadSafetyProject.Repositories;

namespace RoadSafetyProject.Pages
{
    public class monthlylcstatusModel : PageModel
    {
        private readonly MonthlyLcStatusRepository _repo;

        public monthlylcstatusModel(MonthlyLcStatusRepository repo)
        {
            _repo = repo;
        }

        public void OnGet()
        {
            // The table retrieves records using ?handler=List.
        }

        public IActionResult OnGetList()
        {
            try
            {
                return new JsonResult(new { success = true, data = _repo.GetAll() });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Failed to load records: " + ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public IActionResult OnGetItem(int id)
        {
            if (id <= 0)
                return new JsonResult(new { success = false, message = "Invalid record ID." }) { StatusCode = 400 };

            try
            {
                var item = _repo.GetById(id);
                if (item == null)
                    return new JsonResult(new { success = false, message = "Record not found." }) { StatusCode = 404 };

                return new JsonResult(new { success = true, data = item });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Failed to load record: " + ex.Message }) { StatusCode = 500 };
            }
        }

        public IActionResult OnPostSave([FromBody] MonthlyLcStatus model)
        {
            if (model == null)
                return new JsonResult(new { success = false, message = "No data received." }) { StatusCode = 400 };

            if (model.LcNo <= 0)
                return new JsonResult(new { success = false, message = "Please enter a valid LC No." }) { StatusCode = 400 };

            try
            {
                if (model.Id > 0)
                {
                    // MonthlyLcStatusRepository.Update returns bool.
                    bool updated = _repo.Update(model);
                    if (!updated)
                        return new JsonResult(new { success = false, message = "Record not found or could not be updated." }) { StatusCode = 404 };

                    return new JsonResult(new { success = true, id = model.Id, message = "Monthly LC Status updated successfully." });
                }

                int newId = _repo.Insert(model);
                return new JsonResult(new { success = true, id = newId, message = "Monthly LC Status saved successfully." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Save failed: " + ex.Message }) { StatusCode = 500 };
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            if (id <= 0)
                return new JsonResult(new { success = false, message = "Invalid record ID." }) { StatusCode = 400 };

            try
            {
                bool deleted = _repo.Delete(id);
                if (!deleted)
                    return new JsonResult(new { success = false, message = "Record not found." }) { StatusCode = 404 };

                return new JsonResult(new { success = true, message = "Monthly LC Status deleted successfully." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Delete failed: " + ex.Message }) { StatusCode = 500 };
            }
        }
    }
}
