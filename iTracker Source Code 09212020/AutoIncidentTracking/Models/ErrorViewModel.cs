/// <remarks>
/// ================================================================================
/// MODULE:  ControlSystemIndexViewModel.cs
///         
/// PURPOSE:
/// This view model class represents a single error and is part of the 
/// MVC framework.
///         
/// Copyright:    �2018 by E2i, Inc.
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
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Message { get; set; }
    }
}