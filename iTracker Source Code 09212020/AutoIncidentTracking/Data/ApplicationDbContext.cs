using IncidentTracking.Models;
using Microsoft.EntityFrameworkCore;

/// <remarks>
/// ================================================================================
/// MODULE:  ApplicationDbContext.cs
///         
/// PURPOSE:
/// This class is the EF representation of the database.  Much of the relational
/// aspects of the database is translated automatically by EF.
///         
/// Copyright:    ©2018 by E2i, Inc.
/// Created Date: 2018-09-24
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-10-29  Initial version
/// Brad Robbins    2020-04-29  Added T3Groups table
/// Brad Robbins    2020-06-12  Added IncidentMetrics, IncidentIndexViewModel
/// ================================================================================
/// </remarks>

namespace IncidentTracking.Data
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructors
        public ApplicationDbContext() {}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        #endregion Constructors

        /// <summary>Gets/sets data to the ActivityTypes table.</summary>
        public virtual DbSet<ActivityType> ActivityTypes { get; set; }

        /// <summary>Gets/sets data to the Classifications table.</summary>
        public virtual DbSet<Classification> Classifications { get; set; }

        /// <summary>Gets/sets data to the ManufacturingAreas table.</summary>
        public virtual DbSet<ManufacturingArea> ManufacturingAreas { get; set; }

        /// <summary>Gets/sets data to the ControlSystemstable.</summary>
        public virtual DbSet<ControlSystem> ControlSystems { get; set; }

        /// <summary>Gets/sets data to the Incidents table.</summary>
        public virtual DbSet<Incident> Incidents { get; set; }
        
        /// <summary>Gets/sets data to the T3Groups table.</summary>
        public virtual DbSet<T3Group> T3Groups { get; set; }

        /// <summary>Gets/sets incident metric data for the HomeController.</summary>
        public virtual DbSet<IncidentMetric> IncidentMetrics { get; set; }
        
        /// <summary>Gets/sets incident Index data.</summary>
        public virtual DbSet<IncidentIndexViewModel> IncidentLog { get; set; }

        /// <summary>Provides explicit table mapping and indexing.</summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityType>().ToTable("ActivityTypes");
            builder.Entity<Classification>().ToTable("Classifications");
            builder.Entity<T3Group>().ToTable("T3Groups");
            builder.Entity<ManufacturingArea>().ToTable("ManufacturingAreas");
            builder.Entity<ControlSystem>().ToTable("ControlSystems")
                .HasOne(c => c.ManufacturingArea)
                .WithMany(m => m.ControlSystems)
                .HasForeignKey(c => c.ManufacturingAreaId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Incident>().ToTable("Incidents");
            builder.Entity<IncidentMetric>().HasNoKey();
            builder.Entity<IncidentIndexViewModel>().HasNoKey();
        }
    }
}
