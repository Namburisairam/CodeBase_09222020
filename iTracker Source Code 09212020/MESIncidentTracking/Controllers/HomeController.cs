using ES13Web;
using ES13Web.Controllers;
using ES13Web.Data;
using ES13Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

/// <remarks>
/// ================================================================================
/// MODULE:  HomeController.cs
///         
/// PURPOSE:
/// This class is responsible for handling all static views (user displays).
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-10-30
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-10-30  Initial version
/// Brad Robbins    2019-06-03  Updated Index() to use LocalActivityPerformedDateTime
/// Brad Robbins    2020-06-10  Updated Index queries; removed unused report content
/// Brad Robbins    2020-06-15  Added logging
/// ================================================================================
/// </remarks>

namespace ES13Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ApplicationDbContext context, IConfiguration config, ILogger<HomeController> logger)
            : base(context, config, logger){}

        public async Task<IActionResult> Index()
        {
            Logger.LogTrace("Home/Index called");
            var query = @"
                SELECT 
                    m.Name [ManufacturingArea], 
                    CASE WHEN i.RequireES13 = 1 THEN 'Intervention' ELSE 'Incident' END [Type],
                    CASE WHEN DATEPART(HOUR, i.LocalActivityPerformedDateTime) < 7 THEN 3
                        WHEN DATEPART(HOUR, i.LocalActivityPerformedDateTime) < 15 THEN 1
                        WHEN DATEPART(HOUR, i.LocalActivityPerformedDateTime) < 23 THEN 2
                        ELSE 3 END [Shift],
                    i.EstimatedDownTimeHours
                FROM dbo.Incidents i
                INNER JOIN dbo.ManufacturingAreas m ON m.ManufacturingAreaId = i.ManufacturingAreaId
                WHERE i.LocalActivityPerformedDateTime >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE))";

            //How incidents (total) last 30 days?
            var incidentsLast30Days = await DatabaseContext.IncidentMetrics
                .FromSqlRaw(query)
                .ToListAsync();

            //How ES13s last 30 days?
            var interventionsLast30Days = incidentsLast30Days
                .Where(i => i.Type == "Intervention")
                .Count();
            Logger.LogTrace("interventionsLast30Days = {0}", interventionsLast30Days);

            float downtimeLast30Days = 0f;
            if (incidentsLast30Days.Count > 0)
            {
                //What is average downtime last 30 days?
                downtimeLast30Days =  incidentsLast30Days.Average(i => i.EstimatedDownTimeHours);
                Logger.LogTrace("downtimeLast30Days = {downtimeLast30Days}", downtimeLast30Days);

                //Which area has the most incidents last 30 days?
                var mostIncidents = (
                        from i in incidentsLast30Days
                        group i by i.ManufacturingArea into g
                        select new { ManufacturingArea = g.Key, Count = g.Count() }
                    ).OrderByDescending(i => i.Count)
                    .FirstOrDefault();

                string mostIncidentsLast30Days = $"{mostIncidents.ManufacturingArea} with {mostIncidents.Count}";
                Logger.LogTrace("mostIncidentsLast30Days = {mostIncidentsLast30Days}", mostIncidentsLast30Days);

                //get a count of incidents by type and shift
                var incidentsByShift =
                    from i in incidentsLast30Days
                    group i by i.Type into g
                    select new HomeIncidentCountViewModel
                    {
                        Type   = g.Key,
                        First  = g.Count(i => i.Shift == 1),
                        Second = g.Count(i => i.Shift == 2),
                        Third  = g.Count(i => i.Shift == 3),
                        Total  = g.Count()
                    };

                //now calculate percentage of ES13s for each shift
                var percentInterventionsByShift =
                    from r in incidentsByShift
                    where r.Type == "Intervention"
                    select new HomeES13PercentViewModel
                    {
                        Type = "Percentages",
                        First  = r.First == 0  ? 0 : Math.Round(r.First  * 100.0 / incidentsByShift.Sum(s => s.First), 0),
                        Second = r.Second == 0 ? 0 : Math.Round(r.Second * 100.0 / incidentsByShift.Sum(s => s.Second), 0),
                        Third  = r.Third == 0  ? 0 : Math.Round(r.Third  * 100.0 / incidentsByShift.Sum(s => s.Third), 0),
                        Total  = r.Total == 0  ? 0 : Math.Round(r.Total  * 100.0 / incidentsByShift.Sum(s => s.Total), 0)
                    };

                
                
                ViewData["MostIncidentsLast30Days"] = mostIncidentsLast30Days;
                ViewData["IncidentsByShift"] = incidentsByShift.ToList();
                ViewData["PercentInterventionsByShift"] = percentInterventionsByShift.ToList();
            }
            else
            {
                ViewData["MostIncidentsLast30Days"] = "N/A";
                ViewData["IncidentsByShift"] = new List<HomeIncidentCountViewModel>
                {   new HomeIncidentCountViewModel
                    {
                        Type = "Total",
                        First = 0,
                        Second = 0,
                        Third = 0,
                        Total = 0
                    }
                };
                ViewData["PercentInterventionsByShift"] = new List<HomeES13PercentViewModel>
                {
                    new HomeES13PercentViewModel
                    {
                        Type = "Percentages",
                        First  = 0,
                        Second = 0,
                        Third  = 0,
                        Total  = 0
                    }
                };
                
            }
            //transfer the data
            ViewData["IncidentsLast30Days"] = incidentsLast30Days.Count;
            ViewData["InterventionsLast30Days"] = interventionsLast30Days;
            ViewData["DowntimeLast30Days"] = Convert.ToSingle(Math.Round(downtimeLast30Days, 1));
            
            return View();
        }

        //[Authorize(Roles = "cslg1.cslg.net\\KANBPL_APP_ENG_ES11_Admin")]
        public IActionResult Manage() => View();
        
        public IActionResult About() => View();

        public IActionResult Contact() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() 
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var err = exceptionHandlerPathFeature?.Error;
            var vm = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = err.Message
            };
            Logger.LogError("Error occured.  RequestId = {0}, Message = {1}", vm.RequestId, vm.Message);
            return View(vm);
        }
    }
}
