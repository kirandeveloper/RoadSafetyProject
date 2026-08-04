using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using RoadSafetyProject.Models;

namespace RoadSafetyProject.Data
{
    /// <summary>
    /// Plain ADO.NET repository for RSP_MASTER using Oracle.ManagedDataAccess.Client (ODP.NET).
    /// Works against Oracle 10g using bind parameters and standard SQL only
    /// (no analytic/JSON features that 10g doesn't support).
    /// </summary>
    public class RspMasterRepository
    {
        private readonly string _connectionString;

        public RspMasterRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private OracleConnection GetConnection() => new OracleConnection(_connectionString);

        // ---------- CREATE / UPDATE (single Save entry point) ----------
        public int Save(RspMaster m)
        {
            return m.Id > 0 ? Update(m) : Insert(m);
        }

        public int Insert(RspMaster m)
        {
            const string sql = @"
                INSERT INTO RSP_MASTER
                (ID, SR_NO, YEAR_OF_SANCTION, LC_NO, LOCATION_KM, DIVISION, LC_STATUS, SECTION_NAME,
                 SPAN_ARRANGEMENT, SKEW_ANGLE, DISTANCE_EXISTING_LC, STATE_AUTHORITY_APPROVAL,
                 DPR_CONSULTANCY, EXECUTIVE_AGENCY, GAD, CHECKED_RECEIVED, SANCTIONED_CONT,
                 SANCTIONED_DE, TENDER_STATUS, LC_7A_37A, GAZETTE_20A, PAPER_20A, FORM_20B,
                 FORM_20C, FORM_20D, GAZETTE_20E, PAPER_20E, FORM_20F, REMARK, ST_STATUS,
                 TRD_STATUS, ELECTRICAL_G, SUBSTRUCTURE_STATUS, SUPER_STRUCTURE_STATUS,
                 COMMISSIONING_STATUS, GAD_EXIT, DESIGN_STATUS, EXIT_NO, NO_EXIT, DESIGN,
                 NOC_CLOSING_LC, SOA)
                VALUES
                (RSP_MASTER_SEQ.NEXTVAL, :srNo, :yearOfSanction, :lcNo, :locationKm, :division, :lcStatus, :sectionName,
                 :spanArrangement, :skewAngle, :distanceExistingLc, :stateAuthorityApproval,
                 :dprConsultancy, :executiveAgency, :gad, :checkedReceived, :sanctionedCont,
                 :sanctionedDe, :tenderStatus, :lc7a37a, :gazette20A, :paper20A, :form20B,
                 :form20C, :form20D, :gazette20E, :paper20E, :form20F, :remark, :stStatus,
                 :trdStatus, :electricalG, :substructureStatus, :superStructureStatus,
                 :commissioningStatus, :gadExit, :designStatus, :exitNo, :noExit, :design,
                 :nocClosingLc, :soa)
                RETURNING ID INTO :newId";

            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            AddParameters(cmd, m);

            var idParam = new OracleParameter("newId", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add(idParam);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex) when (ex.Number == 2289)
            {
                throw new InvalidOperationException(
                    "RSP_MASTER_SEQ sequence does not exist in this schema. " +
                    "Run: CREATE SEQUENCE RSP_MASTER_SEQ START WITH 1 INCREMENT BY 1 NOCACHE NOCYCLE; " +
                    "then try saving again.", ex);
            }
            catch (OracleException ex) when (ex.Number == 12899)
            {
                throw new InvalidOperationException(
                    "A value entered is too long for its database column. " + ex.Message +
                    " Widen the column with ALTER TABLE RSP_MASTER MODIFY (<column> VARCHAR2(<size>)); " +
                    "or shorten the input.", ex);
            }
            catch (OracleException ex) when (ex.Number == 1 && ex.Message.Contains("PK_ROB_MASTER"))
            {
                throw new InvalidOperationException(
                    "RSP_MASTER_SEQ is out of sync and is generating IDs that already exist. " +
                    "Resync it: DROP SEQUENCE RSP_MASTER_SEQ; then CREATE SEQUENCE RSP_MASTER_SEQ " +
                    "START WITH <max(ID)+1> INCREMENT BY 1 NOCACHE NOCYCLE;", ex);
            }

            var oracleDecimal = (Oracle.ManagedDataAccess.Types.OracleDecimal)idParam.Value;
            return oracleDecimal.ToInt32();
        }

