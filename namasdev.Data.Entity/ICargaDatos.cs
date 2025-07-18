using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace namasdev.Data.Entity
{
    public interface ICargaDatos<TEntidad>
    {
        IEnumerable<Expression<Func<TEntidad, object>>> CrearPaths();
    }
}
