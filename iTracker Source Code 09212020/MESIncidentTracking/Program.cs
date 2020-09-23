using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

/// <remarks>
/// ================================================================================
/// MODULE:  Program.cs
///         
/// PURPOSE:
/// This class is the main entry point when the application starts.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2020-06-15  Updated for .NET Core 3.1 and logging
/// ================================================================================
/// </remarks>

namespace ES13Web
{
    public class Program
    {
        public static IConfiguration Configuration 
        {
            get => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        //make the web server, calling Startup class
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseIISIntegration()
                .UseStartup<Startup>();

        //Main application entry point
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.EventLog("MES IncidentTracker", manageEventSource: true)
                .CreateLogger();
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            try
            {
                Log.Information("Website starting up [{environment}]...", env);
                CreateWebHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
            }
            finally
            {
                Log.Information("Website going down.");
                Log.CloseAndFlush();
            }
        }
    }
}
