using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using IncidentTracking.Data;
using IncidentTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <remarks>
/// ================================================================================
/// MODULE:  IncidentsController.cs
///         
/// PURPOSE:
/// This class represents the incident logs controller which is responsible for 
/// handling all interactions between the IncidentLog model, related view models 
/// and associated views (user displays).
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-09-24
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-10-29  Initial version
/// Brad Robbins    2019-05-01  Added support for LocalCreatedDateTime during creation, BT Service request fields,
/// Brad Robbins    2019-08-19  Commented authorize lines for most functions to allow access to domain users.
/// Brad Robbins    2019-08-29  Added engineering time
/// Brad Robbins    2019-09-16  Revised GetFilteredSearchResults to add one day to end date
/// Brad Robbins    2019-12-06  Added support for hidden UserId field
/// Brad Robbins    2020-03-11  Re-enabled ES-11 features
/// Brad Robbins    2020-03-18  Added escalation
/// Brad Robbins    2020-04-02  Removed software revision field
/// Brad Robbins    2020-04-09  Revised GetDocument to add conditional logic for filling out fields
/// Brad Robbins    2020-04-15  Revised GetControlSystemsAsync to show non-obsolete control systems.
/// Brad Robbins    2020-05-12  Added support for special ES11M attachment to GetDocument()
/// Brad Robbins    2020-05-13  Added short description field
/// Brad Robbins    2020-06-15  Added logging; reworked Index/Search/Export filtering; relocated GetUserFullName to BaseController
/// Brad Robbins    2020-06-22  Fixed editing error that would not allow users to edit their own records
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Controllers
{
    public class IncidentsController : BaseController
    {
        public IncidentsController(ApplicationDbContext context, IConfiguration config, ILogger<IncidentsController> logger)
            : base(context, config, logger){}

        // GET: Incidents
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            Logger.LogTrace("Incidents/Index called with [{sortOrder}], filter [{filter}], search [{search}] and page [{page}]", sortOrder, currentFilter, searchString, page);
            
            //setup sorting, paging, filtering data
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParam"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["LogNumberSortParam"] = sortOrder == "log" ? "log_desc" : "log";
            ViewData["MfgAreaSortParam"] = sortOrder == "mfg" ? "mfg_desc" : "mfg";
            ViewData["ShortDescriptionSortParam"] = sortOrder == "dscptn" ? "dscptn_desc" : "dscptn";
            ViewData["CauseSortParam"] = sortOrder == "cause" ? "cause_desc" : "cause";
            ViewData["EngineerSortParam"] = sortOrder == "engr" ? "engr_desc" : "engr";
            ViewData["PreApprovalSortParam"] = sortOrder == "preapp" ? "preapp_desc" : "preapp";
            ViewData["MgrApprovalSortParam"] = sortOrder == "mgrapp" ? "mgrapp_desc" : "mgrapp";
            ViewData["TagoutSortParam"] = sortOrder == "tagout" ? "tagout_desc" : "tagout";
            
            //apply current filter
            if (String.IsNullOrEmpty(searchString))
                searchString = currentFilter;
            else
                page = 1;

            ViewData["CurrentFilter"] = searchString;

            //Setup query because EF Core 3.1 doesn't like client-side LINQ.
            //It's okay because this is more efficient than LINQ anyway.
            var query = @"
                SELECT 
                    IncidentId = i.IncidentId,
                    ActivityPerformedDateTime = i.ActivityPerformedDateTime,
                    FormattedLocalActivityPerformedDateTime = FORMAT(i.LocalActivityPerformedDateTime, @dateFormat),
                    ManufacturingAreaName = m.[Name],
                    ShortDescription = i.ShortDescription,
                    ClassificationName = c.[Name],
                    EngineerName = i.EngineerName,
                    RequireES11 = CAST(COALESCE(i.RequireES11, 0) AS BIT),
                    LogNumber = i.LogNumber,
                    RequirePreApproval = CAST(COALESCE(i.RequirePreApproval, 0) AS BIT),
                    RequireManagerApproval = CAST(COALESCE(i.RequireManagerApproval, 0) AS BIT),
                    RequireTagOut = CAST(COALESCE(i.RequireTagOut, 0) AS BIT),
                    UserId = i.UserId
                FROM dbo.Incidents i
                INNER JOIN dbo.ManufacturingAreas m ON m.ManufacturingAreaId = i.ManufacturingAreaId
                INNER JOIN dbo.Classifications c ON c.ClassificationId = i.ClassificationId";

            IQueryable<IncidentIndexViewModel> list;
            var dateFormat = new SqlParameter("dateFormat", SqlDbType.NVarChar, 40);
            dateFormat.Value = DateFormat;

            if (!String.IsNullOrEmpty(searchString))
            {
                query += @"
                WHERE @searchString IS NOT NULL AND (i.LogNumber LIKE '%' + @searchString + '%' OR
                    m.Name LIKE '%' + @searchString + '%' OR
                    i.ShortDescription LIKE '%' + @searchString + '%' OR
                    c.Name LIKE '%' + @searchString + '%' OR
                    i.EngineerName LIKE '%' + @searchString + '%') OR @searchString IS NULL";
                var filter = new SqlParameter("searchString", SqlDbType.NVarChar, 50);
                filter.Value = searchString;
                list = DatabaseContext.IncidentLog
                    .FromSqlRaw(query, dateFormat, filter)
                    .AsNoTracking();
            }
            else
            {
                list = DatabaseContext.IncidentLog
                    .FromSqlRaw(query, dateFormat)
                    .AsNoTracking();
            }
                       
            //determine proper sorting
            switch (sortOrder)
            {
                case "date_asc": list = list.OrderBy(i => i.ActivityPerformedDateTime); break;

                case "log": list = list.OrderBy(i => i.LogNumber); break;
                case "log_desc": list = list.OrderByDescending(i => i.LogNumber); break;

                case "dscptn": list = list.OrderBy(i => i.ShortDescription).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "dscptn_desc": list = list.OrderByDescending(i => i.ShortDescription).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "cause": list = list.OrderBy(i => i.ClassificationName).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "cause_desc": list = list.OrderByDescending(i => i.ClassificationName).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "mfg": list = list.OrderBy(i => i.ManufacturingAreaName).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "mfg_desc": list = list.OrderByDescending(i => i.ManufacturingAreaName).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "engr": list = list.OrderBy(i => i.EngineerName).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "engr_desc": list = list.OrderByDescending(i => i.EngineerName).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "preapp": list = list.OrderBy(i => i.RequirePreApproval).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "preapp_desc": list = list.OrderByDescending(i => i.RequirePreApproval).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "mgrapp": list = list.OrderBy(i => i.RequireManagerApproval).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "mgrapp_desc": list = list.OrderByDescending(i => i.RequireManagerApproval).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "tagout": list = list.OrderBy(i => i.RequireTagOut).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "tagout_desc": list = list.OrderByDescending(i => i.RequireTagOut).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                default: list = list.OrderByDescending(i => i.ActivityPerformedDateTime); break;
            }

            var outList = await PaginatedList<IncidentIndexViewModel>
                .CreateAsync(list, page ?? 1, PageSize);
            ViewData["RowCount"] = list.Count();

            return View("Index", outList);
        }

        // GET: Incidents/Search
        public async Task<IActionResult> Search(
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            int? manufacturingAreaId = null,
            string shortDescription = null,
            int? controlSystemId = null,
            int? classificationId = null,
            string engineerName = null,
            string logNumber = null,
            char? activityTypeId = null,
            string incidentDescription = null,
            string actionSummary = null,
            int limit = 20)
        {
            Logger.LogTrace("Index/Search called");
            var data = new IncidentSearchViewModel
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                ManufacturingAreaId = manufacturingAreaId,
                ShortDescription = shortDescription,
                ControlSystemId = controlSystemId,
                ClassificationId = classificationId,
                EngineerName = engineerName,
                LogNumber = logNumber,
                ActivityTypeId = activityTypeId,
                IncidentDescription = incidentDescription,
                ActionSummary = actionSummary,
                Limit = limit
            };

            var list = GetFilteredSearchResults(data);

            if (list != null)
            {
                data.Results = await list
                    .Select(i => new IncidentSearchResultViewModel
                    {
                        IncidentId = i.IncidentId,
                        ActivityPerformedDateTime = i.ActivityPerformedDateTime,
                        FormattedLocalActivityPerformedDateTime = i.LocalActivityPerformedDateTime.ToString(DateFormat),
                        ManufacturingAreaName = i.ManufacturingArea.Name,
                        ShortDescription = i.ShortDescription,
                        ClassificationName = i.Classification.Name,
                        EngineerName = i.EngineerName,
                        RequireES11 = i.RequireES11 ?? false,
                        LogNumber = i.LogNumber,
                        UserId = i.UserId
                    })
                    .OrderByDescending(i => i.ActivityPerformedDateTime)
                    .Take(data.Limit)
                    .ToListAsync();
            }
            else
                data.Results = new List<IncidentSearchResultViewModel>();

            //load select (drop down) lists
            ViewData["ActivityTypeId"] = await GetActivityTypesSelectListAsync();
            ViewData["ClassificationId"] = await GetClassificationsSelectListAsync();
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync();
            ViewData["ControlSystemId"] = await GetControlSystemsSelectListAsync();

            return View("Search", data);
        }

        // GET: Incidents/Export
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES11_Users,CSLG1\\KANBPL_APP_ENG_ES11_Admin")]
        public async Task<IActionResult> Export(
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            int? manufacturingAreaId = null,
            string shortDescription = null,
            int? controlSystemId = null,
            int? classificationId = null,
            string engineerName = null,
            string logNumber = null,
            char? activityTypeId = null,
            string incidentDescription = null,
            string actionSummary = null,
            int limit = 0)
        {
            Logger.LogTrace("Incidents/Export called");
            var data = new IncidentSearchViewModel
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                ManufacturingAreaId = manufacturingAreaId,
                ShortDescription = shortDescription,
                ControlSystemId = controlSystemId,
                ClassificationId = classificationId,
                EngineerName = engineerName,
                LogNumber = logNumber,
                ActivityTypeId = activityTypeId,
                IncidentDescription = incidentDescription,
                ActionSummary = actionSummary,
                Limit = 0
            };

            var list = GetFilteredSearchResults(data);
            var results = list.OrderByDescending(i => i.CreatedDateTime);

            var ms = new MemoryStream(500);
            var writer = new StreamWriter(ms);
            await writer.WriteLineAsync(ExportHeader);

            await list.ForEachAsync(i =>
                writer.WriteLineAsync(IncidentToCsv(i))
            );
            await writer.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "text/csv", "Incidents.csv");
        }

        // GET: Incidents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Logger.LogTrace("Incidents/Details called with [{id}]", id);
            if (!id.HasValue)
                return NotFound();

            var incident = await DatabaseContext.Incidents
                .AsNoTracking()
                .Include(i => i.ManufacturingArea)
                .Include(i => i.ControlSystem)
                .Include(i => i.Classification)
                .FirstOrDefaultAsync(i => i.IncidentId == id);

            if (incident == null)
                return NotFound();

            if (incident.RequireES11.HasValue && incident.RequireES11.Value)
            {
                //this one has ES11; get more information
                incident = await DatabaseContext.Incidents
                    .AsNoTracking()
                    .Include(i => i.ManufacturingArea)
                    .Include(i => i.ControlSystem)
                    .Include(i => i.Classification)
                    .Include(i => i.ActivityType)
                    .FirstAsync(i => i.IncidentId == id);
            }
            
            incident.DateFormat = DateFormat;
            return View(incident);
        }

        // GET: Incidents/Create
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES11_Users,CSLG1\\KANBPL_APP_ENG_ES11_Admin")]
        public async Task<IActionResult> Create()
        {
            Logger.LogTrace("Incidents/Create[GET] called");
            //load select (drop down) lists
            ViewData["ActivityTypeId"] = await GetActivityTypesSelectListAsync();
            ViewData["ClassificationId"] = await GetClassificationsSelectListAsync();
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync();
            ViewData["ControlSystemId"] = await GetControlSystemsSelectListAsync();
            ViewData["Mode"] = "Create";

            var incident = new Incident 
            { 
                EngineerName = GetUserFullName(),
                LocalActivityPerformedDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local)
            };
            return View(incident);
        }

        // POST: Incidents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES11_Users,CSLG1\\KANBPL_APP_ENG_ES11_Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IncidentId,EngineerName,UserId,ShortDescription,ManufacturingAreaId,ControlSystemId,AreaContact,EstimatedDownTimeHours,EngineeringTimeHours,IncidentDescription,ActionSummary,ClassificationId,Resolved,HandoffWork,ActivityTypeId,ImpactSafety,ImpactQuality,ImpactValidation,RequirePreApproval,RequireTagOut,RequireManagerApproval,PRNumber,TagOutNumber,SystemSpecifications,Drawings,LotNumber,ProgramComparisonMade,ActivityComplete,RequireES11,BTServiceRequestNumber,BTServiceRequestSummary,LocalActivityPerformedDateTime,LocalTimeString,Escalation")] Incident incident)
        {
            Logger.LogTrace("Incidents/Create[POST] called");
            if (ModelState.IsValid)
            {
                try
                {
                    var nowUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                    var nowLocal = DateTime.SpecifyKind(DateTime.Parse(incident.LocalTimeString), DateTimeKind.Local);

                    incident.CreatedDateTime = nowUtc;
                    incident.LastModifiedDateTime = nowUtc;

                    incident.LocalCreatedDateTime = nowLocal;
                    incident.LocalLastModifiedDateTime = nowLocal;

                    //ensure proper timezones for date/times of activity
                    var performedLocal = DateTime.SpecifyKind(incident.LocalActivityPerformedDateTime, DateTimeKind.Local);
                    var performedUtc = performedLocal.ToUniversalTime();

                    incident.LocalActivityPerformedDateTime = performedLocal;
                    incident.ActivityPerformedDateTime = performedUtc;

                    //ensure that times are rounded to the nearest 0.25-hour increment
                    float adjustedDowntime = Convert.ToSingle(Math.Round(incident.EstimatedDownTimeHours * 4f, 0) / 4f);
                    incident.EstimatedDownTimeHours = adjustedDowntime;
                    
                    //ensure that times are rounded to the nearest 0.25-hour increment
                    float adjustedEngineeringTime = Convert.ToSingle(Math.Round(incident.EngineeringTimeHours * 4f, 0) / 4f);
                    incident.EngineeringTimeHours = adjustedEngineeringTime;

                    //add the user ID for the engineer creating the record
                    incident.UserId = User.Identity.Name;

                    DatabaseContext.Incidents.Add(incident);
                    await DatabaseContext.SaveChangesAsync();
                    
                    if (incident.RequireES11.HasValue && incident.RequireES11.Value)
                        TempData["Message"] = "Incident has been added.  Remember to submit the ES11 form.";
                    else
                        TempData["Message"] = "Incident has been added.";
                    Logger.LogInformation("{user} added Incident[{id}]", UserName, incident.IncidentId);
                                            
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex)
                {
                    string message = $"Cannot add incident because of a database error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot add incident because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
            }

            //load select (drop down) lists
            ViewData["ActivityTypeId"] = await GetActivityTypesSelectListAsync(
                incident.ActivityTypeId);
            ViewData["ClassificationId"] = await GetClassificationsSelectListAsync(
                incident.ClassificationId);
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync(
                incident.ManufacturingAreaId);

            if (incident.ManufacturingAreaId != 0)
                ViewData["ControlSystemId"] = await GetControlSystemsSelectListAsync(
                    incident.ControlSystemId, incident.ManufacturingAreaId);
            else 
                ViewData["ControlSystemId"] = await GetControlSystemsSelectListAsync(
                    incident.ControlSystemId);
            ViewData["Mode"] = "Create";

            return View(incident);
        }

        // GET: Incidents/Duplicate/4
        //This method creates a copy of an incident, resets its creation date/time
        //to the current time and the engineer name to the current user.  It then
        //opens the new record for final editing.
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES11_Users,CSLG1\\KANBPL_APP_ENG_ES11_Admin")]
        public async Task<IActionResult> Duplicate(int? id, string date)
        {
            Logger.LogTrace("Incidents/Duplicate called with [{id}] and [{date}]", id, date);
            //make sure the record exists
            if (id == null)
                return NotFound();

            var incident = await DatabaseContext.Incidents.FindAsync(id);

            if (incident == null)
                return NotFound();

            //now duplicate it
            try
            {
                var nowUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var nowLocal = DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Local);

                //copy over main information
                var newIncident = new Incident
                {
                    ActionSummary = incident.ActionSummary,
                    ActivityTypeId = incident.ActivityTypeId,
                    AreaContact = incident.AreaContact,
                    ClassificationId = incident.ClassificationId,
                    ControlSystemId = incident.ControlSystemId,
                    IncidentDescription = incident.IncidentDescription,
                    ImpactQuality = incident.ImpactQuality,
                    ImpactSafety = incident.ImpactSafety,
                    ImpactValidation = incident.ImpactValidation,
                    ManufacturingAreaId = incident.ManufacturingAreaId,
                    RequireES11 = incident.RequireES11,
                    RequireManagerApproval = incident.RequireManagerApproval,
                    RequirePreApproval = incident.RequirePreApproval,
                    RequireTagOut = incident.RequireTagOut,
                    ShortDescription = incident.ShortDescription
                };

                //update timing and engineer name
                newIncident.CreatedDateTime = nowUtc;
                newIncident.LastModifiedDateTime = nowUtc;
                newIncident.LocalCreatedDateTime = nowLocal;
                newIncident.LocalLastModifiedDateTime = nowLocal;
                newIncident.ActivityPerformedDateTime = nowUtc;
                newIncident.LocalActivityPerformedDateTime = nowLocal;
                newIncident.EngineerName = GetUserFullName();
                newIncident.UserId = User.Identity.Name;

                DatabaseContext.Incidents.Add(newIncident);
                await DatabaseContext.SaveChangesAsync();

                int newId = newIncident.IncidentId;
                TempData["Message"] = "Incident has been duplicated in the database.  Review entries and resave the record.";
                Logger.LogInformation("{user} duplicated incident [{id}], new ID [{newId}]", UserName, id, newId);
                //open the incident for editing
                return RedirectToAction("Edit", new { id = newId });
            }
            catch (SqlException ex)
            {
                string message = $"Cannot duplicate incident because of a database error: {ex.Message}";
                TempData["Error"] = message;
                Logger.LogError("Error duplicating Incident[{id}] because of {message}", id, message);
                return RedirectToAction("Details", id);
            }
            catch (Exception ex)
            {
                string message = $"Cannot duplicate incident because of a general error: {ex.Message}";
                TempData["Error"] = message;
                Logger.LogError("Error duplicating Incident[{id}] because of {message}", id, message);
                return RedirectToAction("Details", id);
            }
        }

        // GET: Incidents/Edit/5
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES11_Users,CSLG1\\KANBPL_APP_ENG_ES11_Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            Logger.LogTrace("Incidents/Edit[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var incident = await DatabaseContext.Incidents.FindAsync(id);

            if (incident == null)
                return NotFound();

            Logger.LogDebug("UserName: {user}", UserName);
            Logger.LogDebug("Incident.UserId: {id}", incident.UserId);
            Logger.LogDebug("IsAdminUser: {admin}", IsAdminUser());
            if (incident.UserId.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase) || IsAdminUser())
            {
                //load select (drop down) lists
                ViewData["ActivityTypeId"] = await GetActivityTypesSelectListAsync(
                    incident.ActivityTypeId);
                ViewData["ClassificationId"] = await GetClassificationsSelectListAsync(
                    incident.ClassificationId);
                ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync(
                    incident.ManufacturingAreaId);
                ViewData["ControlSystemId"] = await GetControlSystemsSelectListAsync(
                    incident.ControlSystemId, incident.ManufacturingAreaId);
                ViewData["Mode"] = "Edit";
                    
                return View(incident);
            }
            else
            {
                var err = new ApplicationException("Cannot edit incidents created by other users.");
                return BadRequest(err);
            }
        }

        // POST: Incidents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES11_Users,CSLG1\\KANBPL_APP_ENG_ES11_Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IncidentId,CreatedDateTime,LocalCreatedDateTime,ShortDescription,EngineerName,UserId,ManufacturingAreaId,ControlSystemId,AreaContact,EstimatedDownTimeHours,EngineeringTimeHours,IncidentDescription,ActionSummary,ClassificationId,Resolved,HandoffWork,ActivityTypeId,ImpactSafety,ImpactQuality,ImpactValidation,RequirePreApproval,RequireTagOut,RequireManagerApproval,PRNumber,TagOutNumber,SystemSpecifications,Drawings,LotNumber,ProgramComparisonMade,ActivityComplete,RequireES11,BTServiceRequestNumber,BTServiceRequestSummary,LocalActivityPerformedDateTime,LocalTimeString,Escalation")] Incident incident)
        {
            Logger.LogTrace("Incidents/Edit[POST] called with [{id}]", id);
            if (id != incident.IncidentId)
                return NotFound();
            
            if (ModelState.IsValid && (incident.UserId.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase) || IsAdminUser()))
            {
                try
                {
                    var nowUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                    var nowLocal = DateTime.SpecifyKind(DateTime.Parse(incident.LocalTimeString), DateTimeKind.Local);

                    incident.LastModifiedDateTime = nowUtc;
                    incident.LocalLastModifiedDateTime = nowLocal;

                    //ensure proper timezones for date/times of activity
                    var performedLocal = DateTime.SpecifyKind(incident.LocalActivityPerformedDateTime, DateTimeKind.Local);
                    var performedUtc = performedLocal.ToUniversalTime();

                    incident.LocalActivityPerformedDateTime = performedLocal;
                    incident.ActivityPerformedDateTime = performedUtc;

                    //ensure that times are rounded to the nearest 0.25-hour increment
                    float adjustedDowntime = Convert.ToSingle(Math.Round(incident.EstimatedDownTimeHours * 4f, 0) / 4f);
                    incident.EstimatedDownTimeHours = adjustedDowntime;

                    //ensure that times are rounded to the nearest 0.25-hour increment
                    float adjustedEngineeringTime = Convert.ToSingle(Math.Round(incident.EngineeringTimeHours * 4f, 0) / 4f);
                    incident.EngineeringTimeHours = adjustedEngineeringTime;

                    //if the RequireES11 is null       or it is false
                    if (!incident.RequireES11.HasValue || !incident.RequireES11.Value)
                    {
                        //ensure that ES11 fields are "removed"
                        incident.RequireES11 = null;
                        incident.ActivityType = null;
                        incident.ImpactSafety = null;
                        incident.ImpactQuality = null;
                        incident.ImpactValidation = null;
                        incident.RequirePreApproval = null;
                        incident.RequireManagerApproval = null;
                        incident.RequireTagOut = null;
                        incident.PRNumber = null;
                        incident.TagOutNumber = null;
                        incident.SystemSpecifications = null;
                        incident.Drawings = null;
                        incident.LotNumber = null;
                        incident.ActivityComplete = false;
                        incident.ProgramComparisonMade = false;

                    }

                    DatabaseContext.Update(incident);
                    await DatabaseContext.SaveChangesAsync();
                    
                    //did state of ES11 checkbox change to checked?
                    if (incident.RequireES11.HasValue && incident.RequireES11.Value)
                        TempData["Message"] = "Incident has been updated.  Remember to update the ES11 form.";
                    else
                        TempData["Message"] = "Incident has been updated.";
                    Logger.LogInformation("{user} updated Incident[{id}]", UserName, id);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string message;
                    if (!IncidentExists(incident.IncidentId))
                        message = $"Cannot edit incident because it no longer exists.";
                    else
                        message = $"Cannot edit incident because of a concurrency error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating Incident[{id}] because of {message}", id, message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot edit incident because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating Incident[{id}] because of {message}", id, message);
                }
                return RedirectToAction(nameof(Index));
            }
            
            //load select (drop down) lists
            ViewData["ActivityTypeId"] = await GetActivityTypesSelectListAsync(
                incident.ActivityTypeId);
            ViewData["ClassificationId"] = await GetClassificationsSelectListAsync(
                incident.ClassificationId);
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync(
                incident.ManufacturingAreaId);

            if (incident.ManufacturingAreaId != 0)
                ViewData["ControlSystemId"] = await GetControlSystemsSelectListAsync(
                    incident.ControlSystemId, incident.ManufacturingAreaId);
            else 
                ViewData["ControlSystemId"] = await GetControlSystemsSelectListAsync(
                    incident.ControlSystemId);
            ViewData["Mode"] = "Edit";
            
            return View(incident);
        }

     
        // GET: Incidents/Delete/5
        [Authorize(Roles = "cslg1.cslg.net\\KANBPL_APP_ENG_ES11_Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            Logger.LogTrace("Incidents/Delete[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var incident = await DatabaseContext.Incidents
                .AsNoTracking()
                .Include(i => i.ManufacturingArea)
                .Include(i => i.ControlSystem)
                .FirstOrDefaultAsync(m => m.IncidentId == id);
            if (incident == null)
                return NotFound();

            return View(incident);
        }

        // POST: Incidents/Delete/5
        [Authorize(Roles = "cslg1.cslg.net\\KANBPL_APP_ENG_ES11_Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Logger.LogTrace("Incidents/Delete[POST] called with [{id}]", id);
            try
            {
                var incident = await DatabaseContext.Incidents.FindAsync(id);
                DatabaseContext.Incidents.Remove(incident);
                await DatabaseContext.SaveChangesAsync();
                TempData["Message"] = "Incident has been deleted.";
                Logger.LogInformation("{user} deleted Incident[{id}]", UserName, id);
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete an incident that is in use.";
                Logger.LogDebug("Error deleting incident that is in use.");
            }
            catch (SqlException ex)
            {
                TempData["Error"] = $"Cannot delete incident.  A database error occurred: {ex.Message}";
                Logger.LogError("Error deleting incident, SQL error: {message}", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Cannot delete incident.  A general error occurred: {ex.Message}";
                Logger.LogError("Error deleting incident, general error: {message}", ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet] 
        public async Task<IActionResult> GetControlSystemsAsync(int? manufacturingAreaId)
        {
            Logger.LogTrace("Incidents/GetControlSystemsAsync called for ManufacturingArea[{id}]", manufacturingAreaId);
            if (manufacturingAreaId.HasValue)
            {
                var cs = await DatabaseContext
                    .ControlSystems
                    .Where(c => c.ManufacturingAreaId == manufacturingAreaId.Value && !c.IsObsolete.Value)
                    .OrderBy(c => c.Name)
                    .Select(c => new { c.ControlSystemId, c.Name })
                    .ToListAsync();
                return Json(cs);
            }
            else
                return NotFound();
        }

        //helper method to see if incident exists in the database
        private bool IncidentExists(int id) => DatabaseContext.Incidents.Any(e => e.IncidentId == id);

        //helper method to perform search/export filtering
        private IQueryable<Incident> GetFilteredSearchResults(IncidentSearchViewModel data)
        {
            Logger.LogTrace("Incidents/GetFilteredSearchResults called");
            bool filtersApplied = false;

            //setup main query
            IQueryable<Incident> list = DatabaseContext.Incidents
                .AsNoTracking()
                .Include(i => i.ManufacturingArea)
                .Include(i => i.ControlSystem)
                .Include(i => i.Classification)
                .Include(i => i.ActivityType);

            #region Apply Filtering
            if (data.StartDateTime.HasValue)
            {
                list = list.Where(i => i.LocalActivityPerformedDateTime >= data.StartDateTime);
                filtersApplied = true;
            }

            if (data.EndDateTime.HasValue)
            {
                list = list.Where(i => i.LocalActivityPerformedDateTime < data.EndDateTime.Value.AddDays(1));
                filtersApplied = true;
            }

            if (data.ManufacturingAreaId.HasValue)
            {
                list = list.Where(i => i.ManufacturingAreaId == data.ManufacturingAreaId);
                filtersApplied = true;
            }

            if (data.ControlSystemId.HasValue)
            {
                list = list.Where(i => i.ControlSystemId == data.ControlSystemId);
                filtersApplied = true;
            }

            if (data.ClassificationId.HasValue)
            {
                list = list.Where(i => i.ClassificationId == data.ClassificationId);
                filtersApplied = true;
            }

            if (!String.IsNullOrEmpty(data.EngineerName))
            {
                list = list.Where(i => EF.Functions.Like(i.EngineerName, "%" + data.EngineerName + "%"));
                filtersApplied = true;
            }

            if (data.ActivityTypeId.HasValue)
            {
                list = list.Where(i =>
                    i.ActivityTypeId != null &&
                    i.ActivityTypeId == data.ActivityTypeId);
                filtersApplied = true;
            }

            if (!String.IsNullOrEmpty(data.LogNumber))
            {
                list = list.Where(i => EF.Functions.Like(i.LogNumber, "%" + data.LogNumber + "%"));
                filtersApplied = true;
            }

            if (!String.IsNullOrEmpty(data.IncidentDescription))
            {
                list = list.Where(i => EF.Functions.Like(i.IncidentDescription, "%" + data.IncidentDescription + "%"));
                filtersApplied = true;
            }

            if (!String.IsNullOrEmpty(data.ActionSummary))
            {
                list = list.Where(i => EF.Functions.Like(i.ActionSummary, "%" + data.ActionSummary + "%"));
                filtersApplied = true;
            }

            if (!String.IsNullOrEmpty(data.ShortDescription))
            {
                list = list.Where(i => EF.Functions.Like(i.ShortDescription, "%" + data.ShortDescription + "%"));
                filtersApplied = true;
            }
            #endregion Apply Filtering

            if (filtersApplied)
                return list;
            else
                return null;

        }

        //GET:  /Incidents/GetDocument/5
        //REVISED: BDR 2020-04-09 to add conditional logic for filling out fields
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES11_Users,CSLG1\\KANBPL_APP_ENG_ES11_Admin")]
        [HttpGet]
        public async Task<IActionResult> GetDocument(int? id)
        {
            Logger.LogTrace("Incidents/GetDocument called with [{id}]", id);
            //make sure we have a valid incident
            if (!id.HasValue)
                return NotFound();

            var incident = await DatabaseContext
                .Incidents
                .AsNoTracking()
                .Include(i => i.ManufacturingArea)
                .Include(i => i.ControlSystem)
                .Include(i => i.Classification)
                .FirstOrDefaultAsync(i => i.IncidentId == id);

            if (incident == null)
                return NotFound();

            //make sure the incident is an ES11
            if (!incident.RequireES11.HasValue || !incident.RequireES11.Value)
            {
                TempData["Error"] = $"Cannot get document for a non-ES11 incident.";
                return RedirectToAction("Index");
            };
                        
            incident.DateFormat = DateFormat;

            //read the template into memory
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\docs");
            var attachment = incident.RequireES11.HasValue && 
                             incident.RequireES11.Value && 
                             incident.ActivityTypeId.Equals('M') ? "AttachmentA_ES11M.docx" : "AttachmentA.docx";
            var templatePath = Path.Combine(path, attachment);
            var template = await System.IO.File.ReadAllBytesAsync(templatePath);

            //determine the output file name
            var outputFile = incident.LogNumber + ".docx";

            var activityType = "ActivityType" + incident.ActivityTypeId;

            Logger.LogTrace("Incidents/GetDocument, generating document in memory");
            //open the word template
            var mem = new MemoryStream();
            await mem.WriteAsync(template, 0, template.Length);

            #region Attachment A Processing
            using (var doc = WordprocessingDocument.Open(mem, true))
            {
                var main = doc.MainDocumentPart.Document;
                var body = main.Body;

                //loop through the document properties
                foreach (var property in body.Descendants<SdtProperties>())
                {
                    //tag is the property name
                    var tag = property.Descendants<Tag>().First().Val.ToString();
                    var field = property.Parent;

                    //set the appropriate activity type
                    SetCheckBox(field, tag == activityType);

                    //fill out the form fields	REVISED BDR 2020-03-11
                    //REMOVED software revision 2020-04-02
                    switch (tag)
                    {
                        case "ImpactSafety":        
                            SetCheckBox(field, GetCheckboxValue(incident.ImpactSafety)); 
                            break;
                        
                        case "ImpactQuality":       
                            SetCheckBox(field, GetCheckboxValue(incident.ImpactQuality)); 
                            break;
                        
                        case "ImpactValidation":    
                            SetCheckBox(field, GetCheckboxValue(incident.ImpactValidation)); 
                            break;
                        
                        case "RequirePreApproval":  
                            SetCheckBox(field, GetCheckboxValue(incident.RequirePreApproval)); 
                            break;
                        
                        case "RequireTagOut":       
                            SetCheckBox(field, GetCheckboxValue(incident.RequireTagOut)); 
                            break;
                        
                        case "RequireMgrApproval":  
                            SetCheckBox(field, GetCheckboxValue(incident.RequireManagerApproval)); 
                            break;

                        case "ManufacturingArea":   
                            SetText(field, incident.ManufacturingArea.Description); 
                            break;
                        
                        case "ControlSystem":       
                        case "ControlSystem2":      
                            SetText(field, incident.ControlSystem.ControlSystemDescription); 
                            break;
                        
                        case "ProposedChange":      
                            if (incident.ActivityTypeId == 'S' | incident.ActivityTypeId == 'D')
                                SetText(field, "N/A"); 
                            else
                                SetText(field, incident.ActionSummary); 
                            break;

                        case "IncidentDescription": 
                            SetText(field, incident.IncidentDescription); 
                            break;

                        case "ActionSummary":       
                            if (incident.ActivityTypeId == 'M' | incident.ActivityTypeId == 'T' | incident.ActivityTypeId == 'L') 
                                SetText(field, "To be Determined"); 
                            else
                                SetText(field, incident.ActionSummary); 
                            break;

                        case "Classification":      
                            SetText(field, incident.Classification.Name); 
                            break;
                        
                        case "LogNumber":           
                            SetText(field, incident.LogNumber); 
                            break;

                        case "LotNumber":           
                            if (incident.ActivityTypeId == 'M' | incident.ActivityTypeId == 'T' | incident.ActivityTypeId == 'L') 
                                SetText(field, "To be Determined"); 
                            else
                                SetText(field, incident.LotNumber); 
                            break;

                        case "EngineerName":        
                            if (incident.ActivityTypeId == 'M' | incident.ActivityTypeId == 'T' | incident.ActivityTypeId == 'L') 
                                SetText(field, "To be Determined"); 
                            else
                                SetText(field, incident.EngineerName);     
                            break;
                        
                        case "CreatedDate":         
                            if (incident.ActivityTypeId == 'M' | incident.ActivityTypeId == 'T' | incident.ActivityTypeId == 'L')
                                SetText(field, "To be Determined"); 
                            break;

                        case "ComparisonMade":      
                            SetCheckBox(field, GetCheckboxValue(incident.ProgramComparisonMade)); 
                            break;

                        case "ActivityComplete":    
                            SetCheckBox(field, GetCheckboxValue(incident.ActivityComplete)); 
                            break;
                        
                        case "SoftwareIDs":
                            if (incident.ActivityTypeId == 'S' | incident.ActivityTypeId == 'D')
                                SetText(field, "N/A"); 
                            break;

                        case "SoftwareRevision":
                            if (incident.ActivityTypeId == 'M' | incident.ActivityTypeId == 'T' | incident.ActivityTypeId == 'L')
                                SetText(field, "To be Determined"); 
                            break;

                        case "SystemSpecifications":  
                            if (incident.ActivityTypeId == 'S' | incident.ActivityTypeId == 'D')
                                SetText(field, "N/A"); 
                            else
                                SetText(field, incident.SystemSpecifications); 
                            break;

                        case "SystemSpecifications2": 
                            if (incident.ActivityTypeId == 'M' | incident.ActivityTypeId == 'T' | incident.ActivityTypeId == 'L')
                                SetText(field, "To be Determined"); 
                            else
                                SetText(field, incident.SystemSpecifications); 
                            break;

                        case "Drawings":            
                            if (incident.ActivityTypeId == 'S' | incident.ActivityTypeId == 'D')
                                SetText(field, "N/A"); 
                            else
                                SetText(field, incident.Drawings); 
                            break;

                        case "Drawings2":           
                            if (incident.ActivityTypeId == 'M' | incident.ActivityTypeId == 'T' | incident.ActivityTypeId == 'L')
                                SetText(field, "To be Determined"); 
                            else
                                SetText(field, incident.Drawings); 
                            break;
                        case "PRNumber":            
                            if (incident.ActivityTypeId == 'S' | incident.ActivityTypeId == 'D')
                                SetText(field, "N/A"); 
                            else
                                SetText(field, incident.PRNumber); 
                            break;

                        case "PRNumber2":           
                            SetText(field, incident.PRNumber); 
                            break;

                        case "TagOutNumber":        
                            if (incident.ActivityTypeId == 'M' | incident.ActivityTypeId == 'T' | incident.ActivityTypeId == 'L') 
                                SetText(field, "To be Determined"); 
                            else
                                SetText(field, incident.TagOutNumber); 
                            break;
                    }
                }

                //save the changes to memory
                main.Save();
            }
            #endregion Attachment A Processing

            mem.Position = 0;
            return File(mem, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputFile);
        }


        //helper method to format booleans for exporting
        private string FormatBooleanProperty(bool? property)
        {
            if (property.HasValue)
            {
                if (property.Value)
                    return "Yes";
                else
                    return "No";
            }
            return "";
        }

        //helper method to format booleans for the ES11 form
        private bool GetCheckboxValue(bool? property) => (property.HasValue && property.Value);

        //helper method to tranform incident into a CSV record  
        //REVISED BDR 2020-03-11 to enable ES-11 content
        //REMOVED software revision 2020-04-02
        private string IncidentToCsv(Incident incident)
        {
            Logger.LogTrace("Incidents/IncidentToCsv called for [{id}]", incident.IncidentId);
            StringBuilder output = new StringBuilder();
            output.Append(incident.IncidentId).Append(",");

            if (String.IsNullOrEmpty(incident.LogNumber))
                output.Append("N/A,");
            else
                output.Append(incident.LogNumber).Append(",");

            string activity = incident.ActivityTypeId.HasValue ?
                              incident.ActivityType.Description : "";
                        
            output.Append(incident.ActivityPerformedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append(incident.LocalActivityPerformedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append($"\"{ incident.EngineerName }\",");
            output.Append($"\"{ incident.UserId }\",");
            output.Append($"\"{ incident.ShortDescription }\",");
            output.Append($"\"{ incident.Classification.Name }\",");
            output.Append($"\"{ incident.ManufacturingArea.Name }\",");
            output.Append($"\"{ incident.ControlSystem.Name }\",");
            output.Append($"\"{ incident.AreaContact }\",");
            output.Append(incident.EstimatedDownTimeHours.ToString("F2")).Append(",");
            output.Append(incident.EngineeringTimeHours.ToString("F2")).Append(",");
            output.Append($"\"{ incident.IncidentDescription?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append($"\"{ incident.ActionSummary?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.Resolved) }\",");
            output.Append($"\"{ incident.HandoffWork?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append($"\"{ incident.BTServiceRequestNumber }\",");
            output.Append($"\"{ incident.BTServiceRequestSummary?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append($"\"{ incident.Escalation }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.RequireES11) }\",");
            output.Append($"\"{ activity }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.ImpactSafety) }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.ImpactQuality) }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.ImpactValidation) }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.RequirePreApproval) }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.RequireManagerApproval) }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.RequireTagOut) }\",");
            output.Append($"\"{ incident.TagOutNumber }\",");
            output.Append($"\"{ incident.PRNumber }\",");
            output.Append($"\"{ incident.SystemSpecifications }\",");
            output.Append($"\"{ incident.Drawings }\",");
            output.Append($"\"{ incident.LotNumber }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.ProgramComparisonMade) }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.ActivityComplete) }\",");
            output.Append(incident.CreatedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append(incident.LocalCreatedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append(incident.LastModifiedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append(incident.LocalLastModifiedDateTime.ToString("u").TrimEnd('Z'));

            return output.ToString();
        }

        //REVISED BDR 2020-03-11, 2020-03-18
        //REMOVED software revision 2020-04-02
        //ADDED short description 2020-05-13
        private readonly string ExportHeader = "Incident Id,Log Number,UTC Activity Performed DateTime,Local Activity Performed DateTime,Engineer Name,UserId,Short Description,Classification,Manufacturing Area,Control System,Area Contact,Est. Down Time Hours,Engineering Time Hours,Incident Description,Action Summary,Resolved?,Handoff Work,BT Service Request Number,BT Service Request Summary,Escalation,ES-11 Required?,Activity Type,Impact Safety?,Impact Quality?,Impact Validation?,Pre-Approval Required?,Manager Approval Required?,Tagout Required?,Tagout Number,PR Number,System Specifications,Drawings,Lot Number,Comparison Made?,Activity Complete?,UTC Created DateTime,Local Created DateTime,UTC Last Modified DateTime,Local Last Modified DateTime";

        //role helper method
        private bool IsAdminUser() => User.IsInRole("cslg1.cslg.net\\KANBPL_APP_ENG_ES11_Admin");

        private void SetText(OpenXmlElement element, string value)
        {
            var paragraph = element.Descendants<Paragraph>().FirstOrDefault();
            var parent = paragraph.Parent;
            parent.ReplaceChild(new Paragraph(new Run(new Text(value))), paragraph);
        }

        private void SetCheckBox(OpenXmlElement element, bool state)
        {
            //exit early if false; no need to change the state
            if (!state) return;

            var checkbox = element.Descendants<SdtContentCheckBox>().FirstOrDefault();
            checkbox.Checked.Val = OnOffValues.True;
            Run run = element.Descendants<Run>().First();
            run.ReplaceChild(new Text("☒"), run.Descendants<Text>().First());

        }
    }
}
