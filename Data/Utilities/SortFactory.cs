using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Utilities
{
    public class SortFactory<T, TKey> : ISortFactory<T, TKey>
        where T: IEntity<TKey>
        where TKey: IComparable
    {
        public SortSpecification[] Sorts { get; set; }

        // Selectors must be dynamic, at the time of writing (2017-09-14). EF will not work with expressions of return type object.
        public Dictionary<string, dynamic> Selectors { get; set; }
        public Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> DefaultSort { get; set; }
        public SortFactory(
            SortSpecification[] sorts,
            Dictionary<string, dynamic> availableSelectors,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> defaultSort = null)
        {
            this.Sorts = sorts;
            this.Selectors = availableSelectors;
            this.DefaultSort = defaultSort ?? (t => t.OrderBy(q => q.Id));
        }

        public IOrderedQueryable<T> ApplySorts(IQueryable<T> queryable)
        {
            if (Sorts.Length == 0) return DefaultSort.Compile()(queryable);

            var sorted = ApplySort(queryable, Sorts.First());
            foreach (var sort in this.Sorts.Skip(1))
            {
                sorted = ApplyNextSort(sorted, sort);
            }
            return sorted;
        }

        public IOrderedQueryable<T> ApplySort(IQueryable<T> queryable, SortSpecification sort)
        {
            if (sort.Direction == SortDirection.Ascending)
            {
                return Queryable.OrderBy(queryable, GetLambda(sort.Property));
            }

            return Queryable.OrderByDescending(queryable, GetLambda(sort.Property));
        }

        public IOrderedQueryable<T> ApplyNextSort(IOrderedQueryable<T> queryable, SortSpecification sort)
        {
            if (sort.Direction == SortDirection.Ascending)
            {
                return Queryable.ThenBy(queryable, GetLambda(sort.Property));
            }

            return Queryable.ThenByDescending(queryable, GetLambda(sort.Property));
        }

        public dynamic GetLambda(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName) && this.Selectors.ContainsKey(fieldName))
            {
                return this.Selectors[fieldName];
            }

            throw new ArgumentException($"Field '{fieldName}' does not exist in provided selectors.");
        }
    }
}