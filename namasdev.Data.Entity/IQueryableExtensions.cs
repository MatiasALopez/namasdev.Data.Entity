using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Linq;

namespace namasdev.Data.Entity
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> query, Expression<Func<T, TProperty>> path, bool condition)
        {
            return condition
                ? query.Include(path)
                : query;
        }
    }
}
