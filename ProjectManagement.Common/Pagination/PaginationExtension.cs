using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ProjectManagement.Common.Pagination
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<T>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }
    }

    public static class SortingExtensions
    {
        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> query,
            string? sortBy,
            string? sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query; // no sorting, return as is

            bool descending = !string.IsNullOrWhiteSpace(sortDirection) &&
                              sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(param, sortBy);
            var lambda = Expression.Lambda(property, param);

            string methodName = descending ? "OrderByDescending" : "OrderBy";

            var result = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName
                            && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type)
                .Invoke(null, new object[] { query, lambda });

            return (IQueryable<T>)result;
        }
    }
}
