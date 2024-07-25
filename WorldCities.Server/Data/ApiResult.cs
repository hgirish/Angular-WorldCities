
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace WorldCities.Server.Data;

public class ApiResult<T>
{
    private ApiResult(
        List<T> data,
        int count,
        int pageIndex,
        int pageSize,
        string? sortColumn,
        string? sortOrder
        )
    {
        Data = data;
        TotalCount = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
        SortColumn = sortColumn;
        SortOrder = sortOrder;
        TotalPages = (int)Math.Ceiling(count/ (double)PageSize);
    }
    /// <summary>
    /// The data result
    /// </summary>
    public List<T> Data { get; }
    /// <summary>
    /// Total items count
    /// </summary>
    public int TotalCount { get; }
    /// <summary>
    /// Zero-based index of current page
    /// </summary>
    public int PageIndex { get; }
    /// <summary>
    /// Number of items contained in each page
    /// </summary>
    public int PageSize { get; }
    /// <summary>
    /// Sorting Column name (or null if none set)
    /// </summary>
    public string? SortColumn { get; set; }
    /// <summary>
    /// Sorting Order ("ASC", "DESC" or null if none set)
    /// </summary>
    public string? SortOrder { get; set; }

    /// <summary>
    /// Total pages count
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Pages and/or sorts a IQueryable source.
    /// </summary>
    /// <param name="source">An IQueryable source of generic 
    /// type</param>
    /// <param name="pageIndex">Zero-based current page index 
    /// (0 = first page)</param>
    /// <param name="pageSize">The actual size of each 
    /// page</param>
    /// <param name="sortColumn">The sorting column name</param>
    /// <param name="sortOrder">The sorting order ("ASC" or 
    /// "DESC")</param>
    /// <returns>
    /// A object containing the IQueryable paged/sorted result 
    /// and all the relevant paging/sorting navigation info.
    /// </returns>
    public static async Task<ApiResult<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize,
        string? sortColumn=null,
        string? sortOrder=null)
    {
        var count = await source.CountAsync();

        if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
        {
            sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC"
                ? "ASC"
                : "DESC";
            source = source.OrderBy(
                string.Format(
                    "{0} {1}",
                    sortColumn,
                    sortOrder)
                );
        }
        source = source
            .Skip(pageIndex*pageSize)
            .Take(pageSize);

        var data = await source.ToListAsync();

        return new ApiResult<T>(
            data,
            count,
            pageIndex,
            pageSize,
            sortColumn,
            sortOrder);

    }

    public static bool IsValidProperty(
        string propertyName,
        bool throwExceptionIfNotFound = true)
    {
        var prop = typeof(T).GetProperty(
            propertyName,
            BindingFlags.IgnoreCase |
            BindingFlags.Public |
            BindingFlags.Instance);
        if (prop == null && throwExceptionIfNotFound)
        {
            throw new NotSupportedException(
                string.Format(
                    $"ERROR: Property '{propertyName}' does not exist."));            
        }
        return prop != null;
    }

    public bool HasPreviousPage
    {
        get
        {
            return (PageIndex > 0);
        }
    }
    public bool HasNextPage
    {
        get
        {
            return ((PageIndex + 1) < TotalPages);
        }
    }
}
