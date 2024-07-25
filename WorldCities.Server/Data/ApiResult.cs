
using Microsoft.EntityFrameworkCore;

namespace WorldCities.Server.Data;

public class ApiResult<T>
{
    private ApiResult(
        List<T> data,
        int count,
        int pageIndex,
        int pageSize
        )
    {
        Data = data;
        TotalCount = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
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
    /// Total pages count
    /// </summary>
    public int TotalPages { get; }

    public static async Task<ApiResult<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize)
    {
        var count = await source.CountAsync();
        source = source
            .Skip(pageIndex*pageSize)
            .Take(pageSize);

        var data = await source.ToListAsync();

        return new ApiResult<T>(
            data,
            count,
            pageIndex,
            pageSize);

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
