using System;

namespace RoadSafetyProject.Models
{
    public class MonthlyLcStatus
    {
        public int Id { get; set; }
        public int LcNo { get; set; }
        public string? Division { get; set; }
        public string? LcName { get; set; }
        public string? Section { get; set; }
        public string? Km { get; set; }
        public string? ExecutingAgencyRailway { get; set; }
        public string? ExecutingAgencyApproach { get; set; }
        public string? RoadType { get; set; }
        public string? WorkHeldUpReason { get; set; }
        public string? RlyProgress { get; set; }
        public string? ApproachProgress { get; set; }
        public DateTime? StatusAsOnDate { get; set; }
        public string? WorkStatusWeek1 { get; set; }
        public string? WorkStatusWeek2 { get; set; }
        public string? WorkStatusWeek3 { get; set; }
        public string? WorkStatusWeek4 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}