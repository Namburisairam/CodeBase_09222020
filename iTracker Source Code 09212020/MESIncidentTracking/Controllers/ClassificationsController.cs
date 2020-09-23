using ES13Web.Data;
using ES13Web.Models;
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
/// MODULE:  ClassificationsController.cs
///         
/// PURPOSE:
/// This class represents the classifications controller which is responsible for 
/// handling all interactions between the classification model, related view models 
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
/// Brad Robbins    2019-05-28  Added report label to create/edit actions
/// Brad Robbins    2020-06-15  Added logging
/// ================================================================================
/// </remarks>

namespace ES13Web.Controllers
{
    [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES13_Admin")]
    public class ClassificationsController : BaseController
    {
        public ClassificationsController(ApplicationDbContext context, IConfiguration config, ILogger<ClassificationsController> logger)
            : base(context, config, logger){}

        // GET: Classifications
        public async Task<IActionResult> Index(string sortOrder)
        {
            Logger.LogTrace("Classifications/Index called with [{sortOrder}]", sortOrder);
            //setup sorting data
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var list = DatabaseContext.Classifications.AsNoTracking();

            if (sortOrder == "name_desc")
                list = list.OrderByDescending(c => c.Name);
            else
                list = list.OrderBy(c => c.Name);

            return View("Index", await list.ToListAsync());
        }
        
        // GET: Classifications/Create
        public IActionResult Create() => View();

        // POST: Classifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassificationId,Name,ReportLabel")] Classification classification)
        {
            Logger.LogTrace("Classifications/Create called");
            if (ModelState.IsValid)
            {
                try
                {
                    DatabaseContext.Add(classification);
                    await DatabaseContext.SaveChangesAsync();
                    TempData["Message"] = "Classification has been added.";
                    Logger.LogInformation("{user} added Classification[{name}]", UserName, classification.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex)
                {
                    string message = $"Cannot add classification because of a database error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot add classification because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
            }
            return View(classification);
        }

        // GET: Classifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Logger.LogTrace("Classifications/Edit[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();
            
            var classification = await DatabaseContext.Classifications.FindAsync(id);
            if (classification == null)
                return NotFound();
            return View(classification);
        }

        // POST: Classifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassificationId,Name,ReportLabel")] Classification classification)
        {
            Logger.LogTrace("Classifications/Edit[POST] called with [{id}]", id);
            if (id != classification.ClassificationId)
            {
                Logger.LogDebug("Classifications/Edit not found [{id}], id");
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    DatabaseContext.Update(classification);
                    await DatabaseContext.SaveChangesAsync();
                    Logger.LogInformation("{user} updated Classification[{name}]", UserName, classification.Name);
                    TempData["Message"] = "Classification has been updated.";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string message;
                    if (!ClassificationExists(classification.ClassificationId))
                        message = $"Cannot edit classification because it no longer exists.";
                    else
                        message = $"Cannot edit classification because of a concurrency error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating Classification[{id}] because of {message}", id, message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot edit classification because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating Classification[{id}] because of {message}", id, message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(classification);
        }

        // GET: Classifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Logger.LogTrace("Classifications/Delete[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();
            
            var classification = await DatabaseContext.Classifications
                .FirstOrDefaultAsync(m => m.ClassificationId == id);
            if (classification == null)
                return NotFound();
            
            return View(classification);
        }

        // POST: Classifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Logger.LogTrace("Classifications/Delete[POST] called with [{id}]", id);
            try
            {
                var classification = await DatabaseContext.Classifications.FindAsync(id);
                DatabaseContext.Classifications.Remove(classification);
                await DatabaseContext.SaveChangesAsync();
                Logger.LogInformation("{user} deleted Classification[{name}]", UserName, classification.Name);
                TempData["Message"] = "Classification has been deleted.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete a classification that is in use.";
                Logger.LogDebug("Error deleting classification that is in use.");
            }
            catch (SqlException ex)
            {
                TempData["Error"] = $"Cannot delete classification.  A database error occurred: {ex.Message}";
                Logger.LogError("Error deleting classification, SQL error: {message}", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Cannot delete classification.  A general error occurred: {ex.Message}";
                Logger.LogError("Error deleting classification, general error: {message}", ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClassificationExists(int id)
            => DatabaseContext.Classifications.Any(e => e.ClassificationId == id);
    }
}
