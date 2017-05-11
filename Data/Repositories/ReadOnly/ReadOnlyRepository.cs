using Data.Utilities;
using Microsoft.EntityFrameworkCore;
using Specifications;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Repositories.ReadOnly
{
    public class ReadOnlyRepository : IReadOnlyRepository
    {
        private IReadOnlyDataContext DataContext { get; set; }
        public ReadOnlyRepository(IReadOnlyDataContext dataContext)
        {
            DataContext = dataContext;
        }

        public T FindOne<T>(
            ISpecification<T> spec,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity
        {
            return FindAll(spec, includes).FirstOrDefault();
        }

        public IQueryable<T> FindAll<T>(
            ISpecification<T> spec,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity
        {
            var results = DataContext.ISet<T>().AsNoTracking();

            includes.ToList().ForEach(include => Include(results, include));
            return results.Where(spec.AsExpression());
        }

        // LOL never done it this way, before. We'll see...
        public IOrderedQueryable<T> Page<T>(
            ISpecification<T> spec,
            ISortFactory<T> sortFactory,
            int offsetPage = 0, int pageSize = 10,
            params Expression<Func<T, object>>[] includes)
            where T : class, IEntity
        {
            var filtered = FindAll(spec, includes);
            var results = sortFactory.ApplySorts(filtered);

            var query = (IOrderedQueryable<T>) results
                .Skip(offsetPage*pageSize)
                .Take(pageSize);
            //var sql = ((System.Data.Entity.Infrastructure.DbQuery<V_Population>)query).ToString();
            return query;
        }

        public virtual IQueryable<T> Include<T>(IQueryable<T> collection, Expression<Func<T, object>> propertySpec) where T: class
        {
            return collection.Include(propertySpec);
        }
    }
}
