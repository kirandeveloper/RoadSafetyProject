using System;

namespace RoadSafetyProject.Models
{
    /// <summary>
    /// Maps 1:1 to the RSP_MASTER Oracle table.
    /// Property names use PascalCase; DB columns are mapped in RspMasterRepository.
    /// </summary>
    public class RspMaster
    {
        public int Id { get; set; }                      // ID (PK)
        public int? SrNo { get; set; }                    // SR_NO

        // 01 Basic Details
        public string YearOfSanction { get; set; }          // YEAR_OF_SANCTION
        public string LcNo { get; set; }                  // LC_NO
        public string LocationKm { get; set; }             // LOCATION_KM
        public string Division { get; set; }               // DIVISION
        public string LcStatus { get; set; }                // LC_STATUS ('Sanction'/'UnSanction')

        public string SectionName { get; set; }            // SECTION_NAME
        public string SpanArrangement { get; set; }        // SPAN_ARRANGEMENT
        public string SkewAngle { get; set; }             // SKEW_ANGLE
        public string DistanceExistingLc { get; set; }      // DISTANCE_EXISTING_LC
        public string StateAuthorityApproval { get; set; }  // STATE_AUTHORITY_APPROVAL
        public string DprConsultancy { get; set; }          // DPR_CONSULTANCY
        public string ExecutiveAgency { get; set; }         // EXECUTIVE_AGENCY

        // 02 Tender Details
        public string Gad { get; set; }                    // GAD ('Y'/'N')
        public string CheckedReceived { get; set; }        // CHECKED_RECEIVED ('Y'/'N')
        public string SanctionedCont { get; set; }        // SANCTIONED_CONT
        public string SanctionedDe { get; set; }           // SANCTIONED_DE
        public string TenderStatus { get; set; }            // TENDER_STATUS

        // 03 Land Acquisition (DB columns are DATE - parsed from the textarea text)
        public string Lc7a37a { get; set; }              // LC_7A_37A
        public string Gazette20A { get; set; }           // GAZETTE_20A
        public string Paper20A { get; set; }             // PAPER_20A
        public string Form20B { get; set; }              // FORM_20B
        public string Form20C { get; set; }              // FORM_20C
        public string Form20D { get; set; }              // FORM_20D
        public string Gazette20E { get; set; }           // GAZETTE_20E
        public string Paper20E { get; set; }             // PAPER_20E
        public string Form20F { get; set; }              // FORM_20F
        public string Remark { get; set; }                  // REMARK

        // 04 Utility Shifting
        public string StStatus { get; set; }                // ST_STATUS
        public string TrdStatus { get; set; }                // TRD_STATUS
        public string ElectricalG { get; set; }              // ELECTRICAL_G

        // 05 Design
        public string SubstructureStatus { get; set; }       // SUBSTRUCTURE_STATUS
        public string SuperStructureStatus { get; set; }     // SUPER_STRUCTURE_STATUS
        public string CommissioningStatus { get; set; }      // COMMISSIONING_STATUS
        public string GadExit { get; set; }                  // GAD_EXIT ('Y'/'N')
        public string DesignStatus { get; set; }             // DESIGN_STATUS (not fed by form yet)
        public string ExitNo { get; set; }                   // EXIT_NO
        public string NoExit { get; set; }                   // NO_EXIT
        public string Design { get; set; }                   // DESIGN

        // 06 NOC
        public string NocClosingLc { get; set; }             // NOC_CLOSING_LC
        public string Soa { get; set; }                       // SOA

        public string GadRemark { get; set; }
        public string CheckedReceivedRemark { get; set; }
        public string GadExitRemark { get; set; }



    }
}