        public int Update(RspMaster m)
        {
            const string sql = @"
                UPDATE RSP_MASTER SET
                    SR_NO = :srNo,
                    YEAR_OF_SANCTION = :yearOfSanction,
                    LC_NO = :lcNo,
                    LOCATION_KM = :locationKm,
                    DIVISION = :division,
                    LC_STATUS = :lcStatus,
                    SECTION_NAME = :sectionName,
                    SPAN_ARRANGEMENT = :spanArrangement,
                    SKEW_ANGLE = :skewAngle,
                    DISTANCE_EXISTING_LC = :distanceExistingLc,
                    STATE_AUTHORITY_APPROVAL = :stateAuthorityApproval,
                    DPR_CONSULTANCY = :dprConsultancy,
                    EXECUTIVE_AGENCY = :executiveAgency,
                    GAD = :gad,
                    CHECKED_RECEIVED = :checkedReceived,
                    SANCTIONED_CONT = :sanctionedCont,
                    SANCTIONED_DE = :sanctionedDe,
                    TENDER_STATUS = :tenderStatus,
                    LC_7A_37A = :lc7a37a,
                    GAZETTE_20A = :gazette20A,
                    PAPER_20A = :paper20A,
                    FORM_20B = :form20B,
                    FORM_20C = :form20C,
                    FORM_20D = :form20D,
                    GAZETTE_20E = :gazette20E,
                    PAPER_20E = :paper20E,
                    FORM_20F = :form20F,
                    REMARK = :remark,
                    ST_STATUS = :stStatus,
                    TRD_STATUS = :trdStatus,
                    ELECTRICAL_G = :electricalG,
                    SUBSTRUCTURE_STATUS = :substructureStatus,
                    SUPER_STRUCTURE_STATUS = :superStructureStatus,
                    COMMISSIONING_STATUS = :commissioningStatus,
                    GAD_EXIT = :gadExit,
                    DESIGN_STATUS = :designStatus,
                    EXIT_NO = :exitNo,
                    NO_EXIT = :noExit,
                    DESIGN = :design,
                    NOC_CLOSING_LC = :nocClosingLc,
                    SOA = :soa
                WHERE ID = :id";

            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            AddParameters(cmd, m);
            cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = m.Id });

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex) when (ex.Number == 12899)
            {
                throw new InvalidOperationException(
                    "A value entered is too long for its database column. " + ex.Message +
                    " Widen the column with ALTER TABLE RSP_MASTER MODIFY (<column> VARCHAR2(<size>)); " +
                    "or shorten the input.", ex);
            }

            return m.Id;
        }

        // ---------- DELETE ----------
        public bool Delete(int id)
        {
            const string sql = "DELETE FROM RSP_MASTER WHERE ID = :id";
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = id });
            return cmd.ExecuteNonQuery() > 0;
        }

        // ---------- READ (single) ----------
        public RspMaster GetById(int id)
        {
            const string sql = "SELECT * FROM RSP_MASTER WHERE ID = :id";
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = id });
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        // ---------- READ (list / view) ----------
        public List<RspMaster> GetAll()
        {
            const string sql = "SELECT * FROM RSP_MASTER ORDER BY ID DESC";
            var list = new List<RspMaster>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));
            return list;
        }

        // ---------- helpers ----------
        private static void AddParameters(OracleCommand cmd, RspMaster m)
        {
            cmd.Parameters.Add(new OracleParameter("srNo", OracleDbType.Int32) { Value = (object)m.SrNo ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("yearOfSanction", OracleDbType.Int32) { Value = (object)m.YearOfSanction ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("lcNo", OracleDbType.Varchar2) { Value = (object)m.LcNo ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("locationKm", OracleDbType.Varchar2) { Value = (object)m.LocationKm ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("division", OracleDbType.Varchar2) { Value = (object)m.Division ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("lcStatus", OracleDbType.Varchar2) { Value = (object)m.LcStatus ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("sectionName", OracleDbType.Varchar2) { Value = (object)m.SectionName ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("spanArrangement", OracleDbType.Varchar2) { Value = (object)m.SpanArrangement ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("skewAngle", OracleDbType.Decimal) { Value = (object)m.SkewAngle ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("distanceExistingLc", OracleDbType.Varchar2) { Value = (object)m.DistanceExistingLc ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("stateAuthorityApproval", OracleDbType.Varchar2) { Value = (object)m.StateAuthorityApproval ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("dprConsultancy", OracleDbType.Varchar2) { Value = (object)m.DprConsultancy ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("executiveAgency", OracleDbType.Varchar2) { Value = (object)m.ExecutiveAgency ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("gad", OracleDbType.Char) { Value = (object)NormalizeYN(m.Gad) ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("checkedReceived", OracleDbType.Char) { Value = (object)NormalizeYN(m.CheckedReceived) ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("sanctionedCont", OracleDbType.Decimal) { Value = (object)m.SanctionedCont ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("sanctionedDe", OracleDbType.Decimal) { Value = (object)m.SanctionedDe ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("tenderStatus", OracleDbType.Varchar2) { Value = (object)m.TenderStatus ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("lc7a37a", OracleDbType.Date) { Value = (object)m.Lc7a37a ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("gazette20A", OracleDbType.Date) { Value = (object)m.Gazette20A ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("paper20A", OracleDbType.Date) { Value = (object)m.Paper20A ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("form20B", OracleDbType.Date) { Value = (object)m.Form20B ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("form20C", OracleDbType.Date) { Value = (object)m.Form20C ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("form20D", OracleDbType.Date) { Value = (object)m.Form20D ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("gazette20E", OracleDbType.Date) { Value = (object)m.Gazette20E ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("paper20E", OracleDbType.Date) { Value = (object)m.Paper20E ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("form20F", OracleDbType.Date) { Value = (object)m.Form20F ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("remark", OracleDbType.Varchar2) { Value = (object)m.Remark ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("stStatus", OracleDbType.Varchar2) { Value = (object)m.StStatus ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("trdStatus", OracleDbType.Varchar2) { Value = (object)m.TrdStatus ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("electricalG", OracleDbType.Varchar2) { Value = (object)m.ElectricalG ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("substructureStatus", OracleDbType.Varchar2) { Value = (object)m.SubstructureStatus ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("superStructureStatus", OracleDbType.Varchar2) { Value = (object)m.SuperStructureStatus ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("commissioningStatus", OracleDbType.Varchar2) { Value = (object)m.CommissioningStatus ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("gadExit", OracleDbType.Varchar2) { Value = (object)NormalizeYN(m.GadExit) ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("designStatus", OracleDbType.Varchar2) { Value = (object)m.DesignStatus ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("exitNo", OracleDbType.Varchar2) { Value = (object)m.ExitNo ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("noExit", OracleDbType.Varchar2) { Value = (object)m.NoExit ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("design", OracleDbType.Varchar2) { Value = (object)m.Design ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("nocClosingLc", OracleDbType.Varchar2) { Value = (object)m.NocClosingLc ?? DBNull.Value });
            cmd.Parameters.Add(new OracleParameter("soa", OracleDbType.Varchar2) { Value = (object)m.Soa ?? DBNull.Value });
        }

        private static string NormalizeYN(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value.Trim().ToUpperInvariant().StartsWith("Y") ? "Y" : "N";
        }

        private static RspMaster Map(IDataReader r)
        {
            return new RspMaster
            {
                Id = GetInt(r, "ID") ?? 0,
                SrNo = GetInt(r, "SR_NO"),
                YearOfSanction = GetInt(r, "YEAR_OF_SANCTION"),
                LcNo = GetString(r, "LC_NO"),
                LocationKm = GetString(r, "LOCATION_KM"),
                Division = GetString(r, "DIVISION"),
                LcStatus = GetString(r, "LC_STATUS"),
                SectionName = GetString(r, "SECTION_NAME"),
                SpanArrangement = GetString(r, "SPAN_ARRANGEMENT"),
                SkewAngle = GetDecimal(r, "SKEW_ANGLE"),
                DistanceExistingLc = GetString(r, "DISTANCE_EXISTING_LC"),
                StateAuthorityApproval = GetString(r, "STATE_AUTHORITY_APPROVAL"),
                DprConsultancy = GetString(r, "DPR_CONSULTANCY"),
                ExecutiveAgency = GetString(r, "EXECUTIVE_AGENCY"),
                Gad = GetString(r, "GAD"),
                CheckedReceived = GetString(r, "CHECKED_RECEIVED"),
                SanctionedCont = GetDecimal(r, "SANCTIONED_CONT"),
                SanctionedDe = GetDecimal(r, "SANCTIONED_DE"),
                TenderStatus = GetString(r, "TENDER_STATUS"),
                Lc7a37a = GetDate(r, "LC_7A_37A"),
                Gazette20A = GetDate(r, "GAZETTE_20A"),
                Paper20A = GetDate(r, "PAPER_20A"),
                Form20B = GetDate(r, "FORM_20B"),
                Form20C = GetDate(r, "FORM_20C"),
                Form20D = GetDate(r, "FORM_20D"),
                Gazette20E = GetDate(r, "GAZETTE_20E"),
                Paper20E = GetDate(r, "PAPER_20E"),
                Form20F = GetDate(r, "FORM_20F"),
                Remark = GetString(r, "REMARK"),
                StStatus = GetString(r, "ST_STATUS"),
                TrdStatus = GetString(r, "TRD_STATUS"),
                ElectricalG = GetString(r, "ELECTRICAL_G"),
                SubstructureStatus = GetString(r, "SUBSTRUCTURE_STATUS"),
                SuperStructureStatus = GetString(r, "SUPER_STRUCTURE_STATUS"),
                CommissioningStatus = GetString(r, "COMMISSIONING_STATUS"),
                GadExit = GetString(r, "GAD_EXIT"),
                DesignStatus = GetString(r, "DESIGN_STATUS"),
                ExitNo = GetString(r, "EXIT_NO"),
                NoExit = GetString(r, "NO_EXIT"),
                Design = GetString(r, "DESIGN"),
                NocClosingLc = GetString(r, "NOC_CLOSING_LC"),
                Soa = GetString(r, "SOA")
            };
        }

        private static string GetString(IDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }

        private static int? GetInt(IDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (int?)null : Convert.ToInt32(r.GetValue(i));
        }

        private static decimal? GetDecimal(IDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (decimal?)null : Convert.ToDecimal(r.GetValue(i));
        }

        private static DateTime? GetDate(IDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (DateTime?)null : r.GetDateTime(i);
        }

        public List<DivisionCount> GetDivisionCounts()
        {
            const string sql = @"
            SELECT
            DIVISION,
            LC_STATUS,
            COUNT(*) TOTAL
            FROM RSP_MASTER
            GROUP BY DIVISION, LC_STATUS
            ORDER BY DIVISION";

            var list = new List<DivisionCount>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = new OracleCommand(sql, conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new DivisionCount
                {
                    Division = reader["DIVISION"].ToString(),
                    LC_STATUS = reader["LC_STATUS"].ToString(),
                    Total = Convert.ToInt32(reader["TOTAL"])
                });
            }

            return list;
        }


    }
}
