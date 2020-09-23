using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <remarks>
/// ================================================================================
/// MODULE:  ManufacturingArea.cs
///         
/// PURPOSE:
/// This class represents a single manufacturing area object.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins	2019-05-24  Added report label
/// Brad Robbins    2019-09-24  Added SME and Mgr fields
/// ================================================================================
/// </remarks>

namespace ES13Web.Models
{
    ///<summary>Represents a single manufacturing area object.</summary>
    public class ManufacturingArea
    {
        ///<summary>Gets/sets the control system identifier.</summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [DisplayName("Manufacturing Area ID")]
        [Required(ErrorMessage = "Manufacturing Area ID is required.")]
        public int ManufacturingAreaId { get; set; }

        ///<summary>Gets/sets the manufacturing area's name.</summary>
        [DisplayName("Manufacturing Area Name")]
        [Required(ErrorMessage = "Manufacturing area name is required.")]
        public string Name { get; set; }

        ///<summary>Gets/sets the manufacturing area's name.</summary>
        [DisplayName("Report Label")]
        [Required(ErrorMessage = "Report label is required.")]
        public string ReportLabel { get; set; }

        ///<summary>Gets/sets the SME name.</summary>
        [DisplayName("SME Name")]
        [Required(ErrorMessage = "SME name is required.")]
        [StringLength(50, ErrorMessage = "SME name cannot be more than 50 characters.")]
        public string SmeName { get; set; }

        ///<summary>Gets/sets the SME email address.</summary>
        [DisplayName("SME Email Address")]
        [Required(ErrorMessage = "SME email address is required.")]
        [StringLength(50, ErrorMessage = "SME email address cannot be more than 100 characters.")]
        public string SmeEmailAddress { get; set; }

        ///<summary>Gets/sets the manager name.</summary>
        [DisplayName("Manager Name")]
        [Required(ErrorMessage = "Manager name is required.")]
        [StringLength(50, ErrorMessage = "Manager name cannot be more than 50 characters.")]
        public string MgrName { get; set; }

        ///<summary>Gets/sets the manager email address.</summary>
        [DisplayName("Manager Email Address")]
        [Required(ErrorMessage = "Manager email address is required.")]
        [StringLength(50, ErrorMessage = "Manager email address cannot be more than 100 characters.")]
        public string MgrEmailAddress { get; set; }

        ///<summary>Gets/sets the ReplyTo email address.</summary>
        [DisplayName("ReplyTo Email Address")]
        [Required(ErrorMessage = "ReplyTo email address is required.")]
        [StringLength(50, ErrorMessage = "ReplyTo email address cannot be more than 100 characters.")]
        public string ReplyToEmailAddress { get; set; }
    }
}
