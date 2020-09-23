using System.ComponentModel;

/// <remarks>
/// ================================================================================
/// MODULE:  ControlSystemIndexViewModel.cs
///         
/// PURPOSE:
/// This class represents a single control system object for the index view.
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-11-02
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-11-02  Initial version
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    ///<summary>Represents a single control system object for the index view.</summary>
    public struct ControlSystemIndexViewModel
    {
        ///<summary>Gets/sets the control system identifier.</summary>
        public int ControlSystemId { get; set; }

        ///<summary>Gets/sets the manufacturing area name.</summary>
        [DisplayName("Manufacturing Area")]
        public string ManufacturingAreaName { get; set; }
        
        ///<summary>Gets/sets the control system's name.</summary>
        [DisplayName("Control System Name")]
        public string Name { get; set; }

        ///<summary>Gets/sets the control system's description.</summary>
        public string Description { get; set; }
    }
}
