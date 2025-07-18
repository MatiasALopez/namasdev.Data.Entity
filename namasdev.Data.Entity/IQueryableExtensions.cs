using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace namasdev.Data.Entity
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> query, 
            Expression<Func<T, TProperty>> path, bool condition)
        {
            return condition
                ? query.Include(path)
                : query;
        }

        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query,
            IEnumerable<string> paths)
        {
            if (paths != null)
            {
                foreach (var p in paths)
                {
                    query = query.Include(p);
                }
            }

            return query;
        }

        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query,
            IEnumerable<Expression<Func<T, object>>> paths)
        {
            if (paths != null)
            {
                foreach (var p in paths)
                {
                    query = query.Include(p);
                }
            }

            return query;
        }

        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query,
            ICargaPropiedades<T> cargarPropiedades)
            where T : class
        {
            return cargarPropiedades != null
                ? IncludeMultiple(query, cargarPropiedades.CrearPaths())
                : query;
        }

        public static IQueryable<T> IncludeMultipleIf<T>(this IQueryable<T> query,
            IEnumerable<string> paths, bool condition)
        {
            return condition
                ? query.IncludeMultiple(paths)
                : query;
        }

        public static IQueryable<T> IncludeMultipleIf<T>(this IQueryable<T> query,
            IEnumerable<Expression<Func<T, object>>> paths, bool condition)
        {
            return condition
                ? query.IncludeMultiple(paths)
                : query;
        }

        public static IQueryable<T> IncludeMultipleIf<T>(this IQueryable<T> query,
            ICargaPropiedades<T> cargarPropiedades, bool condition)
            where T : class
        {
            return condition 
                ? IncludeMultiple(query, cargarPropiedades)
                : query;
        }
    }
}
