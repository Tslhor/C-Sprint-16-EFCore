using Microsoft.EntityFrameworkCore;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedListViewModel<T>> ToPagedListAsync<T>(
            this IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            }

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            pageNumber = Math.Clamp(pageNumber, 1, totalPages);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedListViewModel<T>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };
        }
    }
}
