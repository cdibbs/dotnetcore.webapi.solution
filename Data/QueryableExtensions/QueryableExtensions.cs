using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.QueryableExtensions
{
    // Thanks to http://blogs.clariusconsulting.net/kzu/how-to-design-a-unit-testable-domain-model-with-entity-framework-code-first/
    public static class QueryableExtensions
    {
        public static IIncluder Includer = new NullIncluder();
        public static IFirstOrDefaulter FirstOrDefaulter = new DbFirstOrDefaulter();

        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> source, Expression<Func<T, TProperty>> path)
            where T : class
        {
            return Includer.Include(source, path);
        }

        public static T FirstOrDefault<T>(this IQueryable<T> source, Expression<Func<T, bool>> path)
            where T : class
        {
            return FirstOrDefaulter.FirstOrDefault(source, path);
        }
    }
}
