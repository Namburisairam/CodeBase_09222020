using IncidentTracking.Data;
using IncidentTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

/// <remarks>
/// ================================================================================
/// MODULE:  BaseController.cs
///         
/// PURPOSE:
/// This class provides a common root on which other controllers are based.  It
/// gives a place to hold logic common to most controllers.
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-09-24
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-11-02  Initial version
/// Brad Robbins    2020-03-23  Changed activity type sorting
/// Brad Robbins    2020-04-15  Don't display obsolete control systems.
/// Brad Robbins    2020-04-29  Added T3Groups select list.
/// Brad Robbins    2020-06-15  Added logging support; revised select list functions; UserName
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Controllers
{
    public class BaseController : Controller, IDisposable
    {
        /// <summary>EF database context</summary>
        protected readonly ApplicationDbContext DatabaseContext;

        /// <summary>appsettings.json configuration</summary>
        protected readonly IConfiguration Configuration;

        /// <summary>AppSettings element within configuration</summary>
        protected readonly IConfigurationSection Settings;

        /// <summary>Default number of objects to show in tabular lists</summary>
        protected readonly int PageSize;

        /// <summary>Default date format</summary>
        protected readonly string DateFormat;

        /// <summary>Shared logger</summary>
        protected readonly ILogger Logger;

        /// <summary>Identity of current user</summary>
        protected string UserName { get => this.GetUserFullName(); }

        protected BaseController(
            ApplicationDbContext context, 
            IConfiguration config,
            ILogger logger)
        {
            DatabaseContext = context;
            Configuration = config;
            Settings = Configuration.GetSection("AppSettings");
            Logger = logger;
            
            PageSize = Settings.GetValue<int>("DefaultPageSize");
            DateFormat = Settings.GetValue<string>("DateFormat");
        }

        protected string GetUserFullName()
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var principal = UserPrincipal.FindByIdentity(context, User.Identity.Name);
                return $"{principal.Surname}, {principal.GivenName}";
            }
        }

        #region Common Select Lists

        /// <summary>Gets a select list of activity types</summary>
        /// <param name="selected">value of selected item</param>
        /// <returns>HTML select list</returns>
        protected async Task<IEnumerable<SelectListItem>> 
            GetActivityTypesSelectListAsync(char? selected = null)
        {
            Logger.LogTrace("GetActivityTypesSelectListAsync called with [{selected}]", selected);
            return new SelectList(
                await DatabaseContext
                    .ActivityTypes
                    .OrderBy(a => a.Ordinal)
                    .Select(a => new { a.ActivityTypeId, a.Description })
                    .ToListAsync(),
                "ActivityTypeId",
                "Description",
                selected);
        }

        /// <summary>Gets a select list of classifications</summary>
        /// <param name="selected">value of selected item</param>
        /// <returns>HTML select list</returns>
        protected async Task<IEnumerable<SelectListItem>> 
            GetClassificationsSelectListAsync(int? selected = null)
        {
            Logger.LogTrace("GetClassificationsSelectListAsync called with [{selected}]", selected);
            return new SelectList(
                await DatabaseContext
                    .Classifications
                    .OrderBy(c => c.Name)
                    .Select(c => new { c.ClassificationId, c.Name })
                    .ToListAsync(),
                "ClassificationId",
                "Name",
                selected);
        }

        /// <summary>Gets a select list of manufacturing areas</summary>
        /// <param name="selected">value of selected item</param>
        /// <returns>HTML select list</returns>
        protected async Task<IEnumerable<SelectListItem>> 
            GetManufacturingAreasSelectListAsync(int? selected = null)
        {
            Logger.LogTrace("GetManufacturingAreasSelectListAsync called with [{selected}]", selected);
            return new SelectList(
                await DatabaseContext
                    .ManufacturingAreas
                    .OrderBy(m => m.ManufacturingAreaId)
                    .Select(m => new { m.ManufacturingAreaId, m.Name })
                    .ToListAsync(),
                "ManufacturingAreaId",
                "Name",
                selected);
        }

        /// <summary>Gets a select list of control systems</summary>
        /// <param name="selected">value of selected item</param>
        /// <returns>HTML select list</returns>
        protected async Task<IEnumerable<SelectListItem>> 
            GetControlSystemsSelectListAsync(
            int? selected = null, int? mfgId = null)
        {
            Logger.LogTrace("GetControlSystemsSelectListAsync called with [{selected}]", selected);
            IQueryable<ControlSystem> list = DatabaseContext.ControlSystems.Where(c => c.IsObsolete.Value == false);

            if (mfgId.HasValue)
                list = list.Where(c => c.ManufacturingAreaId == mfgId);
            
            return new SelectList(
                await list
                    .OrderBy(c => c.Name)
                    .Select(c => new { c.ControlSystemId, c.Name })
                    .ToListAsync(),
                "ControlSystemId",
                "Name",
                selected);
        }
        
        /// <summary>Gets a select list of Tier 3 groups</summary>
        /// <param name="selected">value of selected item</param>
        /// <returns>HTML select list</returns>
        protected async Task<IEnumerable<SelectListItem>> 
            GetT3GroupsSelectListAsync(int? selected = null)
        {
            Logger.LogTrace("GetT3GroupsSelectListAsync called with [{selected}]", selected);
            return new SelectList(
                await DatabaseContext
                    .T3Groups
                    .OrderBy(g => g.Name)
                    .Select(g => new { g.T3GroupId, g.Name })
                    .ToListAsync(),
                "T3GroupId",
                "Name",
                selected);
        }
        #endregion Common Select Lists

        public new void Dispose() => DatabaseContext.Dispose();
    }
}