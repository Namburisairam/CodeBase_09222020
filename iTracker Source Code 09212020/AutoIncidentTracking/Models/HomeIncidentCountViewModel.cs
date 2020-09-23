/// <remarks>
/// ================================================================================
/// MODULE:  HomeIncidentCountViewModel.cs
///
/// PURPOSE:
/// This simple class is used for the incident summary on the Home page.
///
/// COPYRIGHT:    ©2020 by E2i, Inc.
/// CREATED DATE: 2020-06-12
/// AUTHOR:       Brad Robbins (brobbins@e2i.net)
///
/// --------------------------------------------------------------------------------
/// REVISION HISTORY:
/// AUTHOR		DATE		DESCRIPTION
/// B.Robbins	2020-06-12	Initial version
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Models
{
    public class HomeIncidentCountViewModel
    {
        public string Type { get; set; }
        public int First { get; set; }
        public int Second { get; set; }
        public int Third { get; set; }
        public int Total { get; set; }
    }
}
