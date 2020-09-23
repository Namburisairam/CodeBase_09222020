/// <remarks>
/// ================================================================================
/// MODULE:  ControlSystemIndexViewModel.cs
///         
/// PURPOSE:
/// This view model class represents a single error and is part of the 
/// MVC framework.
///         
/// Copyright:    (c)2018 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// ================================================================================
/// </remarks>

namespace ES13Web.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Message { get; set; }
    }
}