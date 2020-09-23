using ES13Web.Models;
using Microsoft.EntityFrameworkCore;

/// <remarks>
/// ================================================================================
/// MODULE:  ApplicationDbContext.cs
///         
/// PURPOSE:
/// This class is the EF representation of the database.  Much of the relational
/// aspects of the database is translated automatically by EF.
///         
/// Copyright:    ©2019 by E2i, Inc.
/// Created Date: 2019-04-26
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-04-26  Initial version
/// Brad Robbins    2020-06-15  Added IncidentMetrics and IncidentLog
/// ================================================================================
/// </remarks>

namespace ES13Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructors
        public ApplicationDbContext() {}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        #endregion Constructors

        /// <summary>Gets/sets data to the Classifications table.</summary>
        public virtual DbSet<Classification> Classifications { get; set; }

        /// <summary>Gets/sets data to the ManufacturingAreas table.</summary>
        public virtual DbSet<ManufacturingArea> ManufacturingAreas { get; set; }

        /// <summary>Gets/sets data to the Incidents table.</summary>
        public virtual DbSet<Incident> Incidents { get; set; }

        public virtual DbSet<IncidentMetric> IncidentMetrics { get; set; }
        
        /// <summary>Gets/sets incident Index data.</summary>
        public virtual DbSet<IncidentIndexViewModel> IncidentLog { get; set; }

        /// <summary>Provides explicit table mapping and indexing.</summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Classification>().ToTable("Classifications");
            builder.Entity<ManufacturingArea>().ToTable("ManufacturingAreas");
            builder.Entity<Incident>().ToTable("Incidents");
            builder.Entity<IncidentMetric>().HasNoKey();
            builder.Entity<IncidentIndexViewModel>().HasNoKey();
        }
    }
}
