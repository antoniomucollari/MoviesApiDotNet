using Microsoft.EntityFrameworkCore;

namespace MyDotNet9Api.Utilities;

public static class HttpContextExtensions
{
    public async static Task InsertPaginationParameterHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
    {
        if (httpContext is null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        double count = await queryable.CountAsync();
        httpContext.Response.Headers.Append("total-records-count", count.ToString());
    }
}