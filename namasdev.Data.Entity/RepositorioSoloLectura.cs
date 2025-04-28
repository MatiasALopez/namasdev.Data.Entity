using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using namasdev.Core.Entity;
using namasdev.Core.Types;

namespace namasdev.Data.Entity
{
    public class RepositorioSoloLectura<TDbContext, TEntidad, TId> : IRepositorioSoloLectura<TEntidad, TId>
        where TDbContext : DbContextBase, new()
        where TEntidad : class, IEntidad<TId>, new()
        where TId : IEquatable<TId>
    {
        public TEntidad Obtener(TId id)
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>()
                    .FirstOrDefault(e => e.Id.Equals(id));
            }
        }

        public TEntidad Obtener(TId id, 
            IEnumerable<string> cargarPropiedades = null)
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades)
                    .FirstOrDefault(e => e.Id.Equals(id));
            }
        }

        public TEntidad Obtener(TId id, 
            IEnumerable<Expression<Func<TEntidad, object>>> cargarPropiedades = null)
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades)
                    .FirstOrDefault(e => e.Id.Equals(id));
            }
        }

        public IEnumerable<TEntidad> ObtenerLista()
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>()
                    .ToArray();
            }
        }

        public IEnumerable<TEntidad> ObtenerLista(
            OrdenYPaginacionParametros op = null, 
            IEnumerable<string> cargarPropiedades = null)
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades)
                    .OrdenarYPaginar(op)
                    .ToArray();
            }
        }

        public IEnumerable<TEntidad> ObtenerLista(
            OrdenYPaginacionParametros op = null,
            IEnumerable<Expression<Func<TEntidad, object>>> cargarPropiedades = null)
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades)
                    .OrdenarYPaginar(op)
                    .ToArray();
            }
        }

        public bool ExistePorId(TId id)
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>()
                    .Any(e => e.Id.Equals(id));
            }
        }
    }
}
