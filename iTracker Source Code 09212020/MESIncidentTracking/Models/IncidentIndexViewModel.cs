using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <remarks>
/// ================================================================================
/// MODULE:  IncidentIndexViewModel.cs
///         
/// PURPOSE:
/// This class represents a single log entry object, meant for the index view.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2019-09-11  Updated display name for RequireES13 to Intervention Performed
/// Brad Robbins    2019-10-04  Added batch number
/// Brad Robbins    2019-12-03  Added user ID
/// Brad Robbins    2020-05-13  Added short description
/// Brad Robbins    2020-06-02  Removed "root cause" in favor of classification
/// Brad Robbins    2020-06-15  Changed from struct to class
/// ================================================================================
/// </remarks>
/// 
namespace ES13Web.Models
{
    ///<summary>
    ///This class represents a single incident log entry object, 
    ///meant for the index view.
    ///</summary>
    public class IncidentIndexViewModel
    {
        /// <summary>Gets/sets the incident identifier.</summary>
        public int IncidentId { get; set; }

        ///<summary>Gets/sets the activity performed date.</summary>
        [DataType(DataType.DateTime)]
        public DateTime ActivityPerformedDateTime { get; set; }

        ///<summary>Gets/sets the formatted activity performed date.</summary>
        [Display(Name = "Activity Performed Date Time")]
        public string FormattedLocalActivityPerformedDateTime { get; set; }

        ///<summary>Gets/sets the manufacturing area identifier.</summary>
        [DisplayName("Manufacturing Area")]
        public string ManufacturingAreaName { get; set; }

        ///<summary>Gets/sets the related classification.</summary>
        [DisplayName("Classification")]
        public string ClassificationName { get; set; }

        ///<summary>Gets/sets the engineer name.</summary>
        [DisplayName("Engineer Name")]
        public string EngineerName { get; set; }

        ///<summary>Gets/sets the short description.</summary>
        [DisplayName("Short Description")]
        public string ShortDescription { get; set; }

        ///<summary>Gets/sets whether intervention was performed.</summary>
        [DisplayName("Intervention Performed?")]
        public bool RequireES13 { get; set; }

        ///<summary>Gets/sets the Werum ticket number.</summary>
        [DisplayName("Werum Ticket Number")]
        public string WerumTicket { get; set; }

        ///<summary>Gets/sets the BT service request number if used.</summary>
        [DisplayName("BT Service Request Number")]
        public string BTServiceRequestNumber { get; set; }
        
        ///<summary>Gets/sets whether the incident is resolved.</summary>
        [DisplayName("Incident Resolved?")]
        public bool Resolved { get; set; }

        ///<summary>Gets/sets whether the related batch number.</summary>
        [DisplayName("Batch Number")]
        public string BatchNumber { get; set; }

        //not for display
        public string UserId { get; set; }

    }
}
