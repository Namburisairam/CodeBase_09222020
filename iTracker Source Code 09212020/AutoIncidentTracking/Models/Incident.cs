using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <remarks>
/// ================================================================================
/// MODULE:  IncidentLog.cs
///         
/// PURPOSE:
/// This class represents a single incident.
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-09-24
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-10-31  Initial version
/// Brad Robbins    2019-05-01  Added BT Service Request fields; added LocalCreatedDateTime formatting
/// Brad Robbins    2019-05-24  Increased size of IncidentDescription and ActionSummary
/// Brad Robbins    2019-06-03  Increased size of IncidentDescription and ActionSummary
/// Brad Robbins    2019-08-29  Added engineering time
/// Brad Robbins    2019-12-06  Added user ID field
/// Brad Robbins    2020-03-18  Added escalation field
/// Brad Robbins    2020-04-02  Removed Software Revison field
/// Brad Robbins    2020-05-13  Added short description field
/// Brad Robbins    2020-06-02  Renamed root cause to classification
/// Brad Robbins    2020-06-12  Replaced RequiredIf attribute with custom attribute; added Type and Shift properties
/// Brad Robbins    2020-06-15  Replaced Required attribute on ShortDescription with custom attribute
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    ///helper class for date validation
    public class RestrictedDate : ValidationAttribute
    {
        ///Tests if data is before the current time
        public override bool IsValid(object dt)
        {
            DateTime date = (DateTime)dt;
            return date < DateTime.Now;
        }
    }
    
    ///<summary>Custom attribute to ensure activity type is supplied for ES11 incidents.</summary>
    public class RequireActivityTypeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext ctx)
        {
            var incident = (Incident)ctx.ObjectInstance;
            var act = (char?)value;

            if (incident.RequireES11.HasValue && incident.RequireES11.Value && !act.HasValue)
                return new ValidationResult("Activity type is required for ES-11 incidents.");
            else
                return ValidationResult.Success;
        }
    }

    ///<summary>Custom attribute to ensure short description is required.</summary>
    public class RequireShortDescriptionAttribute : ValidationAttribute
    {
        //NOTE:  This method use used, rather the [Required] because of model validation
        //that EF 3.1+ performs during queries.
        public override bool IsValid(object value)
        {
            return value != null && !String.IsNullOrWhiteSpace(value.ToString());
        }
    }

    ///<summary>Represents a single incident log entry.</summary>
    public class Incident
    {
        ///<summary>Gets/sets the log identifier.</summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [DisplayName("Incident ID")]
        public int IncidentId { get; set; }

        ///<summary>Gets/sets the record creation date.</summary>
        [DisplayName("Created Date/Time (UTC)")]
        public DateTime CreatedDateTime { get; set; }

        ///<summary>Gets/sets the record creation date.</summary>
        [DisplayName("Created Date/Time")]
        public DateTime LocalCreatedDateTime { get; set; }

        ///<summary>Gets/sets the date/time of the activity.</summary>
        [DisplayName("Activity Performed Date/Time (UTC)")]
        public DateTime ActivityPerformedDateTime { get; set; }

        ///<summary>Gets/sets the date/time of the activity.</summary>
        [DisplayName("Activity Performed Date/Time")]
        [Required(ErrorMessage = "Activity Performed Date/Time is required.")]
        [RestrictedDate(ErrorMessage = "Activity Performed cannot be a future date/time.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime LocalActivityPerformedDateTime { get; set; }

        ///<summary>Gets/sets the engineer name.</summary>
        [DisplayName("Engineer Name")]
        [Required(ErrorMessage = "Engineer name is required.")]
        [StringLength(50, ErrorMessage = "Engineer name cannot be more than 50 characters.")]
        public string EngineerName { get; set; }

        ///<summary>Gets/sets the manufacturing area identifier.</summary>
        [DisplayName("Manufacturing Area")]
        [Required(ErrorMessage = "Manufacturing area is required.")]
        public int ManufacturingAreaId { get; set; }

        ///<summary>Gets/sets the control system identifier.</summary>
        [DisplayName("Control System")]
        [Required(ErrorMessage = "Control system is required.")]
        public int ControlSystemId { get; set; }

        /// <summary>Gets/sets the short description.</summary>
        [DisplayName("Short Description")]
        [RequireShortDescription(ErrorMessage = "Short description is required.")]
        [StringLength(100, ErrorMessage = "Short description cannot be more than 100 characters.")]
        public string ShortDescription { get; set; }

        /// <summary>Gets/sets the area contact.</summary>
        [DisplayName("Area Contact")]
        [StringLength(50, ErrorMessage = "Area contact cannot be more than 50 characters.")]
        public string AreaContact { get; set; }

        /// <summary>Gets/sets the estimated down time (hrs)</summary>
        [DisplayName("Est. Downtime (hours)")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [DefaultValue(0f)]
        public float EstimatedDownTimeHours { get; set; }

        /// <summary>Gets/sets the engineering time (hrs)</summary>
        [DisplayName("Engineering Time (hours)")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [DefaultValue(0f)]
        public float EngineeringTimeHours { get; set; }

        ///<summary>Gets/sets the incident description.</summary>
        [DisplayName("Incident Description")]
        [Required(ErrorMessage = "Incident description is required.")]
        [StringLength(4000, ErrorMessage = "Incident description cannot be more than 4000 characters.")]
        public string IncidentDescription { get; set; }

        ///<summary>Gets/sets the action summary.</summary>
        [DisplayName("Action Summary")]
        [Required(ErrorMessage = "Action summary is required.")]
        [StringLength(4000, ErrorMessage = "Action summary cannot be more than 4000 characters.")]
        public string ActionSummary { get; set; }

        ///<summary>Gets/sets the related classification.</summary>
        [DisplayName("Classification")]
        [Required(ErrorMessage = "Classification is required.")]
        public int ClassificationId { get; set; }

        ///<summary>Gets/sets whether the incident was resolved.</summary>
        [DisplayName("Incident Resolved?")]
        public bool? Resolved { get; set; }

        /// <summary>Gets/sets a description of any handoff work that was given to others.</summary>
        [DisplayName("Handoff Work")]
        [StringLength(500, ErrorMessage = "Handoff work description cannot be more than 500 characters.")]
        public string HandoffWork { get; set; }
        
        ///<summary>Gets/sets the BT service request number if used.</summary>
        [DisplayName("BT Service Request Number")]
        public string BTServiceRequestNumber { get; set; }

        ///<summary>Gets/sets BT service request summary.</summary>
        [DisplayName("BT Service Request Summary")]
        public string BTServiceRequestSummary { get; set; }

        ///<summary>Gets/sets the last modification date.</summary>
        [DisplayName("Last Modified Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime LastModifiedDateTime { get; set; }


        ///<summary>Gets/sets the last modification date.</summary>
        [DisplayName("Local Last Modified Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime LocalLastModifiedDateTime { get; set; }

        /// <summary>Gets/sets the user ID</summary>
        public string UserId { get; set; }

        ///<summary>Gets/sets the escalation.</summary>
        [Display(Name = "Escalation")]
        [StringLength(50, ErrorMessage = "Escalation cannot be more than 50 characters.")]
        public string Escalation { get; set; }

        #region ES11 Fields
        ///<summary>Gets/sets whether an ES11 was required.</summary>
        [DisplayName("Requires ES-11")]
        public bool? RequireES11 { get; set; }

        ///<summary>Gets/sets the activity type.</summary>
        [DisplayName("Activity Type")]
        [RequireActivityType]
        public char? ActivityTypeId { get; set; }

                
        #region Impacts
        ///<summary>Gets/sets whether safety is impacted.</summary>
        [DisplayName("Impact Safety?")]
        public bool? ImpactSafety { get; set; }

        ///<summary>Gets/sets whether quality is impacted.</summary>
        [DisplayName("Impact Quality?")]
        public bool? ImpactQuality { get; set; }

        ///<summary>Gets/sets whether validation is impacted.</summary>
        [DisplayName("Impact Validation?")]
        public bool? ImpactValidation { get; set; }
        #endregion Impacts

        #region Requirements
        ///<summary>Gets/sets whether pre-approval is required.</summary>
        [DisplayName("Pre-approval Required?")]
        public bool? RequirePreApproval { get; set; }

        ///<summary>Gets/sets whether tagout is required.</summary>
        [DisplayName("Tagout Required?")]
        public bool? RequireTagOut { get; set; }

        ///<summary>Gets/sets whether manager approval is required.</summary>
        [DisplayName("Mgr Approval Required?")]
        public bool? RequireManagerApproval { get; set; }
        #endregion Requirements

        ///<summary>Gets/sets the PR number.</summary>
        [DisplayName("PR Number")]
        [StringLength(20, ErrorMessage = "PR Number cannot be longer than 20 characters.")]
        public string PRNumber { get; set; }

        ///<summary>Gets/sets the tagout number.</summary>
        [DisplayName("TagOut Number")]
        [StringLength(20, ErrorMessage = "Tagout Number cannot be longer than 20 characters.")]
        public string TagOutNumber { get; set; }

        /// <summary>Gets/sets the affected system specifications.</summary>
        [DisplayName("System Specifications")]
        [StringLength(100, ErrorMessage = "System specifications cannot be more than 100 characters.")]
        public string SystemSpecifications { get; set; }

        /// <summary>Gets/sets the affected drawings.</summary>
        [DisplayName("Drawings")]
        [StringLength(100, ErrorMessage = "Drawings cannot be more than 100 characters.")]
        public string Drawings { get; set; }

        ///<summary>Gets/sets the production lot number.</summary>
        [Display(Name = "Mfg Lot Number")]
        public string LotNumber { get; set; }

        ///<summary>Gets/sets whether a program comparison was made.</summary>
        [Display(Name = "Prog. Compare Made?")]
        public bool? ProgramComparisonMade { get; set; }

        ///<summary>Gets/sets whether the activity has been completed.</summary>
        [Display(Name = "Activity Complete?")]
        public bool? ActivityComplete { get; set; }

        ///<summary>Gets/sets the log number.</summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DisplayName("Log Number")]
        public string LogNumber { get; set; }
        #endregion ES11 Fields


        #region Additional Fields
        /// <summary>Gets/sets the format string used for outputting dates as strings</summary>
        [NotMapped]
        public string DateFormat { get; set; }

        /// <summary>Gets/sets the local client date/time at record submission.</summary>
        [NotMapped]
        public string LocalTimeString { get; set; }

        /// <summary>Gets a formatted date/time string for the local creation time</summary>
        [NotMapped]
        [DisplayName("Created Date/Time")]
        public string FormattedLocalCreatedDateTime { get => LocalCreatedDateTime.ToString(DateFormat); }

        /// <summary>Gets a formatted date/time string for the local modification time</summary>
        [NotMapped]
        [DisplayName("Last Modified Date/Time")]
        public string FormattedLocalLastModifiedDateTime { get => LocalLastModifiedDateTime.ToString(DateFormat); }
        
        /// <summary>Gets a formatted date/time string for the activity time</summary>
        [NotMapped]
        [DisplayName("Activity Performed Date/Time")]
        public string FormattedLocalActivityPerformedDateTime { get => LocalActivityPerformedDateTime.ToString(DateFormat); }

        /// <summary>Gets the incident type</summary>
        [NotMapped]
        public string Type { get => RequireES11.HasValue && RequireES11.Value ? "ES11" : "Incident"; }

        /// <summary>Gets the incident shift</summary>
        [NotMapped]
        public int Shift 
        {
            get 
            {
                var hour = LocalActivityPerformedDateTime.Hour;
                return hour < 7 ? 3 :
                       hour < 15 ? 1 :
                       hour < 23 ? 2 : 3;
            }
        }
        #endregion Additional Fields

        #region EF Navigation Fields
        /*NOTE:
         *The following fields are used by EF for connection to related data.
         *They are EF's way of handling foreign keys.
         */
        ///<summary>Gets/sets the related activity type.</summary>
        public ActivityType ActivityType { get; set; }

        ///<summary>Gets/sets the related manufacturing area.</summary>
        public ManufacturingArea ManufacturingArea { get; set; }

        ///<summary>Gets/sets the related control system.</summary>
        public ControlSystem ControlSystem { get; set; }

        /// <summary>Gets/sets the related classification.</summary>
        public Classification Classification { get; set; }

        #endregion EF Navigation Fields
    }
}
