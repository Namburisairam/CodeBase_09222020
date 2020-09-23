/// <remarks>
/// ================================================================================
/// MODULE:  HomeES11PercentViewModel.cs
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
    public class HomeES11PercentViewModel
    {
        public string Type { get; set; }
        public double First { get; set; }
        public double Second { get; set; }
        public double Third { get; set; }
        public double Total { get; set; }
    }
}
