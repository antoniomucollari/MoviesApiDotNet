using MyDotNet9Api.DTOs;

namespace MyDotNet9Api.Utilities;

public static class IQuryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
    {
        return queryable
            .Skip((pagination.Page - 1) * pagination.RecordsPerPage).Take(pagination.RecordsPerPage);
    }
    
}   