﻿using System;
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
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>()
                    .Where(e => e.Id.Equals(id));

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query
                    .FirstOrDefault();
            }
        }

        public TEntidad Obtener(TId id,
            bool incluirBorrados = false,
            IEnumerable<string> cargarPropiedades = null)
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
            bool incluirBorrados = false,
            IEnumerable<Expression<Func<TEntidad, object>>> cargarPropiedades = null)
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
            using (var ctx = new TDbContext())
            {
                var query = ctx.Set<TEntidad>().AsQueryable();

                query = AplicarFiltroBorrados(query, incluirBorrados);

                return query
                    .OrdenarYPaginar(op)
                    .ToArray();
            }
        }

        public IEnumerable<TEntidad> ObtenerLista(
            bool incluirBorrados = false,
            OrdenYPaginacionParametros op = null, 
            IEnumerable<string> cargarPropiedades = null)
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
            bool incluirBorrados = false,
            OrdenYPaginacionParametros op = null,
            IEnumerable<Expression<Func<TEntidad, object>>> cargarPropiedades = null)
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
