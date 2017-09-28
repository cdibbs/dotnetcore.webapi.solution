using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.QueryableExtensions
{
    public class DbFirstOrDefaulter : IFirstOrDefaulter
    {
        public T FirstOrDefault<T>(IQueryable<T> source, Expression<Func<T, bool>> path)
            where T : class
        {
            return System.Linq.Queryable.FirstOrDefault(source, path);
        }
    }
}
