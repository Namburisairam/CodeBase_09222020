using IncidentTracking.Data;
using IncidentTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;


/// <remarks>
/// ================================================================================
/// MODULE:  HomeControllers.cs
///         
/// PURPOSE:
/// Handles all actions for the Home controller.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-05-28
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-05-28  Initial version
/// Brad Robbins    2019-07-02  Added estimated downtime to query
/// Brad Robbins    2020-05-13  Added short description to query
/// Brad Robbins    2020-06-02  Replaced RootCause with Classification
/// Brad Robbins    2020-06-10  Updated to use FromSqlRaw because of .NET Core 3.1 upgrade
/// ================================================================================

namespace IncidentTracking.Controllers
{
    public class HomeController : Controller, IDisposable
    {
        /// <summary>EF database context</summary>
        private readonly ApplicationDbContext _db;

        /// <summary>appsettings.json configuration</summary>
        private readonly IConfiguration _configuration;

        /// <summary>AppSettings element within configuration</summary>
        private readonly IConfigurationSection _settings;

        /// <summary>Default number of objects to show in tabular lists</summary>
        private readonly int _pageSize;

        /// <summary>Default date format</summary>
        private readonly string _dateFormat;

        public HomeController(
            ApplicationDbContext context, 
            IConfiguration config)
        {
            _db = context;
            _configuration = config;
            _settings = _configuration.GetSection("AppSettings");

            _pageSize = _settings.GetValue<int>("DefaultPageSize");
            _dateFormat = _settings.GetValue<string>("DateFormat");
        }

        public async Task<IActionResult> Index()
        {
            //setup the SQL query
            var query = $@"
                SELECT TOP {_pageSize} 
                      [IncidentId]
                    , [Type]
                    , [ActivityPerformedDateTime]
                    , FORMAT([ActivityPerformedDateTime], '{_dateFormat}', 'en-US') [FormattedActivityPerformedDateTime]
                    , [Area]
                    , [Classification]
                    , [EngineerName]
                    , [ShortDescription]
                    , [EstimatedDowntime]
                FROM 
                (
                    SELECT TOP {_pageSize} 
                          i.IncidentId
                        , 'PCS' [Type]
                        , i.LocalActivityPerformedDateTime ActivityPerformedDateTime
                        , m.[Name] Area
                        , c.[Name] Classification
                        , i.EngineerName
                        , i.ShortDescription
                        , i.EstimatedDownTimeHours [EstimatedDowntime]
                    FROM AUTO_IncidentTrack.dbo.Incidents i
                    INNER JOIN AUTO_IncidentTrack.dbo.ManufacturingAreas m ON i.ManufacturingAreaId = m.ManufacturingAreaId
                    INNER JOIN AUTO_IncidentTrack.dbo.Classifications c ON i.ClassificationId = c.ClassificationId
                    ORDER BY i.LocalActivityPerformedDateTime Desc, m.[Name], i.EngineerName
                    UNION
                    SELECT TOP {_pageSize}
                          i.IncidentId
                        , 'MES' [Type]
                        , i.LocalActivityPerformedDateTime ActivityPerformedDateTime
                        , m.[Name] Area
                        , c.[Name] Classification
                        , i.EngineerName
                        , i.ShortDescription
                        , i.EstimatedDownTimeHours [EstimatedDowntime]
                    FROM MES_IncidentTrack.dbo.Incidents i
                    INNER JOIN MES_IncidentTrack.dbo.ManufacturingAreas m ON i.ManufacturingAreaId = m.ManufacturingAreaId
                    INNER JOIN MES_IncidentTrack.dbo.Classifications c ON i.ClassificationId = c.ClassificationId
                    ORDER BY i.LocalActivityPerformedDateTime Desc, m.[Name], i.EngineerName
                ) q
                ORDER BY ActivityPerformedDateTime Desc, Area, EngineerName";

            var list = await _db.Incidents.FromSqlRaw(query).ToListAsync();

            return View(list);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
