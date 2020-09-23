using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <remarks>
/// ================================================================================
/// MODULE:  Classification.cs
///         
/// PURPOSE:
/// This class represents a single classification and limits the selection values 
/// for classifications within incidents.
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-09-24
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-09-24  Initial version
/// Brad Robbins    2019-05-28  Added report label
/// ================================================================================
/// </remarks>


namespace IncidentTracking.Models
{
    ///<summary>Represents a single classification object.</summary>
    public class Classification
    {
        ///<summary>Gets/sets the classification identifier</summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ClassificationId { get; set; }

        ///<summary>Gets/sets the classification name</summary>
        [Required(ErrorMessage = "Classification name is required.")]
        public string Name { get; set; }

        ///<summary>Gets/sets the classifications's report label.</summary>
        [DisplayName("Report Label")]
        [Required(ErrorMessage = "Report label is required.")]
        public string ReportLabel { get; set; }
    }
}
