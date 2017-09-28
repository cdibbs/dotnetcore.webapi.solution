using Data.Utilities;
using Specifications;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Repositories.ReadOnly
{
    public interface IReadOnlyRepository<TKey>
        where TKey: IComparable
    {
    T FindOne<T>(
        ISpecification<T> spec,
        params Expression<Func<T, object>>[] includes)
        where T : class, IEntity<TKey>;

    IQueryable<T> FindAll<T>(
        ISpecification<T> spec,
        params Expression<Func<T, object>>[] includes)
        where T : class, IEntity<TKey>;

    IOrderedQueryable<T> Page<T>(
        ISpecification<T> spec,
        ISortFactory<T, TKey> sortFactory = null,
        int offsetPage = 0, int pageSize = 10,
        params Expression<Func<T, object>>[] includes)
        where T : class, IEntity<TKey>;
    }
}