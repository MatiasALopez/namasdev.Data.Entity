﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

using namasdev.Core.Entity;
using namasdev.Core.Validation;

namespace namasdev.Data.Entity
{
    public class Repositorio<TDbContext, TEntidad, TId> : RepositorioSoloLectura<TDbContext, TEntidad, TId>, IRepositorio<TEntidad, TId>
        where TDbContext : DbContextBase, new()
        where TEntidad : class, IEntidad<TId>, new()
        where TId : IEquatable<TId>
    {
        private const int TAMAÑO_BATCH_DEFAULT = 100;

        public virtual void Agregar(IEnumerable<TEntidad> entidades,
            int tamañoBatch = TAMAÑO_BATCH_DEFAULT)
        {
            DbContextHelper<TDbContext>.AgregarBatch(entidades,
                tamañoBatch: tamañoBatch);
        }

        public virtual void Agregar(TEntidad entidad)
        {
            DbContextHelper<TDbContext>.Agregar(entidad);
        }

        public virtual void Actualizar(IEnumerable<TEntidad> entidades,
            int tamañoBatch = TAMAÑO_BATCH_DEFAULT)
        {
            DbContextHelper<TDbContext>.ActualizarBatch(entidades, 
                tamañoBatch: tamañoBatch);
        }

        public virtual void Actualizar(TEntidad entidad)
        {
            DbContextHelper<TDbContext>.Actualizar(entidad);
        }

        public virtual void ActualizarPropiedades(IEnumerable<TEntidad> entidades, 
            int tamañoBatch = TAMAÑO_BATCH_DEFAULT, 
            params string[] propiedades)
        {
            DbContextHelper<TDbContext>.ActualizarPropiedadesEnBatch(entidades, propiedades, tamañoBatch);
        }

        public virtual void ActualizarPropiedades(TEntidad entidad, params string[] propiedades)
        {
            DbContextHelper<TDbContext>.ActualizarPropiedades(entidad, propiedades);
        }

        public virtual void ActualizarDatosBorrado(TEntidad entidad)
        {
            var e = entidad as IEntidadBorrado;
            if (e == null)
            {
                return;
            }
                
            DbContextHelper<TDbContext>.ActualizarPropiedades(entidad, 
                nameof(e.BorradoPor),
                nameof(e.BorradoFecha));
        }

        public virtual void ActualizarDatosBorrado(IEnumerable<TEntidad> entidades, int tamañoBatch = TAMAÑO_BATCH_DEFAULT)
        {
            if (typeof(TEntidad) is IEntidadBorrado)
            {
                DbContextHelper<TDbContext>.ActualizarPropiedadesEnBatch(entidades, 
                    new[] {
                        nameof(IEntidadBorrado.BorradoPor),
                        nameof(IEntidadBorrado.BorradoFecha)
                    }, 
                    tamañoBatch);
            }
        }

        public virtual void Eliminar(IEnumerable<TEntidad> entidades,
            int tamañoBatch = TAMAÑO_BATCH_DEFAULT)
        {
            DbContextHelper<TDbContext>.EliminarBatch(entidades, 
                tamañoBatch: tamañoBatch);
        }

        public virtual void Eliminar(TEntidad entidad)
        {
            DbContextHelper<TDbContext>.Eliminar(entidad);
        }

        public virtual void EliminarPorId(IEnumerable<TId> ids, 
            int tamañoBatch = TAMAÑO_BATCH_DEFAULT)
        {
            Validador.ValidarArgumentRequeridoYThrow(ids, nameof(ids));
            var entidades = ids
                .Select(id => new TEntidad { Id = id })
                .ToArray();
            DbContextHelper<TDbContext>.EliminarBatch(entidades, 
                tamañoBatch: tamañoBatch);
        }

        public virtual void EliminarPorId(TId id)
        {
            DbContextHelper<TDbContext>.Eliminar(new TEntidad { Id = id });
        }

        protected DbSet<TEntidad> EntidadSet(TDbContext ctx)
        {
            return ctx.Set<TEntidad>();
        }

        protected TDbContext CrearContext()
        {
            return new TDbContext();
        }
    }
}
