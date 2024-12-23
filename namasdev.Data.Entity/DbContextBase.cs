﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

using namasdev.Core.Entity;

namespace namasdev.Data.Entity
{
    public class DbContextBase : DbContext
    {
        public DbContextBase(
            string nameOrConnectionString,
            bool lazyLoadingEnabled = false,
            int? commandTimeout = null)
            : base(nameOrConnectionString)
        {
            Configuration.LazyLoadingEnabled = lazyLoadingEnabled;
            Database.CommandTimeout = commandTimeout;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.AddFromAssembly(GetType().Assembly);
        }

        public void Attach<T>(T entidad, EntityState state,
            string[] propiedadesAExcluirEnModificacion = null)
            where T : class
        {
            Set<T>().Attach(entidad);
            var entry = Entry(entidad);
            entry.State = state;

            if (state == EntityState.Modified)
            {
                if (entidad is IEntidadCreado 
                    && propiedadesAExcluirEnModificacion == null)
                {
                    propiedadesAExcluirEnModificacion = new[]
                    {
                        nameof(IEntidadCreado.CreadoPor),
                        nameof(IEntidadCreado.CreadoFecha),
                    };
                }

                EstablecerStatusModificadoDePropiedades(entry, false, propiedadesAExcluirEnModificacion);
            }
        }

        public void AttachPropiedadesModificadas<T>(T entidad, string[] propiedades)
            where T : class
        {
            Set<T>().Attach(entidad);
            EstablecerStatusModificadoDePropiedades(Entry(entidad), true, propiedades);
        }

        public void EstablecerStatusModificadoDePropiedades<T>(DbEntityEntry<T> entry, bool modificado, string[] propiedades)
            where T : class
        {
            if (propiedades != null)
            {
                foreach (string p in propiedades)
                {
                    entry.Property(p).IsModified = modificado;
                }
            }
        }

        public TResultado EjecutarQueryYObtenerValor<TResultado>(string query,
            params object[] parametros)
        {
            return Database
                .SqlQuery<TResultado>(query, parametros)
                .FirstOrDefault();
        }

        public List<TResultado> EjecutarQueryYObtenerLista<TResultado>(string query,
            params object[] parametros)
        {
            return Database
                .SqlQuery<TResultado>(query, parametros)
                .ToList();
        }

        public void EjecutarComando(string comando, 
            DbParameter[] parametros = null,
            TransactionalBehavior transactionalBehavior = TransactionalBehavior.DoNotEnsureTransaction)
        {
            Database.ExecuteSqlCommand(transactionalBehavior, comando, parametros);
        }

        public TResultado EjecutarComandoYObtenerValor<TResultado>(string comando,
            Func<DbDataReader, TResultado> mapeoResultado,
            IEnumerable<DbParameter> parametros = null)
            where TResultado : class
        {
            TResultado resultado = null;

            EjecutarComandoEnNuevaConexion(
                comando,
                (cmd) => {
                    using (var reader = cmd.ExecuteReader())
                    {
                        resultado = mapeoResultado(reader);
                    }
                },
                parametros: parametros);

            return resultado;
        }

        private void EjecutarComandoEnNuevaConexion(string comando, Action<DbCommand> action,
            IEnumerable<DbParameter> parametros = null)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = comando;

                if (Database.CommandTimeout.HasValue)
                {
                    cmd.CommandTimeout = Database.CommandTimeout.Value;
                }

                if (parametros != null)
                {
                    cmd.Parameters.AddRange(parametros.ToArray());
                }

                try
                {
                    Database.Connection.Open();

                    action(cmd);
                }
                finally
                {
                    Database.Connection.Close();
                }
            }
        }

        public ObjectResult<TEntidad> MapearReaderAEntidad<TEntidad>(DbDataReader reader)
        {
            return ((IObjectContextAdapter)this)
                .ObjectContext
                .Translate<TEntidad>(reader);
        }
    }
}
