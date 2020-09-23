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
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2019-05-28  Added report label
/// ================================================================================
/// </remarks>


namespace ES13Web.Models
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

        ///<summary>Gets/sets the report label.</summary>
        [DisplayName("Report Label")]
        [Required(ErrorMessage = "Report label is required.")]
        public string ReportLabel { get; set; }
    }
}
