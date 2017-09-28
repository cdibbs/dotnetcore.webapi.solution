using Data.Utilities;
using Microsoft.EntityFrameworkCore;
using Specifications;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Repositories.ReadOnly
{
    public class ReadOnlyRepository<TKey> : IReadOnlyRepository<TKey>
        where TKey: IComparable
    {
        private IReadOnlyDataContext DataContext { get; set; }
        public ReadOnlyRepository(IReadOnlyDataContext dataContext)
        {
            DataContext = dataContext;
        }

        public T FindOne<T>(
            ISpecification<T> spec,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity<TKey>
        {
            return FindAll(spec, includes).FirstOrDefault();
        }

        public IQueryable<T> FindAll<T>(
            ISpecification<T> spec,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity<TKey>
        {
            var results = DataContext.ISet<T>().AsNoTracking();

            includes.ToList().ForEach(include => Include(results, include));
            return results.Where(spec.AsExpression());
        }

        public IOrderedQueryable<T> Page<T>(
            ISpecification<T> spec,
            ISortFactory<T, TKey> sortFactory,
            int offsetPage = 0, int pageSize = 10,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity<TKey>
        {
            var filtered = FindAll(spec, includes);
            var results = sortFactory.ApplySorts(filtered);

            var query = (IOrderedQueryable<T>) results
                .Skip(offsetPage*pageSize)
                .Take(pageSize);
            return query;
        }


        [ExcludeFromCodeCoverage] // Could rethink this. See ReadOnlyDataContext's use of static methods...
        public virtual IQueryable<T> Include<T>(IQueryable<T> collection, Expression<Func<T, object>> propertySpec) where T: class
        {
            return collection.Include(propertySpec);
        }
    }
}
