
/// <remarks>
/// ================================================================================
/// MODULE:  ManufacturingAreaIndexViewModel.cs
///         
/// PURPOSE:
/// This class represents a single manufacturing area object for the index view.
///         
/// Copyright:    ©2020 by E2i, Inc.
/// Created Date: 2020-06-12
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2020-06-12  Initial version
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    /// <summary>Represents a single manufacturing area object for the index view.</summary>
    public class ManufacturingAreaIndexViewModel
    {
        /// <summary>Get/sets the manufacturing area identifier</summary>
        public int ManufacturingAreaId { get; set; }

        /// <summary>Get/sets the manufacturing area name</summary>
        public string Name { get; set; }

        /// <summary>Get/sets the manufacturing area description</summary>
        public string Description { get; set; }
    }
}