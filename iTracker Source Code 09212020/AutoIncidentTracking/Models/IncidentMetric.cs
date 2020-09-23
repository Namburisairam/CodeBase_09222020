/// <remarks>
/// ================================================================================
/// MODULE:  IncidentMetric.cs
///         
/// PURPOSE:
/// This class represents simple data about incidents.  This is used by the Home page.
///         
/// Copyright:    ©2020 by E2i, Inc.
/// Created Date: 2020-06-11
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2020-06-11  Initial version
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    public class IncidentMetric
    {
        ///<summary>Manufacturing Area name</summary>
        public string ManufacturingArea { get; set; }

        ///<summary>Incident type</summary>
        public string Type { get; set; }

        ///<summary>Shift when incident occurred</summary>
        public int Shift { get; set; }

        ///<summary>Estimated down time (hours)</summary>
        public float EstimatedDownTimeHours { get; set; }    
    }
}