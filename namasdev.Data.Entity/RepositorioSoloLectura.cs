using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using namasdev.Core.Entity;
using namasdev.Core.Reflection;
using namasdev.Core.Types;

namespace namasdev.Data.Entity
{
    public class RepositorioSoloLectura<TDbContext, TEntidad, TId> : IRepositorioSoloLectura<TEntidad, TId>
        where TDbContext : DbContextBase, new()
        where TEntidad : class, IEntidad<TId>, new()
        where TId : IEquatable<TId>
    {
        private readonly bool _entidadImplementaBorrado;

        public RepositorioSoloLectura()
        {
            _entidadImplementaBorrado = ReflectionHelper.ClaseImplementaInterfaz<TEntidad, IEntidadBorrado>(); 
        }

        public TEntidad Obtener(TId id)
        {
            return Obtener(id, 
                incluirBorrados: false);
        }

        public TEntidad Obtener(TId id, bool incluirBorrados)
        {
            return Obtener(id,
                cargarPropiedades: (IEnumerable<string>)null,
                incluirBorrados: incluirBorrados);
        }

        public TEntidad Obtener(TId id,
            IEnumerable<string> cargarPropiedades,
            bool incluirBorrados = false)
        {
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades)
                    .Where(e => e.Id.Equals(id));

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query.FirstOrDefault();
            }
        }

        public TEntidad Obtener(TId id,
            IEnumerable<Expression<Func<TEntidad, object>>> cargarPropiedades,
            bool incluirBorrados = false)
        {
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades)
                    .Where(e => e.Id.Equals(id));

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query.FirstOrDefault();
            }
        }

        public TEntidad Obtener(TId id,
            ICargaPropiedades<TEntidad> cargarPropiedades,
            bool incluirBorrados = false)
        {
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades)
                    .Where(e => e.Id.Equals(id));

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query.FirstOrDefault();
            }
        }

        public IEnumerable<TEntidad> ObtenerLista(
            OrdenYPaginacionParametros op = null)
        {
            return ObtenerLista(
                incluirBorrados: false,
                op);
        }

        public IEnumerable<TEntidad> ObtenerLista(
            bool incluirBorrados,
            OrdenYPaginacionParametros op = null)
        {
            return ObtenerLista(
                incluirBorrados: incluirBorrados,
                op: op,
                cargarPropiedades: (IEnumerable<string>)null);
        }

        public IEnumerable<TEntidad> ObtenerLista(
            IEnumerable<string> cargarPropiedades,
            bool incluirBorrados = false,
            OrdenYPaginacionParametros op = null)
        {
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades);

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query
                    .OrdenarYPaginar(op)
                    .ToArray();
            }
        }

        public IEnumerable<TEntidad> ObtenerLista(
            IEnumerable<Expression<Func<TEntidad, object>>> cargarPropiedades,
            bool incluirBorrados = false,
            OrdenYPaginacionParametros op = null)
        {
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades);

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query
                    .OrdenarYPaginar(op)
                    .ToArray();
            }
        }

        public IEnumerable<TEntidad> ObtenerLista(
            ICargaPropiedades<TEntidad> cargarPropiedades,
            bool incluirBorrados = false,
            OrdenYPaginacionParametros op = null)
        {
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>()
                    .IncludeMultiple(cargarPropiedades);

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query
                    .OrdenarYPaginar(op)
                    .ToArray();
            }
        }

        public bool ExistePorId(TId id,
            bool incluirBorrados = false)
        {
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>()
                    .Where(e => e.Id.Equals(id));

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query.Any();
            }
        }

        private IQueryable<TEntidad> AplicarFiltroBorrados(IQueryable<TEntidad> query, bool incluirBorrados)
        {
            if (!_entidadImplementaBorrado || incluirBorrados)
            {
                return query;
            }

            return ((IQueryable<IEntidadBorrado>)query)
                .Where(e => !e.Borrado)
                .Cast<TEntidad>();
        }
    }
}
