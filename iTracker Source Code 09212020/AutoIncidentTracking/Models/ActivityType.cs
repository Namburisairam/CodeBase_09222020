using System.ComponentModel.DataAnnotations;

/// <remarks>
/// ================================================================================
/// MODULE:  ActivityType.cs
///         
/// PURPOSE:
/// This class represents a single activity type and limits the selection values 
/// for Activity Types for incidents.
/// ///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-10-18
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-10-18  Initial version
/// Brad Robbins    2020-03-23  Added ordinal field
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    public class ActivityType
    {
        //<summary>Gets/sets the activity type identifier</summary>
        [Key]
        public char ActivityTypeId { get; set; }

        //<summary>Gets/sets the activity type description</summary>
        public string Description { get; set; }
        
        //<summary>Gets/sets the ordinal</summary>
        public byte Ordinal { get; set; }
    }
}
