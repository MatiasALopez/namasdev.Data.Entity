using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace namasdev.Data.Entity
{
    public class DbContextHelper<TDbContext>
        where TDbContext : DbContextBase, new()
    {
        public static void Config()
        {
            Database.SetInitializer<TDbContext>(null);
        }

        public static void Agregar<T>(T entidad)
            where T : class
        {
            Operacion(entidad, EntityState.Added);
        }

        public static void Actualizar<T>(T entidad)
            where T : class
        {
            Operacion(entidad, EntityState.Modified);
        }

        public static void Eliminar<T>(T entidad)
            where T : class
        {
            Operacion(entidad, EntityState.Deleted);
        }

        private static void Operacion<T>(T entidad, EntityState state)
            where T : class
        {
            using (var ctx = new TDbContext())
            {
                ctx.Attach(entidad, state);
                ctx.SaveChanges();
            }
        }

        public static void AgregarBatch<T>(IEnumerable<T> entidades,
            int tamañoBatch = 100) 
            where T : class
        {
            OperacionEnBatch(entidades, EntityState.Added, tamañoBatch);
        }

        public static void ActualizarBatch<T>(IEnumerable<T> entidades,
            int tamañoBatch = 100) 
            where T : class
        {
            OperacionEnBatch(entidades, EntityState.Modified, tamañoBatch);
        }

        public static void EliminarBatch<T>(IEnumerable<T> entidades,
            int tamañoBatch = 100) 
            where T : class
        {
            OperacionEnBatch(entidades, EntityState.Deleted, tamañoBatch);
        }

        public static void OperacionEnBatch<T>(IEnumerable<T> entidades, EntityState state,
            int tamañoBatch = 100) 
            where T : class
        {
            AccionEnBatch(entidades,
                (ctx, entidad) => ctx.Attach(entidad, state),
                tamañoBatch: tamañoBatch);
        }

        public static void ActualizarPropiedades<T>(T entidad, params string[] propiedades)
           where T : class
        {
            if (propiedades == null)
            {
                throw new ArgumentNullException(nameof(propiedades));
            }

            if (!propiedades.Any())
            {
                return;
            }

            using (var ctx = new TDbContext())
            {
                ctx.AttachPropiedadesModificadas(entidad, propiedades);

                ctx.Configuration.ValidateOnSaveEnabled = false;
                ctx.SaveChanges();
            }
        }

        public static void ActualizarPropiedadesEnBatch<T>(IEnumerable<T> entidades, string[] propiedades,
            int tamañoBatch = 100)
            where T : class
        {
            if (propiedades == null)
            {
                throw new ArgumentNullException(nameof(propiedades));
            }

            if (!propiedades.Any())
            {
                return;
            }

            AccionEnBatch(entidades,
                (ctx, entidad) => ctx.AttachPropiedadesModificadas(entidad, propiedades),
                tamañoBatch: tamañoBatch,
                crearCtx: () =>
                {
                    var ctx = new TDbContext();
                    ctx.Configuration.ValidateOnSaveEnabled = false;
                    return ctx;
                });
        }

        public static void AccionEnBatch<T>(IEnumerable<T> entidades, Action<TDbContext, T> accion,
            int tamañoBatch = 100,
            Func<TDbContext> crearCtx = null) 
            where T : class
        {
            if (entidades == null)
            {
                throw new ArgumentNullException(nameof(entidades));
            }

            if (!entidades.Any())
            {
                return;
            }

            if (tamañoBatch > 100)
            {
                tamañoBatch = 100;
            }

            crearCtx = crearCtx ?? (() => new TDbContext());

            var ctx = crearCtx();
            try
            {
                int cantidad = 0;
                foreach (var entidad in entidades)
                {
                    accion(ctx, entidad);
                    cantidad++;

                    if (cantidad == tamañoBatch)
                    {
                        ctx.SaveChanges();
                        ctx.Dispose();

                        ctx = crearCtx();

                        cantidad = 0;
                    }
                }

                if (cantidad > 0)
                {
                    ctx.SaveChanges();
                }
            }
            finally
            {
                if (ctx != null)
                {
                    ctx.Dispose();
                }
            }
        }
    }
}
