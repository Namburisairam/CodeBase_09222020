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
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-11-06
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-11-06  Initial version
/// Brad Robbins    2019-05-02  Changed from creation to activity performed date/time.
/// Brad Robbins    2019-12-06  Added hidden UserId field
/// Brad Robbins    2020-05-13  Added short description; removed control system description
/// Brad Robbins    2020-06-02  Removed "root cause" in favor of classification
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    ///<summary>This class represents a single incident search result.summary>
    public struct IncidentSearchResultViewModel
    {
        /// <summary>Gets/sets the incident identifier.</summary>
        public int IncidentId { get; set; }

        ///<summary>Gets/sets the activity performed date.</summary>
        [DataType(DataType.DateTime)]
        public DateTime ActivityPerformedDateTime { get; set; }

        ///<summary>Gets/sets the formatted activity performed date.</summary>
        [Display(Name = "Activity Performed Date Time")]
        [DataType(DataType.DateTime)]
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

        [DisplayName("ES11 Number")]
        public string LogNumber { get; set; }
        
        //hidden field
        public string UserId { get; set;}
    }
}
