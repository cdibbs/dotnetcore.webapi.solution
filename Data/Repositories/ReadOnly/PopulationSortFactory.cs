using Data.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Repositories.ReadOnly.Utility
{
    public class PopulationSortFactory<T> : ISortFactory<T> where T: ViewEntity
    {
        public SortSpecification[] Sorts { get; set; }
        public Dictionary<string, Expression<Func<T, object>>> Selectors { get; set; }
        public PopulationSortFactory(SortSpecification[] sorts, Dictionary<string, Expression<Func<T, object>>> availableSelectors)
        {
            this.Sorts = sorts;
            this.Selectors = availableSelectors;
        }

        public IOrderedQueryable<T> ApplySorts(IQueryable<T> queryable)
        {
            if (Sorts.Length == 0) return queryable.OrderBy(t => t.Username);

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

            // EF will not work with expressions of return type object.
            Expression<Func<T, string>> expr = (T t) => t.Username;
            dynamic dexpr = expr;
            return dexpr;
        }
    }
}