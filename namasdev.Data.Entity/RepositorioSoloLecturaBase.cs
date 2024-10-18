﻿using System;
using System.Collections.Generic;
using System.Linq;

using namasdev.Core.Types;

namespace namasdev.Data.Entity
{
    public abstract class RepositorioSoloLecturaBase<TDbContext, TEntidad, TId> : IRepositorioSoloLectura<TEntidad,TId>
        where TDbContext : DbContextBase, new()
        where TEntidad : class, IEntidad<TId>, new()
        where TId : IEquatable<TId>
    {
        public TEntidad Obtener(TId id)
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>().Find(id);
            }
        }

        public IEnumerable<TEntidad> ObtenerLista(OrdenYPaginacionParametros op = null)
        {
            using (var ctx = new TDbContext())
            {
                return ctx.Set<TEntidad>()
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
