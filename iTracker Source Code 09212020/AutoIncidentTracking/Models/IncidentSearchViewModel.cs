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
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-11-06
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-11-06  Initial version
/// Brad Robbins    2020-06-02  Removed "root cause" in favor of classification
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
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

        ///<summary>Gets/sets the control system identifier.</summary>
        [DisplayName("Control System")]
        public int? ControlSystemId { get; set; }
        
        ///<summary>Gets/sets the short description.</summary>
        [DisplayName("Short Description")]
        [StringLength(50, ErrorMessage = "Engineer name cannot be more than 50 characters.")]
        public string ShortDescription { get; set; }
        
        ///<summary>Gets/sets the engineer name.</summary>
        [DisplayName("Engineer Name")]
        [Required(ErrorMessage = "Engineer name is required.")]
        public string EngineerName { get; set; }

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

        ///<summary>Gets/sets the activity type.</summary>
        [DisplayName("Activity Type")]
        public char? ActivityTypeId { get; set; }

        ///<summary>Gets/sets the log number.</summary>
        [DisplayName("Log Number")]
        public string LogNumber { get; set; }

        ///<summary>Gets/sets the result limit.</summary>
        [DisplayName("Result Limit")]
        [DefaultValue(20)]
        public int Limit { get; set; }
    }
}
