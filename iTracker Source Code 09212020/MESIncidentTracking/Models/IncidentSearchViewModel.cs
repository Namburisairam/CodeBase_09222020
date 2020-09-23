using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <remarks>
/// ================================================================================
/// MODULE:  IncidentSearchViewModel.cs
///         
/// PURPOSE:
/// This class represents a single incident meant for the Search view.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2019-09-11  Added TernaryOption, HasWerumTicket, InterventionPerformed; reworked Resolved
/// Brad Robbins    2019-10-04  Added batch number
/// Brad Robbins    2020-05-13  Added short description field
/// Brad Robbins    2020-06-02  Removed "root cause" in favor of classification
/// ================================================================================
/// </remarks>

namespace ES13Web.Models
{
    public enum TernaryOption
    {
        All,
        Yes,
        No
    }

    public class IncidentSearchViewModel
    {
        ///<summary>Gets/sets the search results.</summary>
        public List<IncidentSearchResultViewModel> Results { get; set; }

        ///<summary>Gets/sets the search start date.</summary>
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDateTime { get; set; }

        ///<summary>Gets/sets the search end date.</summary>
        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDateTime { get; set; }
        
        ///<summary>Gets/sets the manufacturing area identifier.</summary>
        [DisplayName("Manufacturing Area")]
        public int? ManufacturingAreaId { get; set; }
        ///<summary>Gets/sets the engineer name.</summary>
        [DisplayName("Engineer Name")]
        [StringLength(50, ErrorMessage = "Engineer name cannot be more than 50 characters.")]
        public string EngineerName { get; set; }

        ///<summary>Gets/sets the short description.</summary>
        [DisplayName("Short Description")]
        [StringLength(100, ErrorMessage = "Short description cannot be more than 100 characters.")]
        public string ShortDescription { get; set; }
        
        ///<summary>Gets/sets the incident description.</summary>
        [DisplayName("Incident Description")]
        [StringLength(500, ErrorMessage = "Incident description cannot be more than 500 characters.")]
        public string IncidentDescription { get; set; }

        ///<summary>Gets/sets the action summary.</summary>
        [DisplayName("Action Summary")]
        [StringLength(500, ErrorMessage = "Action summary cannot be more than 500 characters.")]
        public string ActionSummary { get; set; }

        ///<summary>Gets/sets the related classification.</summary>
        [DisplayName("Classification")]
        public int? ClassificationId { get; set; }
        
        ///<summary>Gets/sets whether ES13 is required.</summary>
        [DisplayName("Intervention Performed?")]
        [EnumDataType(typeof(TernaryOption))]
        public TernaryOption InterventionPerformed { get; set; }

        [DisplayName("Incident Resolved?")]
        [EnumDataType(typeof(TernaryOption))]
        public TernaryOption Resolved { get; set; }

        ///<summary>Gets/sets the Werum ticket number.</summary>
        [DisplayName("Werum Ticket Number")]
        public string WerumTicket { get; set; }

        ///<summary>Gets if there is a Werum ticket.</summary>
        [DisplayName("Has Werum Ticket?")]
        [EnumDataType(typeof(TernaryOption))]
        public TernaryOption HasWerumTicket { get; set; }

        ///<summary>Gets/sets the BT service request number if used.</summary>
        [DisplayName("BT Service Request Number")]
        public string BTServiceRequestNumber { get; set; }

        ///<summary>Gets/sets the related batch number.</summary>
        [DisplayName("Batch Number")]
        public string BatchNumber { get; set; }

        ///<summary>Gets/sets the result limit.</summary>
        [DisplayName("Result Limit")]
        [DefaultValue(20)]
        public int Limit { get; set; }
    }
}
