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
/// Created Date: 2019-05-28
/// Author:       Brad Robbins (brobbins@e2i.net)
///   
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2019-05-28  Initial version
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
        public virtual DbSet<Incident> Incidents { get; set; }

        /// <summary>Provides explicit table mapping and indexing.</summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Incident>().HasKey(i => new { i.IncidentId, i.Type });
        }
    }
}
