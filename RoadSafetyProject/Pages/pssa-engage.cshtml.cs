using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using RoadSafetyProject.Models;

namespace RoadSafetyProject.Pages
{
    public class pssa_engageModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public List<Pssa> PssaList { get; set; } = new();

        [BindProperty]
        public Pssa Pssa { get; set; } = new();

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
        public int TotalRecords { get; set; }

        public pssa_engageModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //==========================
        // Load Page
        //==========================
        public void OnGet()
        {
            LoadData();
        }

        //==========================
        // Common Method
        //==========================
        private void LoadData()
        {
            try
            {
                PssaList.Clear();

                string connString = _configuration.GetConnectionString("OracleDb");

                using OracleConnection con = new OracleConnection(connString);

                con.Open();

                string sql = @"SELECT
                                SRNO,
                                HQ,
                                DESIGNATION,
                                NOMITEDWORK,
                                NAMEOFPOC
                               FROM PSSA
                               ORDER BY SRNO";

                using OracleCommand cmd = new OracleCommand(sql, con);

                using OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    PssaList.Add(new Pssa
                    {
                        SRNO = Convert.ToInt32(dr["SRNO"]),
                        HQ = dr["HQ"].ToString(),
                        DESIGNATION = dr["DESIGNATION"].ToString(),
                        NOMITEDWORK = dr["NOMITEDWORK"].ToString(),
                        NAMEOFPOC = dr["NAMEOFPOC"].ToString()
                    });
                }

                TotalRecords = PssaList.Count;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        //==========================
        // INSERT
        //==========================
        public IActionResult OnPostInsert()
        {
            try
            {
                string connString = _configuration.GetConnectionString("OracleDb");

                using OracleConnection con = new OracleConnection(connString);

                con.Open();

                string sql = @"INSERT INTO PSSA
                               (
                                   SRNO,
                                   HQ,
                                   DESIGNATION,
                                   NOMITEDWORK,
                                   NAMEOFPOC
                               )
                               VALUES
                               (
                                   :SRNO,
                                   :HQ,
                                   :DESIGNATION,
                                   :NOMITEDWORK,
                                   :NAMEOFPOC
                               )";

                OracleCommand cmd = new OracleCommand(sql, con);

                cmd.Parameters.Add(":SRNO", OracleDbType.Int32).Value = Pssa.SRNO;
                cmd.Parameters.Add(":HQ", OracleDbType.Varchar2).Value = Pssa.HQ;
                cmd.Parameters.Add(":DESIGNATION", OracleDbType.Varchar2).Value = Pssa.DESIGNATION;
                cmd.Parameters.Add(":NOMITEDWORK", OracleDbType.Varchar2).Value = Pssa.NOMITEDWORK;
                cmd.Parameters.Add(":NAMEOFPOC", OracleDbType.Varchar2).Value = Pssa.NAMEOFPOC;

                cmd.ExecuteNonQuery();

                SuccessMessage = "Record inserted successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadData();

            return Page();
        }

        //==========================
        // UPDATE
        //==========================
        public IActionResult OnPostUpdate()
        {
            try
            {
                string connString = _configuration.GetConnectionString("OracleDb");

                using OracleConnection con = new OracleConnection(connString);

                con.Open();

                string sql = @"UPDATE PSSA
                               SET
                                   HQ=:HQ,
                                   DESIGNATION=:DESIGNATION,
                                   NOMITEDWORK=:NOMITEDWORK,
                                   NAMEOFPOC=:NAMEOFPOC
                               WHERE SRNO=:SRNO";

                OracleCommand cmd = new OracleCommand(sql, con);

                cmd.Parameters.Add(":HQ", OracleDbType.Varchar2).Value = Pssa.HQ;
                cmd.Parameters.Add(":DESIGNATION", OracleDbType.Varchar2).Value = Pssa.DESIGNATION;
                cmd.Parameters.Add(":NOMITEDWORK", OracleDbType.Varchar2).Value = Pssa.NOMITEDWORK;
                cmd.Parameters.Add(":NAMEOFPOC", OracleDbType.Varchar2).Value = Pssa.NAMEOFPOC;
                cmd.Parameters.Add(":SRNO", OracleDbType.Int32).Value = Pssa.SRNO;

                cmd.ExecuteNonQuery();

                SuccessMessage = "Record updated successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadData();

            return Page();
        }

        //==========================
        // DELETE
        //==========================
        public IActionResult OnPostDelete()
        {
            try
            {
                string connString = _configuration.GetConnectionString("OracleDb");

                using OracleConnection con = new OracleConnection(connString);

                con.Open();

                string sql = @"DELETE FROM PSSA
                               WHERE SRNO=:SRNO";

                OracleCommand cmd = new OracleCommand(sql, con);

                cmd.Parameters.Add(":SRNO", OracleDbType.Int32).Value = Pssa.SRNO;

                cmd.ExecuteNonQuery();

                SuccessMessage = "Record deleted successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadData();

            return Page();
        }

        /* save */

        public IActionResult OnPostSave()
        {
            try
            {
                string connString = _configuration.GetConnectionString("OracleDb");

                using OracleConnection con = new OracleConnection(connString);
                con.Open();

                // Check if record exists
                string checkSql = "SELECT COUNT(*) FROM PSSA WHERE SRNO = :SRNO";

                OracleCommand checkCmd = new OracleCommand(checkSql, con);
                checkCmd.Parameters.Add(":SRNO", OracleDbType.Int32).Value = Pssa.SRNO;

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                OracleCommand cmd;

                if (count == 0)
                {
                    // INSERT
                    cmd = new OracleCommand(@"
                INSERT INTO PSSA
                (SRNO,HQ,DESIGNATION,NOMITEDWORK,NAMEOFPOC)
                VALUES
                (:SRNO,:HQ,:DESIGNATION,:NOMITEDWORK,:NAMEOFPOC)", con);

                    SuccessMessage = "Record saved successfully.";
                }
                else
                {
                    // UPDATE
                    cmd = new OracleCommand(@"
                UPDATE PSSA
                SET
                    HQ=:HQ,
                    DESIGNATION=:DESIGNATION,
                    NOMITEDWORK=:NOMITEDWORK,
                    NAMEOFPOC=:NAMEOFPOC
                WHERE SRNO=:SRNO", con);

                    SuccessMessage = "Record updated successfully.";
                }

                cmd.Parameters.Add(":SRNO", OracleDbType.Int32).Value = Pssa.SRNO;
                cmd.Parameters.Add(":HQ", OracleDbType.Varchar2).Value = Pssa.HQ;
                cmd.Parameters.Add(":DESIGNATION", OracleDbType.Varchar2).Value = Pssa.DESIGNATION;
                cmd.Parameters.Add(":NOMITEDWORK", OracleDbType.Varchar2).Value = Pssa.NOMITEDWORK;
                cmd.Parameters.Add(":NAMEOFPOC", OracleDbType.Varchar2).Value = Pssa.NAMEOFPOC;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadData();

            return Page();
        }
    }
}