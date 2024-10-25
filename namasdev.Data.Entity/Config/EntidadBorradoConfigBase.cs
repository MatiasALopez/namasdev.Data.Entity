using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using namasdev.Core.Entity;

namespace namasdev.Data.Entity.Config
{
    public abstract class EntidadBorradoConfigBase<TEntidad> : EntityTypeConfiguration<TEntidad>
        where TEntidad : class, IEntidadBorrado
    {
        public EntidadBorradoConfigBase()
        {
            Property(e => e.Borrado)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}
