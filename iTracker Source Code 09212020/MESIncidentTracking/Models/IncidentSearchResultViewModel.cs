using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <remarks>
/// ================================================================================
/// MODULE:  IncidentSearchResultViewModel.cs
///         
/// PURPOSE:
/// This class represents a single incident search result.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2019-09-11  Renamed RequiresES13 to InterventionPerformed
/// Brad Robbins    2019-10-04  Added batch number
/// Brad Robbins    2019-12-06  Added hidden UserId field
/// Brad Robbins    2020-05-13  Added short description field
/// Brad Robbins    2020-06-02  Removed "root cause" in favor of classification
/// ================================================================================
/// </remarks>

namespace ES13Web.Models
{
    ///<summary>This class represents a single incident search result.summary>
    public struct IncidentSearchResultViewModel
    {
        /// <summary>Gets/sets the incident identifier.</summary>
        public int IncidentId { get; set; }

        ///<summary>Gets/sets the activity performed date.</summary>
        [DataType(DataType.DateTime)]
        public DateTime ActivityPerformedDateTime { get; set; }

        ///<summary>Gets/sets the activity performed date.</summary>
        [Display(Name = "Activity Performed Date Time")]
        [DataType(DataType.DateTime)]
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
        [DisplayName("InterventionPerformed?")]
        public bool InterventionPerformed { get; set; }

        [DisplayName("Incident Resolved?")]
        public bool Resolved { get; set; }

        ///<summary>Gets/sets the Werum ticket number.</summary>
        [DisplayName("Werum Ticket Number")]
        public string WerumTicket { get; set; }

        ///<summary>Gets/sets the BT service request number if used.</summary>
        [DisplayName("BT Service Request Number")]
        public string BTServiceRequestNumber { get; set; }
        
        ///<summary>Gets/sets the related batch number.</summary>
        [DisplayName("Batch Number")]
        public string BatchNumber { get; set; }

        ///hidden field
        public string UserId { get; set; }
    }
}
