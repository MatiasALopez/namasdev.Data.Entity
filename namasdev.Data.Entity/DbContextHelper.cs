using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using namasdev.Core.Entity;
using namasdev.Core.Validation;

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
            AttachYSaveChanges(entidad, EntityState.Added);
        }

        public static void AgregarBatch<T>(IEnumerable<T> entidades,
            int tamañoBatch = 100)
            where T : class
        {
            AttachEnBatch(entidades, EntityState.Added, 
                tamañoBatch: tamañoBatch);
        }

        public static void Actualizar<T>(T entidad,
            bool excluirPropiedadesCreado = true,
            bool excluirPropiedadesBorrado = true)
            where T : class
        {
            AttachYSaveChanges(
                entidad, 
                EntityState.Modified,
                propiedadesAExcluirEnModificacion: CrearPropiedadesAExcluirEnModificacion<T>(
                    excluirPropiedadesCreado: excluirPropiedadesCreado,
                    excluirPropiedadesBorrado: excluirPropiedadesBorrado));
        }

        public static void ActualizarBatch<T>(IEnumerable<T> entidades,
            bool excluirPropiedadesCreado = true, 
            bool excluirPropiedadesBorrado = true,
            int tamañoBatch = 100)
            where T : class
        {
            AttachEnBatch(
                entidades, 
                EntityState.Modified,
                propiedadesAExcluirEnModificacion: CrearPropiedadesAExcluirEnModificacion<T>(
                    excluirPropiedadesCreado: excluirPropiedadesCreado,
                    excluirPropiedadesBorrado: excluirPropiedadesBorrado),
                tamañoBatch: tamañoBatch);
        }

        public static void Eliminar<T>(T entidad)
            where T : class
        {
            AttachYSaveChanges(entidad, EntityState.Deleted);
        }

        public static void EliminarBatch<T>(IEnumerable<T> entidades,
            int tamañoBatch = 100)
            where T : class
        {
            AttachEnBatch(entidades, EntityState.Deleted, 
                tamañoBatch: tamañoBatch);
        }

        private static void AttachYSaveChanges<T>(T entidad, EntityState state,
            string[] propiedadesAExcluirEnModificacion = null)
            where T : class
        {
            using (var ctx = new TDbContext())
            {
                ctx.Attach(entidad, state, 
                    propiedadesAExcluirEnModificacion: propiedadesAExcluirEnModificacion);

                ctx.SaveChanges();
            }
        }

        public static void ActualizarPropiedades<T>(T entidad, params string[] propiedades)
           where T : class
        {
            Validador.ValidarArgumentListaRequeridaYThrow(propiedades, nameof(propiedades), validarNoVacia: false);

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
            Validador.ValidarArgumentListaRequeridaYThrow(propiedades, nameof(propiedades), validarNoVacia: false);

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

        private static void AttachEnBatch<T>(IEnumerable<T> entidades, EntityState state,
            string[] propiedadesAExcluirEnModificacion = null,
            int tamañoBatch = 100)
            where T : class
        {
            AccionEnBatch(entidades,
                (ctx, entidad) => ctx.Attach(entidad, state, propiedadesAExcluirEnModificacion),
                tamañoBatch: tamañoBatch);
        }

        private static void AccionEnBatch<T>(IEnumerable<T> entidades, Action<TDbContext, T> accion,
            int tamañoBatch = 100,
            Func<TDbContext> crearCtx = null) 
            where T : class
        {
            Validador.ValidarArgumentListaRequeridaYThrow(entidades, nameof(entidades), validarNoVacia: false);

            if (!entidades.Any())
            {
                return;
            }

            tamañoBatch = Math.Min(100, tamañoBatch);

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

        private static string[] CrearPropiedadesAExcluirEnModificacion<T>(bool excluirPropiedadesCreado, bool excluirPropiedadesBorrado)
        {
            var propiedades = new List<string>();

            if (excluirPropiedadesCreado
                && typeof(T).IsAssignableFrom(typeof(IEntidadCreado)))
            {
                propiedades.AddRange(new[]
                {
                    nameof(IEntidadCreado.CreadoPor),
                    nameof(IEntidadCreado.CreadoFecha)
                });
            }
            if (excluirPropiedadesBorrado
                && typeof(T).IsAssignableFrom(typeof(IEntidadBorrado)))
            {
                propiedades.AddRange(new[]
                {
                    nameof(IEntidadBorrado.BorradoPor),
                    nameof(IEntidadBorrado.BorradoFecha)
                });
            }

            return propiedades.Any()
                ? propiedades.ToArray()
                : null;
        }
    }
}
