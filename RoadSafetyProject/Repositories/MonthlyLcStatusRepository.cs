using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using RoadSafetyProject.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoadSafetyProject.Repositories
{
    public class MonthlyLcStatusRepository
    {
        private readonly string _connectionString;

        public MonthlyLcStatusRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("OracleDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'OracleDb' was not found in appsettings.json. " +
                    "Check the 'ConnectionStrings' section and the key name.");
        }

        // =========================================================
        // GET ALL
        // =========================================================
        public List<MonthlyLcStatus> GetAll()
        {
            var list = new List<MonthlyLcStatus>();

            const string sql = @"
                SELECT
                    ID,
                    LC_NO,
                    DIVISION,
                    LC_NAME,
                    SECTION,
                    KM,
                    EXECUTING_AGENCY_RAILWAY,
                    EXECUTING_AGENCY_APPROACH,
                    ROAD_TYPE,
                    WORK_HELD_UP_REASON,
                    RLY_PROGRESS,
                    APPROACH_PROGRESS,
                    STATUS_AS_ON_DATE,
                    WORK_STATUS_WEEK1,
                    WORK_STATUS_WEEK2,
                    WORK_STATUS_WEEK3,
                    WORK_STATUS_WEEK4,
                    CREATED_DATE,
                    UPDATED_DATE
                FROM MONTHLY_LC_STATUS
                ORDER BY ID DESC";

            using var con = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand(sql, con);

            con.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        // =========================================================
        // GET BY ID
        // =========================================================
        public MonthlyLcStatus? GetById(int id)
        {
            const string sql = @"
                SELECT
                    ID,
                    LC_NO,
                    DIVISION,
                    LC_NAME,
                    SECTION,
                    KM,
                    EXECUTING_AGENCY_RAILWAY,
                    EXECUTING_AGENCY_APPROACH,
                    ROAD_TYPE,
                    WORK_HELD_UP_REASON,
                    RLY_PROGRESS,
                    APPROACH_PROGRESS,
                    STATUS_AS_ON_DATE,
                    WORK_STATUS_WEEK1,
                    WORK_STATUS_WEEK2,
                    WORK_STATUS_WEEK3,
                    WORK_STATUS_WEEK4,
                    CREATED_DATE,
                    UPDATED_DATE
                FROM MONTHLY_LC_STATUS
                WHERE ID = :id";

            using var con = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand(sql, con);

            cmd.BindByName = true;

            cmd.Parameters.Add(
                "id",
                OracleDbType.Int32
            ).Value = id;

            con.Open();

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return Map(reader);
            }

            return null;
        }

        // =========================================================
        // INSERT
        // =========================================================
        public int Insert(MonthlyLcStatus m)
        {
            const string sql = @"
                INSERT INTO MONTHLY_LC_STATUS
                (
                    LC_NO,
                    DIVISION,
                    LC_NAME,
                    SECTION,
                    KM,
                    EXECUTING_AGENCY_RAILWAY,
                    EXECUTING_AGENCY_APPROACH,
                    ROAD_TYPE,
                    WORK_HELD_UP_REASON,
                    RLY_PROGRESS,
                    APPROACH_PROGRESS,
                    STATUS_AS_ON_DATE,
                    WORK_STATUS_WEEK1,
                    WORK_STATUS_WEEK2,
                    WORK_STATUS_WEEK3,
                    WORK_STATUS_WEEK4,
                    CREATED_DATE
                )
                VALUES
                (
                    :lcNo,
                    :division,
                    :lcName,
                    :section,
                    :km,
                    :railwayAgency,
                    :approachAgency,
                    :roadType,
                    :workHeldUpReason,
                    :rlyProgress,
                    :approachProgress,
                    :statusDate,
                    :week1,
                    :week2,
                    :week3,
                    :week4,
                    SYSDATE
                )
                RETURNING ID INTO :newId";

            using var con = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand(sql, con);

            cmd.BindByName = true;

            AddParameters(cmd, m);

            var newId = new OracleParameter(
                "newId",
                OracleDbType.Int32
            )
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(newId);

            con.Open();

            cmd.ExecuteNonQuery();

            return Convert.ToInt32(newId.Value.ToString());
        }

        // =========================================================
        // UPDATE
        // =========================================================
        public bool Update(MonthlyLcStatus m)
        {
            const string sql = @"
                UPDATE MONTHLY_LC_STATUS
                SET
                    LC_NO = :lcNo,
                    DIVISION = :division,
                    LC_NAME = :lcName,
                    SECTION = :section,
                    KM = :km,
                    EXECUTING_AGENCY_RAILWAY = :railwayAgency,
                    EXECUTING_AGENCY_APPROACH = :approachAgency,
                    ROAD_TYPE = :roadType,
                    WORK_HELD_UP_REASON = :workHeldUpReason,
                    RLY_PROGRESS = :rlyProgress,
                    APPROACH_PROGRESS = :approachProgress,
                    STATUS_AS_ON_DATE = :statusDate,
                    WORK_STATUS_WEEK1 = :week1,
                    WORK_STATUS_WEEK2 = :week2,
                    WORK_STATUS_WEEK3 = :week3,
                    WORK_STATUS_WEEK4 = :week4,
                    UPDATED_DATE = SYSDATE
                WHERE ID = :id";

            using var con = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand(sql, con);

            cmd.BindByName = true;

            AddParameters(cmd, m);

            cmd.Parameters.Add(
                "id",
                OracleDbType.Int32
            ).Value = m.Id;

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }

        // =========================================================
        // DELETE
        // =========================================================
        public bool Delete(int id)
        {
            const string sql = @"
                DELETE FROM MONTHLY_LC_STATUS
                WHERE ID = :id";

            using var con = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand(sql, con);

            cmd.BindByName = true;

            cmd.Parameters.Add(
                "id",
                OracleDbType.Int32
            ).Value = id;

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }

        // =========================================================
        // PARAMETERS
        // =========================================================
        private void AddParameters(
            OracleCommand cmd,
            MonthlyLcStatus m)
        {
            cmd.Parameters.Add(
                "lcNo",
                OracleDbType.Int32
            ).Value = m.LcNo;

            cmd.Parameters.Add(
                "division",
                OracleDbType.Varchar2
            ).Value = DbValue(m.Division);

            cmd.Parameters.Add(
                "lcName",
                OracleDbType.Varchar2
            ).Value = DbValue(m.LcName);

            cmd.Parameters.Add(
                "section",
                OracleDbType.Varchar2
            ).Value = DbValue(m.Section);

            cmd.Parameters.Add(
                "km",
                OracleDbType.Varchar2
            ).Value = DbValue(m.Km);

            cmd.Parameters.Add(
                "railwayAgency",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.ExecutingAgencyRailway
            );

            cmd.Parameters.Add(
                "approachAgency",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.ExecutingAgencyApproach
            );

            cmd.Parameters.Add(
                "roadType",
                OracleDbType.Varchar2
            ).Value = DbValue(m.RoadType);

            cmd.Parameters.Add(
                "workHeldUpReason",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.WorkHeldUpReason
            );

            // IMPORTANT:
            // Oracle column is VARCHAR2(500)
            cmd.Parameters.Add(
                "rlyProgress",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.RlyProgress
            );

            // IMPORTANT:
            // Oracle column is VARCHAR2(500)
            cmd.Parameters.Add(
                "approachProgress",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.ApproachProgress
            );

            cmd.Parameters.Add(
                "statusDate",
                OracleDbType.Date
            ).Value =
                m.StatusAsOnDate.HasValue
                    ? m.StatusAsOnDate.Value
                    : DBNull.Value;

            cmd.Parameters.Add(
                "week1",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.WorkStatusWeek1
            );

            cmd.Parameters.Add(
                "week2",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.WorkStatusWeek2
            );

            cmd.Parameters.Add(
                "week3",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.WorkStatusWeek3
            );

            cmd.Parameters.Add(
                "week4",
                OracleDbType.Varchar2
            ).Value = DbValue(
                m.WorkStatusWeek4
            );
        }

        // =========================================================
        // MAP DATABASE RECORD
        // =========================================================
        private MonthlyLcStatus Map(
            OracleDataReader r)
        {
            return new MonthlyLcStatus
            {
                Id = Convert.ToInt32(r["ID"]),

                LcNo = Convert.ToInt32(r["LC_NO"]),

                Division = GetString(
                    r,
                    "DIVISION"
                ),

                LcName = GetString(
                    r,
                    "LC_NAME"
                ),

                Section = GetString(
                    r,
                    "SECTION"
                ),

                Km = GetString(
                    r,
                    "KM"
                ),

                ExecutingAgencyRailway =
                    GetString(
                        r,
                        "EXECUTING_AGENCY_RAILWAY"
                    ),

                ExecutingAgencyApproach =
                    GetString(
                        r,
                        "EXECUTING_AGENCY_APPROACH"
                    ),

                RoadType =
                    GetString(
                        r,
                        "ROAD_TYPE"
                    ),

                WorkHeldUpReason =
                    GetString(
                        r,
                        "WORK_HELD_UP_REASON"
                    ),

                RlyProgress =
                    GetString(
                        r,
                        "RLY_PROGRESS"
                    ),

                ApproachProgress =
                    GetString(
                        r,
                        "APPROACH_PROGRESS"
                    ),

                StatusAsOnDate =
                    GetDate(
                        r,
                        "STATUS_AS_ON_DATE"
                    ),

                WorkStatusWeek1 =
                    GetString(
                        r,
                        "WORK_STATUS_WEEK1"
                    ),

                WorkStatusWeek2 =
                    GetString(
                        r,
                        "WORK_STATUS_WEEK2"
                    ),

                WorkStatusWeek3 =
                    GetString(
                        r,
                        "WORK_STATUS_WEEK3"
                    ),

                WorkStatusWeek4 =
                    GetString(
                        r,
                        "WORK_STATUS_WEEK4"
                    ),

                CreatedDate =
                    GetDate(
                        r,
                        "CREATED_DATE"
                    ),

                UpdatedDate =
                    GetDate(
                        r,
                        "UPDATED_DATE"
                    )
            };
        }

        // =========================================================
        // HELPERS
        // =========================================================
        private static object DbValue(
            string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? DBNull.Value
                : value;
        }

        private static string? GetString(
            OracleDataReader reader,
            string column)
        {
            return reader[column] == DBNull.Value
                ? null
                : reader[column]?.ToString();
        }

        private static DateTime? GetDate(
            OracleDataReader reader,
            string column)
        {
            return reader[column] == DBNull.Value
                ? null
                : Convert.ToDateTime(
                    reader[column]
                );
        }
    }
}