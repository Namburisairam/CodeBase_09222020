using ES13Web.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

/// <remarks>
/// ================================================================================
/// MODULE:  Startup.cs
///         
/// PURPOSE:
/// This class brings up all the dependent services and configures dependency 
/// injection.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2020-06-15  Updates for .NET Core 3.1 and logging
/// ================================================================================
/// </remarks>

namespace ES13Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //Reference to appsettings.json
        public IConfiguration Configuration { get; }

        //This method gets called by the runtime. Use this method to 
        //add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.Configure<CookiePolicyOptions>(options =>
            {
                //This lambda determines whether user consent for non-essential 
                //cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                    policy => policy.RequireRole("cslg1.cslg.net\\KANBPL_APP_ENG_ES13_Admin"));
            });
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(connectionString));

        }

        //This method gets called by the runtime. Use this method to configure 
        //the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
               app.UseExceptionHandler("/Error");
               //app.UseHsts();
            }

            app.UseStatusCodePages();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseCookiePolicy();
        }
    }
}
