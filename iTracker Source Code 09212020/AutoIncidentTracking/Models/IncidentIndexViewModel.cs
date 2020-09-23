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
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-09-24
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-10-22  Initial version
/// Brad Robbins    2019-05-02  Changed from creation to activity performed date/time.
/// Brad Robbins    2019-12-06  Added user ID
/// Brad Robbins    2020-05-13  Added short description; removed control system
/// Brad Robbins    2020-06-02  Removed "root cause" in favor of classification
/// Brad Robbins    2020-06-12  Changed from struct to class
/// ================================================================================
/// </remarks>
/// 
namespace IncidentTracking.Models
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

        ///<summary>Gets/sets the activity performed formatted date.</summary>
        [Display(Name = "Activity Performed Date Time")]
        public string FormattedLocalActivityPerformedDateTime { get; set; }

        ///<summary>Gets/sets the manufacturing area identifier.</summary>
        [DisplayName("Manufacturing Area")]
        public string ManufacturingAreaName { get; set; }

        ///<summary>Gets/sets the short description.</summary>
        [DisplayName("Short Description")]
        public string ShortDescription { get; set; }

        ///<summary>Gets/sets the related classification.</summary>
        [DisplayName("Classification")]
        public string ClassificationName { get; set; }

        ///<summary>Gets/sets the engineer name.</summary>
        [DisplayName("Engineer Name")]
        public string EngineerName { get; set; }


        ///<summary>Gets/sets whether ES11 is required.</summary>
        [DisplayName("ES-11 Required?")]
        public bool RequireES11 { get; set; }

        ///<summary>Gets/sets whether pre-approval is required.</summary>
        [DisplayName("Pre-approval Required?")]
        public bool RequirePreApproval { get; set; }

        ///<summary>Gets/sets whether tagout is required.</summary>
        [DisplayName("Tagout Required?")]
        public bool RequireTagOut { get; set; }

        ///<summary>Gets/sets whether manager approval is required.</summary>
        [DisplayName("Manager Approval Required?")]
        public bool RequireManagerApproval { get; set; }

        [DisplayName("ES11 Number")]
        public string LogNumber { get; set; }
        
        //not for display
        public string UserId { get; set; }
    }
}
