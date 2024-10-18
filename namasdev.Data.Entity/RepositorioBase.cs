using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

using namasdev.Core.Validation;

namespace namasdev.Data.Entity
{
    public abstract class RepositorioBase<TDbContext, TEntidad, TId> : RepositorioSoloLecturaBase<TDbContext, TEntidad, TId>, IRepositorio<TEntidad, TId>
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
