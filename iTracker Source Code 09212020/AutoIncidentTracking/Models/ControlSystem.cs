using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <remarks>
/// ================================================================================
/// MODULE:  ControlSystem.cs
///         
/// PURPOSE:
/// This class represents a single control system object and limits the selection
/// values for control systems within incidents.
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-09-24
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-09-24  Initial version
/// Brad Robbins    2020-04-10  Added IsObsolete column
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    ///<summary>Represents a single control system object.</summary>
    public class ControlSystem
    {
        ///<summary>Gets/sets the control system identifier.</summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ControlSystemId { get; set; }

        [DisplayName("Manufacturing Area")]
        ///<summary>Gets/sets the control system's manufacturing identitifier.</summary>
        public int ManufacturingAreaId { get; set; }
        
        ///<summary>Gets/sets the control system's name.</summary>
        [Required(ErrorMessage = "Control system name is required.")]
        public string Name { get; set; }

        ///<summary>Gets/sets the control system's description.</summary>
        public string Description { get; set; }

        ///<summary>Gets/sets the PLC identifier.</summary>
        [DisplayName("PLC ID")]
        public string PlcId { get; set; }

        ///<summary>Gets/sets the physical location.</summary>
        [DisplayName("Physical Location")]
        public string PhysicalLocation { get; set; }

        ///<summary>Gets/sets whether the device is networked.</summary>
        [DisplayName("Is Networked?")]
        public string IsNetworked { get; set; }

        ///<summary>Gets/sets the IP address.</summary>
        [DisplayName("IP Address")]
        public string IpAddress { get; set; }

        ///<summary>Gets/sets the GDRS location.</summary>
        [DisplayName("GDRS Location")]
        public string GdrsLocation { get; set; }

        ///<summary>Gets/sets the make/brand.</summary>
        [DisplayName("Make/Brand")]
        public string Make { get; set; }

        ///<summary>Gets/sets the model number.</summary>
        [DisplayName("Model Number")]
        public string Model { get; set; }

        ///<summary>Gets/sets the firmware number.</summary>
        [DisplayName("Firmware Number")]
        public string Firmware { get; set; }

        ///<summary>Gets/sets whether the device is in Autosave.</summary>
        [DisplayName("In Autosave?")]
        public bool? IsInAutosave { get; set; }
        
        ///<summary>Gets/sets whether the device is obsolete.</summary>
        [DisplayName("Obsolete?")]
        public bool? IsObsolete { get; set; }

        #region Additional Properties
        ///<summary>Gets the control system description.</summary>
        [NotMapped]
        [Display(Name = "Control Sys/Software")]
        public string ControlSystemDescription
        {
            get
            {
                if (String.IsNullOrEmpty(Description))
                    return Name;
                else
                    return Name + " / " + Description;
            }
        }
        #endregion Additional Properties

        #region EF Navigation Fields
        ///<summary>Gets/sets the related manufacturing area.</summary>
        public ManufacturingArea ManufacturingArea { get; set; }
        #endregion EF Navigation Fields
    }
}
