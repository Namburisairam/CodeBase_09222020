using ES13Web.Data;
using ES13Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;

/// <remarks>
/// ================================================================================
/// MODULE:  BaseController.cs
///         
/// PURPOSE:
/// This class provides a common root on which other controllers are based.  It
/// gives a place to hold logic common to most controllers.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2020-06-15  Added logging support; revised select list functions; UserName
/// ================================================================================
/// </remarks>

namespace ES13Web.Controllers
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

        protected string GetUserFullName()
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var principal = UserPrincipal.FindByIdentity(context, User.Identity.Name);
                return $"{principal.Surname}, {principal.GivenName}";
            }
        }
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


        #region Common Select Lists

        /// <summary>Gets a select list of classifications</summary>
        /// <param name="selected">value of selected item</param>
        /// <returns>HTML select list</returns>
        protected async Task<IEnumerable<SelectListItem>> 
            GetClassificationsSelectListAsync(int? selected = null)
        {
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
        #endregion Common Select Lists

        public new void Dispose() => DatabaseContext.Dispose();
    }
}