using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <remarks>
/// ================================================================================
/// MODULE:  Incident.cs
///         
/// PURPOSE:
/// This class represents a single incident.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2019-05-24  Increased size of IncidentDescription and ActionSummary
/// Brad Robbins    2019-06-03  Increased size of IncidentDescription and ActionSummary
/// Brad Robbins    2019-08-29  Added engineering time
/// Brad Robbins    2019-09-11  Changed display name for RequireES13 to Intervention Performed; made AreaContact required; added BatchNumber
/// Brad Robbins    2019-12-03  Added user ID field
/// Brad Robbins    2020-03-18  Added escalation field
/// Brad Robbins    2020-05-13  Added short description field
/// Brad Robbins    2020-06-02  Removed "root cause" in favor of classification
/// Brad Robbins    2020-06-10  Added Type and Shift properties
/// Brad Robbins    2020-06-15  Added RequireForNewAttribute
/// ================================================================================
/// </remarks>

namespace ES13Web.Models
{
    public class RestrictedDate : ValidationAttribute
    {
        public override bool IsValid(object dt)
        {
            DateTime date = (DateTime)dt;
            return date < DateTime.Now;
        }
    }

    ///<summary>Custom attribute to ensure item is required.</summary>
    public class RequireForNewAttribute : ValidationAttribute
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

        ///<summary>Gets/sets the short description.</summary>
        [DisplayName("Short Description")]
        [RequireForNew(ErrorMessage = "Short description is required.")]
        [StringLength(100, ErrorMessage = "Short description cannot be more than 100 characters.")]
        public string ShortDescription { get; set; }

        ///<summary>Gets/sets the manufacturing area identifier.</summary>
        [DisplayName("Manufacturing Area")]
        [Required(ErrorMessage = "Manufacturing area is required.")]
        public int ManufacturingAreaId { get; set; }

        /// <summary>Gets/sets the area contact.</summary>
        [DisplayName("Area Contact")]
        [RequireForNew(ErrorMessage = "Area contact is required.")]
        [StringLength(50, ErrorMessage = "Area contact cannot be more than 50 characters.")]
        public string AreaContact { get; set; }

        /// <summary>Gets/sets the estimated down time (hrs)</summary>
        [DisplayName("Est. Downtime (hours)")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [DefaultValue(0f)]
        [Range(0, Single.MaxValue, ErrorMessage = "Invalid downtime value.")]
        public float EstimatedDownTimeHours { get; set; }

        /// <summary>Gets/sets the engineering time (hrs)</summary>
        [DisplayName("Engineering Time (hours)")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [DefaultValue(0f)]
        [Range(0, Single.MaxValue, ErrorMessage = "Invalid engineering time value.")]
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

        ///<summary>Gets/sets the last modification date.</summary>
        [DisplayName("Last Modified Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime LastModifiedDateTime { get; set; }


        ///<summary>Gets/sets the last modification date.</summary>
        [DisplayName("Local Last Modified Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime LocalLastModifiedDateTime { get; set; }

        ///<summary>Gets/sets whether an intervention was required.</summary>
        [DisplayName("Intervention Performed?")]
        public bool? RequireES13 { get; set; }
        
        ///<summary>Gets/sets the Werum ticket number.</summary>
        [DisplayName("Werum Ticket Number")]
        [StringLength(50, ErrorMessage = "Werum Ticket Number cannot be longer than 50 characters.")]
        public string WerumTicket { get; set; }

        ///<summary>Gets/sets whether Werum Ticket was resolved.</summary>
        [DisplayName("Werum Ticket Resolved?")]
        public bool? WerumTicketResolved { get; set; }
        
        ///<summary>Gets/sets the Werum Ticket Summary.</summary>
        [DisplayName("Werum Ticket Summary")]
        [StringLength(500, ErrorMessage = "Werum Ticket Summary cannot be longer than 500 characters.")]
        public string WerumTicketSummary { get; set; }

        ///<summary>Gets/sets the log number.</summary>
        [DisplayName("Associated ES-11 Log Number")]
        public string ES11LogNumber { get; set; }
        
        ///<summary>Gets/sets the BT service request number if used.</summary>
        [DisplayName("BT Service Request Number")]
        public string BTServiceRequestNumber { get; set; }

        ///<summary>Gets/sets BT service request summary.</summary>
        [DisplayName("BT Service Request Summary")]
        public string BTServiceRequestSummary { get; set; }

        [DisplayName("Batch Number")]
        [RequireForNew(ErrorMessage = "Batch number is required.")]
        [StringLength(20, ErrorMessage = "Batch number cannot be longer than 20 characters.")]
        public string BatchNumber { get; set; }

        /// <summary>Gets/sets the user ID</summary>
        public string UserId { get; set; }

        [StringLength(50, ErrorMessage = "Escalation cannot be longer than 50 characters.")]
        public string Escalation { get; set; }

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

        /// <summary>Gets whether the record includes a Werum ticket number</summary>
        [NotMapped]
        public bool HasWerumTicket { get => !String.IsNullOrEmpty(this.WerumTicket); }

        [NotMapped]
        public string Type { get => RequireES13.HasValue && RequireES13.Value ? "Intervention" : "Incident"; }

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
        ///<summary>Gets/sets the related manufacturing area.</summary>
        public ManufacturingArea ManufacturingArea { get; set; }

        /// <summary>Gets/sets the related classification.</summary>
        public Classification Classification { get; set; }

        #endregion EF Navigation Fields
    }
}
