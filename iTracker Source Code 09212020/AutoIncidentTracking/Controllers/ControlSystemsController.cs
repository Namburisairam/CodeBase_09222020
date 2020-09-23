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
/// MODULE:  ControlSystemsController.cs
///         
/// PURPOSE:
/// This class represents the control systems controller which is responsible for 
/// handling all interactions between the control systems model, related view models 
/// and associated views (user displays).
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-09-24
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-10-30  Initial version
/// Brad Robbins    2020-04-10  Added IsObsolete field
/// Brad Robbins    2020-06-11  Added logging; updated Index to improve filtering
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Controllers
{
    [Authorize(Roles = "cslg1.cslg.net\\KANBPL_APP_ENG_ES11_Admin")]
    public class ControlSystemsController : BaseController
    {
        public ControlSystemsController(ApplicationDbContext context, IConfiguration config, ILogger<ControlSystemsController> logger)
            : base(context, config, logger){}

        // GET: ControlSystems
        public async Task<IActionResult> Index(
            string sortOrder, 
            string currentFilter,
            string searchString,
            int? page)
        {
            Logger.LogTrace("ControlSystems/Index called with [{sortOrder}], filter [{filter}], search [{search}] and page [{page}]", sortOrder, currentFilter, searchString, page);
            //setup sorting, paging, filtering data
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ManufacturingAreaNameSortParam"] = sortOrder == "mfg" ? "mfg_desc" : "mfg";

            if (String.IsNullOrEmpty(searchString))
                searchString = currentFilter;
            else
                page = 1;

            ViewData["CurrentFilter"] = searchString;
            
            var list = DatabaseContext.ControlSystems
                .Include(c => c.ManufacturingArea)
                .AsNoTracking()
                .Select(c => new ControlSystemIndexViewModel
                {
                    ControlSystemId = c.ControlSystemId,
                    ManufacturingAreaName = c.ManufacturingArea.Name,
                    Name = c.Name,
                    Description = c.Description
                });

            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(c => EF.Functions.Like(c.ManufacturingAreaName, "%" + searchString + "%")
                                    || EF.Functions.Like(c.Name, "%" + searchString + "%")
                                    || EF.Functions.Like(c.Description, "%" + searchString + "%"));
            }

            //determine proper sorting
            switch (sortOrder)
            {
                case "name_desc": list = list.OrderByDescending(c => c.Name); break;
                case "mfg": list = list.OrderBy(c => c.ManufacturingAreaName); break;
                case "mfg_desc": list = list.OrderByDescending(c => c.ManufacturingAreaName); break;
                default: list = list.OrderBy(c => c.Name); break;
            }

            var outList = await PaginatedList<ControlSystemIndexViewModel>
                .CreateAsync(list, page ?? 1, PageSize);

            return View("Index", outList);
        }

        // GET: ControlSystems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Logger.LogTrace("ControlSystems/Details called with [{id}]", id);
            if (!id.HasValue)
                return NotFound();

            var cs = await DatabaseContext.ControlSystems
                .AsNoTracking()
                .Include(c => c.ManufacturingArea)
                .FirstAsync(c => c.ControlSystemId == id);

            if (cs == null)
                return NotFound();

            return View("Details", cs);
        }
        // GET: ControlSystems/Create
        public async Task<IActionResult> Create()
        {
            Logger.LogTrace("ControlSystems/Create[GET] called");
            //load the select (drop down) list
            ViewData["ManufacturingAreaId"] = await GetManufacturingAreasSelectListAsync();

            return View(new ControlSystem());
        }

        // POST: ControlSystems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ControlSystemId,ManufacturingAreaId,Name,Description,PlcId,PhysicalLocation,IsNetworked,IpAddress,GdrsLocation,Make,Model,Firmware,IsInAutosave,IsObsolete")] ControlSystem controlSystem)
        {
            Logger.LogTrace("ControlSystems/Create[POST] called");
            if (ModelState.IsValid)
            {
                try
                {
                    DatabaseContext.Add(controlSystem);
                    await DatabaseContext.SaveChangesAsync();
                    TempData["Message"] = "Control system has been added.";
                    Logger.LogInformation("{user} added Control System[{name}]", UserName, controlSystem.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex)
                {
                    string message = $"Cannot add control system because of a database error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot add control system because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError(message);
                }
            }

            //load the select (drop down) list
            ViewData["ManufacturingAreaId"] = 
                await GetManufacturingAreasSelectListAsync(controlSystem.ManufacturingAreaId);

            return View(controlSystem);
        }

        // GET: ControlSystems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Logger.LogTrace("ControlSystems/Edit[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();
            
            var controlSystem = await DatabaseContext.ControlSystems.FindAsync(id);
            if (controlSystem == null)
                return NotFound();

            //load the select (drop down) list
            ViewData["ManufacturingAreaId"] =
                await GetManufacturingAreasSelectListAsync(controlSystem.ManufacturingAreaId);

            return View(controlSystem);
        }

        // POST: ControlSystems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ControlSystemId,ManufacturingAreaId,Name,Description,PlcId,PhysicalLocation,IsNetworked,IpAddress,GdrsLocation,Make,Model,Firmware,IsInAutosave,IsObsolete")] ControlSystem controlSystem)
        {
            Logger.LogTrace("ControlSystems/Edit[POST] called with [{id}]", id);
            if (id != controlSystem.ControlSystemId)
                return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    DatabaseContext.Update(controlSystem);
                    await DatabaseContext.SaveChangesAsync();
                    TempData["Message"] = "Control system has been updated.";
                    Logger.LogInformation("{user} updated ControlSystem[{name}]", UserName, controlSystem.Name);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string message;
                    if (!ControlSystemExists(controlSystem.ControlSystemId))
                        message = $"Cannot edit control system because it no longer exists.";
                    else
                        message = $"Cannot edit control system because of a concurrency error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating ControlSystem[{id}] because of {message}", id, message);
                }
                catch (Exception ex)
                {
                    string message = $"Cannot edit control system because of a general error: {ex.Message}";
                    TempData["Error"] = message;
                    Logger.LogError("Error updating ControlSystem[{id}] because of {message}", id, message);
                }
                return RedirectToAction(nameof(Index));
            }

            //load the select (drop down) list
            ViewData["ManufacturingAreaId"] =
                await GetManufacturingAreasSelectListAsync(controlSystem.ManufacturingAreaId);

            return View(controlSystem);
        }

        // GET: ControlSystems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Logger.LogTrace("ControlSystems/Delete[GET] called with [{id}]", id);
            if (id == null)
                return NotFound();

            var controlSystem = await DatabaseContext
                .ControlSystems
                .Include(c => c.ManufacturingArea)
                .FirstOrDefaultAsync(m => m.ControlSystemId == id);
            if (controlSystem == null)
                return NotFound();

            return View(controlSystem);
        }

        // POST: ControlSystems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Logger.LogTrace("ControlSystems/Delete[POST] called with [{id}]", id);
            try
            {
                var controlSystem = await DatabaseContext.ControlSystems.FindAsync(id);
                DatabaseContext.ControlSystems.Remove(controlSystem);
                await DatabaseContext.SaveChangesAsync();
                TempData["Message"] = "Control system has been deleted.";
                Logger.LogInformation("{user} deleted ControlSystem[{name}]", UserName, controlSystem.Name);
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete a control system that is in use.";
                Logger.LogDebug("Error deleting control system that is in use.");
            }
            catch (SqlException ex)
            {
                TempData["Error"] = $"Cannot delete control system.  A database error occurred: {ex.Message}";
                Logger.LogError("Error deleting control system, SQL error: {message}", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Cannot delete control system.  A general error occurred: {ex.Message}";
                Logger.LogError("Error deleting control system, general error: {message}", ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ControlSystemExists(int id)
            => DatabaseContext.ControlSystems.Any(e => e.ControlSystemId == id);

    }
}
