using IncidentTracking.Data;
using IncidentTracking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

/// <remarks>
/// ================================================================================
/// MODULE:  T3GroupsController.cs
///         
/// PURPOSE:
/// This class represents the Tier3 groups controller which is responsible for 
/// handling all interactions between the groups model, related view 
/// models and associated views (user displays).
///         
/// Copyright:    ©2020 by E2i, Inc.
/// Created Date: 2020-04-29
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2020-04-29  Initial version
/// Brad Robbins    2020-06-11  Added logging
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Controllers
{
    [Authorize(Roles = "cslg1.cslg.net\\KANBPL_APP_ENG_ES11_Admin")]
    public class T3GroupsController : BaseController
    {
        public T3GroupsController(ApplicationDbContext context, IConfiguration config, ILogger<T3GroupsController> logger)
            : base(context, config, logger){}

        // GET: T3Groups
        public async Task<IActionResult> Index(string sortOrder, int? page)
        {
            Logger.LogTrace("T3Groups/Index called with [{sortOrder}] and page [{page}]", sortOrder, page);
            //setup sorting data
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var list = DatabaseContext.T3Groups.AsNoTracking();

            if (sortOrder == "name_desc")
                list = list.OrderByDescending(c => c.Name);
            else
                list = list.OrderBy(c => c.Name);

            var outList = await PaginatedList<T3Group>
                .CreateAsync(list, page ?? 1, PageSize);

            return View("Index", outList);
        }
        
        // GET: T3Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Logger.LogTrace("T3Groups/Details called with [{id}]", id);
            if (id == null)
                return NotFound();

            var group = await DatabaseContext.T3Groups
                .Include(g => g.ManufacturingAreas)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.T3GroupId == id);
            if (group == null)
                return NotFound();

            return View(group);
        }

        // GET: T3Groups/Create
        public IActionResult Create() => View();

        // POST: T3Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("T3GroupId,Name,SmeName,SmeEmailAddress,ReplyToEmailAddress")] T3Group group)
        {
            Logger.LogTrace("T3Groups/Create[Post] called");
            if (ModelState.IsValid)
            {
                try
                {
                    DatabaseContext.Add(group);
                    await DatabaseContext.SaveChangesAsync();
                    TempData["Message"] = "Group has been added.";
                    Logger.LogInformation("{user} added T3Group[{name}]", group.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    string message = "Cannot add group because ID is in use.";
                    TempData["Error"] = message;
                    Logger.LogDebug(message);
                }
                catch (SqlException ex)
                {
                    string message = $"Cannot add group because of a database error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot add group because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
            }
            return View(group);
        }

        // GET: T3Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Logger.LogTrace("T3Groups/Edit[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var group = await DatabaseContext.T3Groups.FindAsync(id);
            if (group == null)
                return NotFound();
            return View(group);
        }

        // POST: T3Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("T3GroupId,Name,SmeName,SmeEmailAddress,ReplyToEmailAddress")] T3Group group)
        {
            Logger.LogTrace("T3Groups/Edit[POST] called with [{id}]", id);
            if (id != group.T3GroupId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    DatabaseContext.Update(group);
                    await DatabaseContext.SaveChangesAsync();
                    TempData["Message"] = "Group has been updated.";
                    Logger.LogInformation("{user} updated T3Group[{name}]", UserName, group.Name);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string message;
                    if (!T3GroupExists(group.T3GroupId))
                        message = $"Cannot edit group because it no longer exists.";
                    else
                        message = $"Cannot edit group because of a concurrency error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating T3Group[{id}] because of {message}", id, message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot edit group because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                     Logger.LogError("Error updating T3Group[{id}] because of {message}", id, message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        // GET: T3Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Logger.LogTrace("T3Groups/Delete[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var group = await DatabaseContext.T3Groups
                .FirstOrDefaultAsync(g => g.T3GroupId == id);
            if (group == null)
                return NotFound();

            return View(group);
        }

        // POST: T3Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Logger.LogTrace("T3Groups/Delete[POST] called with [{id}]", id);
            try
            {
                var group = await DatabaseContext.T3Groups.FindAsync(id);
                DatabaseContext.T3Groups.Remove(group);
                await DatabaseContext.SaveChangesAsync();
                Logger.LogInformation("{user} deleted T3Group[{name}]", UserName, group.Name);
                TempData["Message"] = "Group has been deleted.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete an group that is in use.";
                Logger.LogDebug("Error deleting T3Group that is in user.");
            }
            catch (SqlException ex)
            {
                TempData["Error"] = $"Cannot delete group.  A database error occurred: {ex.Message}";
                Logger.LogError("Error deleting T3Group, SQL error: {message}", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Cannot delete group.  A general error occurred: {ex.Message}";
                Logger.LogError("Error deleting T3Group, general error: {message}", ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool T3GroupExists(int id) => DatabaseContext.T3Groups.Any(g => g.T3GroupId == id);
    }
}
