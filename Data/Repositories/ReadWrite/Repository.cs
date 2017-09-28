using Data.Utilities;
using Microsoft.EntityFrameworkCore;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Data.QueryableExtensions;

namespace Data
{
    using QueryableExtensions;
    public class Repository : IRepository
    {
        static Repository()
        {
            QueryableExtensions.QueryableExtensions.Includer = new DbIncluder();
            QueryableExtensions.QueryableExtensions.FirstOrDefaulter = new DbFirstOrDefaulter();
        }

        public Repository (IDataContext dataContext, ISoftDeletedDataContext sdDataContext)
        {
            DataContext = dataContext;
            SdDataContext = sdDataContext;
        }

        public T FindOne<T>(
            ISpecification<T> spec,
            bool track = false, bool incSoftDel = false, params Expression<Func<T, object>>[] includes)
            where T: BaseEntity
        {
            return FindAll(spec, track, incSoftDel, includes).FirstOrDefault();
        }

        public IQueryable<T> FindAll<T>(
            ISpecification<T> spec,
            bool track = false, bool incSoftDel = false,
            params Expression<Func<T, object>>[] includes)
            where T : BaseEntity
        {
            var dc = incSoftDel ? SdDataContext : DataContext;
            var results = track
                ? dc.ISet<T>()
                : NoTracking(dc.ISet<T>());
            
            foreach (var include in includes) results = results.Include(include);
            return results.Where(spec.AsExpression());
        }

        public IEnumerable<T> Page<T>(
            ISpecification<T> spec,
            int offsetPage = 0, int pageSize = 10,
            ISortFactory<T, long> sortFactory = null,
            bool track = false, bool incSoftDel = false, 
            params Expression<Func<T, object>>[] includes)
            where T: BaseEntity
        {
            var filtered = FindAll(spec, track, incSoftDel, includes);

            IOrderedQueryable<T> results;
            if (sortFactory != null)
                results = sortFactory.ApplySorts(filtered);
            else
                results = filtered.OrderBy(t => t.Id);

            var query = results
                    .Skip(offsetPage * pageSize)
                    .Take(pageSize);
            return query.ToList();
        }

        public void AddEntity<T>(T o) where T : BaseEntity
        {
            DataContext.ISet<T>().Add(o);
        }

        /// <summary>
        /// Deletes the first specified entity, if it exists. Returns entity or null (if it doesn't exist).
        /// If specification doesn't match only one entity, then the first one found will be deleted.
        /// </summary>
        /// <typeparam name="T">The entity type to delete.</typeparam>
        /// <param name="spec">An entity specification.</param>
        /// <returns>The deleted entity or null (if no such entity).</returns>
        public T Delete<T>(ISpecification<T> spec, bool hardDelete = false, params Expression<Func<T, object>>[] includes) where T: BaseEntity
        {
            IQueryable<T> set = DataContext.ISet<T>();
            foreach (var include in includes) set = set.Include(include);
            var entity = set.FirstOrDefault(spec.AsExpression());
            if (entity != null)
            {
                if (! hardDelete)
                {
                    entity.IsDeleted = true;
                }
                else
                {
                    DataContext.ISet<T>().Remove(entity);
                }
            }
            return entity;
        }

        public virtual void Save(bool allDataContext = false)
        {
            (allDataContext ? SdDataContext : DataContext).Save();
        }

        /*public virtual IQueryable<T> Include<T>(IQueryable<T> collection, Expression<Func<T, object>> propertySpec) where T: class
        {
            return collection.Include(propertySpec);
        }*/

        public virtual IQueryable<T> NoTracking<T>(DbSet<T> collection) where T: class
        {
            return collection.AsNoTracking();
        }

        private IDataContext DataContext { get; set; }
        private ISoftDeletedDataContext SdDataContext { get; set; }
    }
}
