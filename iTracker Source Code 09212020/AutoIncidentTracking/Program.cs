using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

/// <remarks>
/// ================================================================================
/// MODULE:  Program.cs
///
/// PURPOSE:
/// This is the main entry point for the entire application.
///
/// COPYRIGHT:    ©2020 by E2i, Inc.
/// CREATED DATE: 2020-06-11
/// AUTHOR:       Brad Robbins (brobbins@e2i.net)
///
/// --------------------------------------------------------------------------------
/// REVISION HISTORY:
/// AUTHOR		DATE		DESCRIPTION
/// B.Robbins	2020-06-11	Updated to use configuration settings and start logger
/// B.Robbins   2020-06-15  Updated to use event logging
/// ================================================================================
/// </remarks>

namespace IncidentTracking
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.EventLog("PCS IncidentTracker", manageEventSource: true)
                .CreateLogger();
                
            try
            {
                Log.Information("Website starting up...");
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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseIISIntegration()
                .UseStartup<Startup>();
    }
}
