using System;
using System.Linq;
using System.Linq.Expressions;

namespace Data.QueryableExtensions
{
    public interface IFirstOrDefaulter
    {
        T FirstOrDefault<T>(IQueryable<T> source, Expression<Func<T, bool>> path)
            where T : class;
    }
}