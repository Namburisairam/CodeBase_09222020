using ES13Web.Data;
using ES13Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
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
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2019-08-18  Commented authorize lines to open access to domain users
/// Brad Robbins    2019-08-29  Added support for engineering time
/// Brad Robbins    2019-09-11  Changes for searching, adding/editing changes
/// Brad Robbins    2019-09-16  Revised GetFilteredSearchResults to add one day to end date
/// Brad Robbins    2019-10-04  Added support for batch number to index, details, export and search actions
/// Brad Robbins    2019-12-03  Added support for hidden UserId field
/// Brad Robbins    2019-12-06  Added UserId for Duplicate and Search actions
/// Brad Robbins    2020-03-18  Added escalation field
/// Brad Robbins    2020-06-15  Added logging; reworked Index/Search/Export filtering; relocated GetUserFullName to BaseController
/// ================================================================================
/// </remarks>

namespace ES13Web.Controllers
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
            ViewData["MfgAreaSortParam"] = sortOrder == "mfg" ? "mfg_desc" : "mfg";
            ViewData["CauseSortParam"] = sortOrder == "cause" ? "cause_desc" : "cause";
            ViewData["EngineerSortParam"] = sortOrder == "engr" ? "engr_desc" : "engr";
            ViewData["ShortDescriptionSortParam"] = sortOrder == "dscrptn" ? "dscrptn_desc" : "dscrptn";
            ViewData["ResolvedSortParam"] = sortOrder == "resolved" ? "resolved_desc" : "resolved";
            ViewData["WerumSortParam"] = sortOrder == "werum" ? "werum_desc" : "werum";
            ViewData["BTSortParam"] = sortOrder == "bt" ? "bt_desc" : "bt";
            ViewData["Es13SortParam"] = sortOrder == "es13" ? "es13_desc" : "es13";
            ViewData["BatchSortParam"] = sortOrder == "batch" ? "batch_desc" : "batch";

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
                    ManufacturingAreaName = m.Name,
                    ClassificationName = c.Name,
                    EngineerName = i.EngineerName,
                    ShortDescription = i.ShortDescription,
                    RequireES13 = CAST(COALESCE(i.RequireES13, 0) AS BIT),
                    Resolved = CAST(COALESCE(i.Resolved, 0) AS BIT),
                    WerumTicket = i.WerumTicket,
                    BTServiceRequestNumber = i.BTServiceRequestNumber,
                    BatchNumber = i.BatchNumber,
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
                WHERE @searchString IS NOT NULL AND (
                    i.WerumTicket LIKE '%' + @searchString + '%' OR
                    i.BTServiceRequestNumber LIKE '%' + @searchString + '%' OR
                    i.BatchNumber LIKE '%' + @searchString + '%' OR
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

                case "cause": list = list.OrderBy(i => i.ClassificationName).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "cause_desc": list = list.OrderByDescending(i => i.ClassificationName).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "mfg": list = list.OrderBy(i => i.ManufacturingAreaName).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "mfg_desc": list = list.OrderByDescending(i => i.ManufacturingAreaName).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "engr": list = list.OrderBy(i => i.EngineerName).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "engr_desc": list = list.OrderByDescending(i => i.EngineerName).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "dscrptn": list = list.OrderBy(i => i.ShortDescription).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "dscrptn_desc": list = list.OrderByDescending(i => i.ShortDescription).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "resolved": list = list.OrderBy(i => i.Resolved).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "resolved_desc": list = list.OrderByDescending(i => i.Resolved).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "werum": list = list.OrderBy(i => i.WerumTicket).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "werum_desc": list = list.OrderByDescending(i => i.WerumTicket).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "bt": list = list.OrderBy(i => i.BTServiceRequestNumber).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "bt_desc": list = list.OrderByDescending(i => i.BTServiceRequestNumber).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                
                case "es13": list = list.OrderBy(i => i.RequireES13).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "es13_desc": list = list.OrderByDescending(i => i.RequireES13).ThenByDescending(i => i.ActivityPerformedDateTime); break;

                case "batch": list = list.OrderBy(i => i.BatchNumber).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                case "batch_desc": list = list.OrderByDescending(i => i.BatchNumber).ThenByDescending(i => i.ActivityPerformedDateTime); break;
                
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
            int? classificationId = null,
            string engineerName = null,
            string shortDescription = null,
            string incidentDescription = null,
            string actionSummary = null,
            TernaryOption resolved = TernaryOption.All,
            TernaryOption hasWerumTicket = TernaryOption.All,
            string werumTicket = null,
            string bTServiceRequestNumber = null,
            TernaryOption interventionPerformed = TernaryOption.All,
            string batchNumber = null,
            int limit = 20)
        {
            Logger.LogTrace("Incidents/Search called");
            var data = new IncidentSearchViewModel
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                ManufacturingAreaId = manufacturingAreaId,
                ClassificationId = classificationId,
                EngineerName = engineerName,
                ShortDescription = shortDescription,
                IncidentDescription = incidentDescription,
                ActionSummary = actionSummary,
                Resolved = resolved,
                HasWerumTicket = hasWerumTicket,
                WerumTicket = werumTicket,
                BTServiceRequestNumber = bTServiceRequestNumber,
                InterventionPerformed = interventionPerformed,
                BatchNumber = batchNumber,
                Limit = limit
            };

            var list = GetFilteredSearchResults(data);

            if (list != null)
            {
                data.Results = await list
                    .Select(i => new IncidentSearchResultViewModel
                    {
                        IncidentId = i.IncidentId,
                        ActivityPerformedDateTime = i.LocalActivityPerformedDateTime,
                        FormattedLocalActivityPerformedDateTime = i.LocalActivityPerformedDateTime.ToString(DateFormat),
                        ManufacturingAreaName = i.ManufacturingArea.Name,
                        ClassificationName = i.Classification.Name,
                        EngineerName = i.EngineerName,
                        ShortDescription = i.ShortDescription,
                        InterventionPerformed = i.RequireES13 ?? false,
                        Resolved = i.Resolved ?? false,
                        WerumTicket = i.WerumTicket,
                        BTServiceRequestNumber = i.BTServiceRequestNumber,
                        BatchNumber = i.BatchNumber,
                        UserId = i.UserId
                    })
                    .OrderByDescending(i => i.ActivityPerformedDateTime)
                    .Take(data.Limit)
                    .ToListAsync();
            }
            else
                data.Results = new List<IncidentSearchResultViewModel>();

            //load select (drop down) lists
            ViewData["ClassificationId"] = await GetClassificationsSelectListAsync();
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync();
            
            return View("Search", data);
        }

        // GET: Incidents/Export
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES13_Users,CSLG1\\KANBPL_APP_ENG_ES13_Admin")]
        public async Task<IActionResult> Export(
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            int? manufacturingAreaId = null,
            int? classificationId = null,
            string engineerName = null,
            string shortDescription = null,
            char? activityTypeId = null,
            string incidentDescription = null,
            string actionSummary = null,
            TernaryOption resolved = TernaryOption.All,
            TernaryOption hasWerumTicket = TernaryOption.All,
            string werumTicket = null,
            string bTServiceRequestNumber = null,
            TernaryOption interventionPerformed = TernaryOption.All,
            string batchNumber = null,
            int limit = 0)
        {
            Logger.LogTrace("Incidents/Export called");
            var data = new IncidentSearchViewModel
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                ManufacturingAreaId = manufacturingAreaId,
                ClassificationId = classificationId,
                EngineerName = engineerName,
                ShortDescription = shortDescription,
                IncidentDescription = incidentDescription,
                ActionSummary = actionSummary,
                Resolved = resolved,
                HasWerumTicket = hasWerumTicket,
                WerumTicket = werumTicket,
                BTServiceRequestNumber = bTServiceRequestNumber,
                InterventionPerformed = interventionPerformed,
                BatchNumber = batchNumber,
                Limit = limit
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
                .Include(i => i.Classification)
                .FirstOrDefaultAsync(i => i.IncidentId == id);

            if (incident == null)
                return NotFound();

            if (incident.RequireES13.HasValue && incident.RequireES13.Value)
            {
                //this one has ES13; get more information
                incident = await DatabaseContext.Incidents
                    .AsNoTracking()
                    .Include(i => i.ManufacturingArea)
                    .Include(i => i.Classification)
                    .FirstAsync(i => i.IncidentId == id);
            }

            incident.DateFormat = DateFormat;
            return View(incident);
        }

        // GET: Incidents/Create
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES13_Users,CSLG1\\KANBPL_APP_ENG_ES13_Admin")]
        public async Task<IActionResult> Create()
        {
            Logger.LogTrace("Incidents/Create[GET] called");
            //load select (drop down) lists
            ViewData["ClassificationId"] = await GetClassificationsSelectListAsync();
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync();
            ViewData["Mode"] = "Create";
            
            var log = new Incident
            {
                EngineerName = GetUserFullName(),
                LocalActivityPerformedDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local)
            };
            return View(log);
        }

        // POST: Incidents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES13_Users,CSLG1\\KANBPL_APP_ENG_ES13_Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IncidentId,EngineerName,ShortDescription,UserId,ManufacturingAreaId,AreaContact,EstimatedDownTimeHours,EngineeringTimeHours,IncidentDescription,ActionSummary,ClassificationId,Resolved,HandoffWork,RequireES13,WerumTicket,ES11LogNumber,BTServiceRequestNumber,BTServiceRequestSummary,LocalActivityPerformedDateTime,LocalTimeString,BatchNumber,WerumTicketResolved,WerumTicketSummary,Escalation")] Incident incident)
        {
            Logger.LogTrace("Incidents/Create[POST] called");
            if (ModelState.IsValid)
            {
                try
                {
                    //set create/modify times
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
                    float adjustedEngingeeringTime = Convert.ToSingle(Math.Round(incident.EngineeringTimeHours * 4f, 0) / 4f);
                    incident.EngineeringTimeHours = adjustedEngingeeringTime;

                    //add the user ID for the engineer creating the record
                    incident.UserId = User.Identity.Name;

                    DatabaseContext.Incidents.Add(incident);
                    await DatabaseContext.SaveChangesAsync();
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
            ViewData["ClassificationId"] = await GetClassificationsSelectListAsync(
                incident.ClassificationId);
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync(
                incident.ManufacturingAreaId);
            ViewData["Mode"] = "Create";

            return View(incident);
        }

        // GET: Incidents/Duplicate/4
        //This method creates a copy of an incident, resets its creation date/time
        //to the current time and the engineer name to the current user.  It then
        //opens the new record for final editing.
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES13_Users,CSLG1\\KANBPL_APP_ENG_ES13_Admin")]
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
                    AreaContact = incident.AreaContact,
                    ClassificationId = incident.ClassificationId,
                    ShortDescription = incident.ShortDescription,
                    IncidentDescription = incident.IncidentDescription,
                    ManufacturingAreaId = incident.ManufacturingAreaId,
                    RequireES13 = incident.RequireES13
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
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES13_Users,CSLG1\\KANBPL_APP_ENG_ES13_Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            Logger.LogTrace("Incidents/Edit[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var incident = await DatabaseContext.Incidents.FindAsync(id);

            if (incident == null)
                return NotFound();

            if (incident.UserId.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase) || IsAdminUser())
            {
                //load select (drop down) lists
                ViewData["ClassificationId"] = await GetClassificationsSelectListAsync(
                    incident.ClassificationId);
                ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync(
                    incident.ManufacturingAreaId);
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
        [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES13_Users,CSLG1\\KANBPL_APP_ENG_ES13_Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IncidentId,EngineerName,ShortDescription,UserId,ManufacturingAreaId,AreaContact,EstimatedDownTimeHours,EngineeringTimeHours,IncidentDescription,ActionSummary,ClassificationId,Resolved,HandoffWork,RequireES13,WerumTicket,ES11LogNumber,BTServiceRequestNumber,BTServiceRequestSummary,LocalActivityPerformedDateTime,CreatedDateTime,LocalCreatedDateTime,LocalTimeString,BatchNumber,WerumTicketResolved,WerumTicketSummary,Escalation")] Incident incident)
        {
            Logger.LogTrace("Incidents/Edit[POST] called with [{id}]", id);
            if (id != incident.IncidentId)
                return NotFound();

            if (ModelState.IsValid && (incident.UserId.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase) || IsAdminUser()))
            {
                try
                {
                    //set modify times
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
                    float adjustedEngingeeringTime = Convert.ToSingle(Math.Round(incident.EngineeringTimeHours * 4f, 0) / 4f);
                    incident.EngineeringTimeHours = adjustedEngingeeringTime;

                    DatabaseContext.Update(incident);
                    await DatabaseContext.SaveChangesAsync();
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
            ViewData["ClassificationId"] = await GetClassificationsSelectListAsync(
                incident.ClassificationId);
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync(
                incident.ManufacturingAreaId);
            ViewData["Mode"] = "Edit";

            return View(incident);
        }


        // GET: Incidents/Delete/5
        [Authorize(Roles = "cslg1.cslg.net\\KANBPL_APP_ENG_ES13_Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            Logger.LogTrace("Incidents/Delete[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var incident = await DatabaseContext.Incidents
                .AsNoTracking()
                .Include(i => i.ManufacturingArea)
                .FirstOrDefaultAsync(m => m.IncidentId == id);
            if (incident == null)
                return NotFound();

            return View(incident);
        }

        // POST: Incidents/Delete/5
        [Authorize(Roles = "cslg1.cslg.net\\KANBPL_APP_ENG_ES13_Admin")]
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
                .Include(i => i.Classification);

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

            //must include a Werum Ticket?
            if (data.HasWerumTicket == TernaryOption.Yes)
                list = list.Where(i => !String.IsNullOrEmpty(i.WerumTicket));

            //must not include a Werum Ticket?
            if (data.HasWerumTicket == TernaryOption.No)
                list = list.Where(i => String.IsNullOrEmpty(i.WerumTicket));
            //otherwise, don't filter

            if (!String.IsNullOrEmpty(data.WerumTicket))
            {
                list = list.Where(i => EF.Functions.Like(i.WerumTicket, "%" + data.WerumTicket + "%"));
                filtersApplied = true;
            }

            if (!String.IsNullOrEmpty(data.BTServiceRequestNumber))
            {
                list = list.Where(i => EF.Functions.Like(i.BTServiceRequestNumber, "%" + data.BTServiceRequestNumber + "%"));
                filtersApplied = true;
            }
            
            if (!String.IsNullOrEmpty(data.BatchNumber))
            {
                list = list.Where(i => EF.Functions.Like(i.BatchNumber, "%" + data.BatchNumber + "%"));
                filtersApplied = true;
            }
            
            //must be resolved?
            if (data.Resolved == TernaryOption.Yes)
                list = list.Where(i => i.Resolved.HasValue && i.Resolved.Value);
            //must not be resolved?
            if (data.Resolved == TernaryOption.No)
                list = list.Where(i => (i.Resolved.HasValue && !i.Resolved.Value) || !i.Resolved.HasValue);
            //otherwise, don't filter

            //must have intervention?
            if (data.InterventionPerformed == TernaryOption.Yes)
                list = list.Where(i => i.RequireES13.HasValue && i.RequireES13.Value);
            //must not be resolved?
            if (data.InterventionPerformed == TernaryOption.No)
                list = list.Where(i => (i.RequireES13.HasValue && !i.RequireES13.Value) || !i.RequireES13.HasValue);
            //otherwise, don't filter

            #endregion Apply Filtering

            if (filtersApplied)
                return list;
            else
                return null;

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


        //helper method to tranform incident into a CSV record
        private string IncidentToCsv(Incident incident)
        {
            Logger.LogTrace("Incidents/IncidentToCsv called for [{id}]", incident.IncidentId);
            StringBuilder output = new StringBuilder();
            output.Append(incident.IncidentId).Append(",");

            output.Append(incident.ActivityPerformedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append(incident.LocalActivityPerformedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append($"\"{ incident.EngineerName }\",");
            output.Append($"\"{ incident.UserId }\",");
            output.Append($"\"{ incident.ShortDescription }\",");
            output.Append($"\"{ incident.Classification.Name }\",");
            output.Append($"\"{ incident.ManufacturingArea.Name }\",");
            output.Append($"\"{ incident.AreaContact }\",");
            output.Append(incident.EstimatedDownTimeHours.ToString("F2")).Append(",");
            output.Append(incident.EngineeringTimeHours.ToString("F2")).Append(",");
            output.Append($"\"{ incident.IncidentDescription?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append($"\"{ incident.ActionSummary?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append($"\"{ incident.BatchNumber }\",");
            output.Append($"\"{ incident.Escalation }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.Resolved) }\",");
            output.Append($"\"{ incident.HandoffWork?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append($"\"{ incident.WerumTicket }\",");
            output.Append($"\"{ incident.WerumTicketSummary?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.WerumTicketResolved) }\",");
            output.Append($"\"{ FormatBooleanProperty(incident.RequireES13) }\",");
            //output.Append($"\"{ incident.ES11LogNumber }\",");
            output.Append($"\"{ incident.BTServiceRequestNumber }\",");
            output.Append($"\"{ incident.BTServiceRequestSummary?.Replace("\"", "'").Replace("\r\n", "|") }\",");
            output.Append(incident.CreatedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append(incident.LocalCreatedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append(incident.LastModifiedDateTime.ToString("u").TrimEnd('Z')).Append(",");
            output.Append(incident.LocalLastModifiedDateTime.ToString("u").TrimEnd('Z')).Append(",");

            return output.ToString();
        }

        private readonly string ExportHeader = "Incident Id,UTC Activity Performed DateTime,Local Activity Performed DateTime,Engineer Name,User ID,Short Description,Classification Name,Manufacturing Area,Area Contact,Est. Down Time Hours,Engineering Time Hours,Incident Description,Action Summary,Batch Number,Escalation,Resolved?,Handoff Work,Werum Ticket,Werum Ticket Summary,Werum Ticket Resolved?,Intervention Performed,BT Service Request Number,BT Service Request Summary,UTC Created DateTime,Local Created DateTime,UTC Last Modified DateTime,Local Last Modified DateTime";

        //role helper method
        private bool IsAdminUser()
        {
            return User.IsInRole("cslg1.cslg.net\\KANBPL_APP_ENG_ES13_Admin");
        }
    }
}
