using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <remarks>
/// ================================================================================
/// MODULE:  T3Group.cs
///         
/// PURPOSE:
/// This class represents a single Tier3 group and limits the selection values 
/// for groups within manufacturing areas.
///         
/// Copyright:    ©2020 by E2i, Inc.
/// Created Date: 2020-04-29
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2020-04-29  Initial version
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    ///<summary>Represents a single Tier3 group object.</summary>
    public class T3Group
    {
        ///<summary>Gets/sets the T3Group identifier</summary>
        [Key]
        public int T3GroupId { get; set; }

        ///<summary>Gets/sets the group name</summary>
        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(50, ErrorMessage = "Group name cannot be more than 50 characters.")]
        public string Name { get; set; }

        ///<summary>Gets/sets the SME name.</summary>
        [DisplayName("SME Name")]
        [Required(ErrorMessage = "SME name is required.")]
        [StringLength(50, ErrorMessage = "SME name cannot be more than 50 characters.")]
        public string SmeName { get; set; }

        ///<summary>Gets/sets the SME email address.</summary>
        [DisplayName("SME Email Address")]
        [Required(ErrorMessage = "SME email address is required.")]
        [StringLength(100, ErrorMessage = "SME email address cannot be more than 100 characters.")]
        public string SmeEmailAddress { get; set; }

        ///<summary>Gets/sets the ReplyTo email address.</summary>
        [DisplayName("ReplyTo Email Address")]
        [Required(ErrorMessage = "ReplyTo email address is required.")]
        [StringLength(100, ErrorMessage = "ReplyTo email address cannot be more than 100 characters.")]
        public string ReplyToEmailAddress { get; set; }

        #region EF Navigation Fields
        ///<summary>Gets/sets the related manufacturing areas.</summary>
        public ICollection<ManufacturingArea> ManufacturingAreas { get; set; }
        #endregion EF Navigation Fields
    }
}
