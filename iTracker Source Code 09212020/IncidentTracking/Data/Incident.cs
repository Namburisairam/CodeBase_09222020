using System;
using System.ComponentModel.DataAnnotations;

/// <remarks>
/// ================================================================================
/// MODULE:  IncidentTrackingRepository.cs
///         
/// PURPOSE:
/// Together these classes represent the data acquisition repository.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-05-28
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-05-28  Initial version
/// Brad Robbins    2019-07-02  Added estimated downtime
/// Brad Robbins    2020-05-13  Added short description
/// Brad Robbins    2020-06-02  Replaced RootCause with Classification
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Data
{
    ///<summary>Repesents a single incident with limited data.</summary>
    public class Incident
    {
        ///<summary>Gets/sets the primary key</summary>
        public int IncidentId { get; set; }
        ///<summary>Gets/sets the incident type</summary>
        public string Type  { get; set; }
        ///<summary>Gets/sets the time the activity was performed</summary>
        public DateTime ActivityPerformedDateTime { get; set; }
        ///<summary>Gets/sets a formatted version of the date</summary>
        public string FormattedActivityPerformedDateTime { get; set; }
        ///<summary>Gets/sets the area name</summary>
        public string Area { get; set; }
        ///<summary>Gets/sets the root cause</summary>
        public string Classification { get; set; }
        ///<summary>Gets/sets the engineer name</summary>
        public string EngineerName { get; set; }
        ///<summary>Gets/sets the short description</summary>
        public string ShortDescription { get; set; }
        ///<summary>Gets/sets the estimated downtime</summary>
        [DisplayFormat(DataFormatString="{0:F2}")]
        public Single EstimatedDowntime { get; set; }
    }
}