/// <remarks>
/// ================================================================================
/// MODULE:  HomeIncidentCountViewModel.cs
///         
/// PURPOSE:
/// This class represents a set of incident-related data for use with the home page.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-29
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-29  Initial version
/// ================================================================================
/// </remarks>

namespace ES13Web.Models
{
    /// <summary>
    /// This class represents a set of incident-related data for use with the home page.
    /// </summary>
    public class HomeIncidentCountViewModel
    {
        /// <summary>Incident type</summary>
        public string Type { get; set; }

        /// <summary>First shift data</summary>
        public int First { get; set; }

        /// <summary>Second shift data</summary>
        public int Second { get; set; }

        /// <summary>Third shift data</summary>
        public int Third { get; set; }

        /// <summary>All shifts data</summary>
        public int Total { get; set; }
    }
}
