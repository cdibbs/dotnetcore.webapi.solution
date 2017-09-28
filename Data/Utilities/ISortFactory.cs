using System;
using System.Linq;

namespace Data.Utilities
{
    public interface ISortFactory<T, TKey>
        where T : IEntity<TKey>
        where TKey: IComparable
    {
        IOrderedQueryable<T> ApplySorts(IQueryable<T> queryable);
    }
}