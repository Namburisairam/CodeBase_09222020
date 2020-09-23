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
/// MODULE:  ManufacturingAreasController.cs
///         
/// PURPOSE:
/// This class represents the manufacturing area controller which is responsible for 
/// handling all interactions between the manufacturing areas model, related view 
/// models and associated views (user displays).
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2019-05-24  Added ReportLabel for creating/editing
/// Brad Robbins    2019-09-24  Added support for SME and Mgr fields; added Details action
/// Brad Robbins    2020-06-15  Added logging
/// ================================================================================
/// </remarks>

namespace ES13Web.Controllers
{
    [Authorize(Roles = "CSLG1\\KANBPL_APP_ENG_ES13_Admin")]
    public class ManufacturingAreasController : BaseController
    {
        public ManufacturingAreasController(ApplicationDbContext context, IConfiguration config, ILogger<ManufacturingAreasController> logger)
            : base(context, config, logger){}

        // GET: ManufacturingAreas
        public async Task<IActionResult> Index(string sortOrder, int? page)
        {
            Logger.LogTrace("ManufacturingAreas/Index called with [{sortOrder}] and page [{page}]", sortOrder, page);
            //setup sorting data
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var list = DatabaseContext.ManufacturingAreas
                .Select(m => new ManufacturingAreaIndexViewModel 
                { 
                    ManufacturingAreaId = m.ManufacturingAreaId,
                    Name = m.Name
                }).AsNoTracking();

            switch (sortOrder)
            {
                case "name_desc": list = list.OrderByDescending(m => m.Name); break;
                default: list = list.OrderBy(m => m.Name); break;
            }

            var outList = await PaginatedList<ManufacturingAreaIndexViewModel>
                .CreateAsync(list, page ?? 1, PageSize);

            return View("Index", outList);
        }

        // GET: ManufacturingAreas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Logger.LogTrace("ManufacturingAreas/Details called with [{id}]", id);
            if (id == null)
                return NotFound();

            var manufacturingArea = await DatabaseContext.ManufacturingAreas
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ManufacturingAreaId == id);
            if (manufacturingArea == null)
                return NotFound();

            return View(manufacturingArea);
        }

        // GET: ManufacturingAreas/Create
        public IActionResult Create() => View();

        // POST: ManufacturingAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ManufacturingAreaId,Name,ReportLabel,SmeName,SmeEmailAddress,MgrName,MgrEmailAddress,ReplyToEmailAddress")] ManufacturingArea manufacturingArea)
        {
            Logger.LogTrace("ManufacturingAreas/Create[Post] called");
            if (ModelState.IsValid)
            {
                try
                {
                    DatabaseContext.Add(manufacturingArea);
                    await DatabaseContext.SaveChangesAsync();
                    TempData["Message"] = "Area has been added.";
                    Logger.LogInformation("{user} added Manufacturing Area[{name}]", UserName, manufacturingArea.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex)
                {
                    string message = $"Cannot add area because of a database error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot add area because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
            }
            return View(manufacturingArea);
        }

        // GET: ManufacturingAreas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Logger.LogTrace("ManufacturingAreas/Edit[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var manufacturingArea = await DatabaseContext.ManufacturingAreas.FindAsync(id);
            if (manufacturingArea == null)
                return NotFound();
            return View(manufacturingArea);
        }

        // POST: ManufacturingAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ManufacturingAreaId,Name,ReportLabel,SmeName,SmeEmailAddress,MgrName,MgrEmailAddress,ReplyToEmailAddress")] ManufacturingArea manufacturingArea)
        {
            Logger.LogTrace("ManufacturingAreas/Edit[POST] called with [{id}]", id);
            if (id != manufacturingArea.ManufacturingAreaId)
                return NotFound();

            if (ModelState.IsValid)
            {
                
                try
                {
                    DatabaseContext.Update(manufacturingArea);
                    await DatabaseContext.SaveChangesAsync();
                    TempData["Message"] = "Area has been updated.";
                    Logger.LogInformation("{user} updated ManufacturingArea[{id}]", UserName, id);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string message;
                    if (!ManufacturingAreaExists(manufacturingArea.ManufacturingAreaId))
                        message = $"Cannot edit area because it no longer exists.";
                    else
                        message = $"Cannot edit area because of a concurrency error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating ManufacturingArea[{id}] because of {message}", id, message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot edit area because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating ManufacturingArea[{id}] because of {message}", id, message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(manufacturingArea);
        }

        // GET: ManufacturingAreas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Logger.LogTrace("ManufacturingAreas/Delete[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var manufacturingArea = await DatabaseContext.ManufacturingAreas
                .FirstOrDefaultAsync(m => m.ManufacturingAreaId == id);
            if (manufacturingArea == null)
                return NotFound();

            return View(manufacturingArea);
        }

        // POST: ManufacturingAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Logger.LogTrace("ManufacturingAreas/Delete[POST] called with [{id}]", id);
            try
            {
                var manufacturingArea = await DatabaseContext.ManufacturingAreas.FindAsync(id);
                DatabaseContext.ManufacturingAreas.Remove(manufacturingArea);
                await DatabaseContext.SaveChangesAsync();
                Logger.LogInformation("{user} deleted ManufacturingArea[{name}]", UserName, manufacturingArea.Name);
                TempData["Message"] = "Area has been deleted.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete an area that is in use.";
                Logger.LogDebug("Error deleting manufacturing area that is in use.");
            }
            catch (SqlException ex)
            {
                TempData["Error"] = $"Cannot delete area.  A database error occurred: {ex.Message}";
                Logger.LogError("Error deleting manufacturing area, SQL error: {message}", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Cannot delete area.  A general error occurred: {ex.Message}";
                Logger.LogError("Error deleting manufacturing area, general error: {message}", ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ManufacturingAreaExists(int id)
            => DatabaseContext.ManufacturingAreas.Any(e => e.ManufacturingAreaId == id);
    }
}
