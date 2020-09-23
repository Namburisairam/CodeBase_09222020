using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <remarks>
/// ================================================================================
/// MODULE:  PaginatedList.cs
///         
/// PURPOSE: This class facilitates paging on index views.
///         
/// <see cref="https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-2.0"/>
///
/// --------------------------------------------------------------------------------
/// Revision History:			
/// Author          Date        Description
/// Brad Robbins	2018-10-30  Initial version
/// ================================================================================
/// </remarks>

namespace IncidentTracking
{
    public class PaginatedList<T> : List<T>
    {
        /// <summary>Gets the page index</summary>
        public int PageIndex { get; private set; }

        /// <summary>Gets the total number of pages</summary>
        public int TotalPages { get; private set; }

        /// <summary>Determines if a previous page is available</summary>
        public bool HasPreviousPage { get => (PageIndex > 1); }

        /// <summary>Determines if a next page is available</summary>
        public bool HasNextPage { get => (PageIndex < TotalPages); }

        /// <summary>Primary constructor</summary>
        /// <param name="items">list of items</param>
        /// <param name="count">total number of items</param>
        /// <param name="pageIndex">which page are we on?</param>
        /// <param name="pageSize">page size</param>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        /// <summary>Creates a list based on the source list provided.</summary>
        /// <param name="source">source list of items</param>
        /// <param name="pageIndex">which page are we on?</param>
        /// <param name="pageSize">how many items per page?</param>
        /// <returns>PaginatedList</returns>
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
