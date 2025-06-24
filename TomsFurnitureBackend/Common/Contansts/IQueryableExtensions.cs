using Microsoft.EntityFrameworkCore;
using TomsFurnitureBackend.Common.Models;

public static class IQueryableExtensions
{
    public static async Task<PaginationModel<T>> ToPagedAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationModel<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
