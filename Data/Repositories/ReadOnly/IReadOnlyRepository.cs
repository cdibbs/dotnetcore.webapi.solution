using Data.Utilities;
using Specifications;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Repositories.ReadOnly
{
    public interface IReadOnlyRepository
    {
        T FindOne<T>(
            ISpecification<T> spec,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity;

        IQueryable<T> FindAll<T>(
            ISpecification<T> spec,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity;

        IOrderedQueryable<T> Page<T>(
            ISpecification<T> spec,
            ISortFactory<T> sortFactory = null,
            int offsetPage = 0, int pageSize = 10,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity;
    }
}