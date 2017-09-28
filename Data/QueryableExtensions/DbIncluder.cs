using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data
{
    public class DbIncluder : QueryableExtensions.IIncluder
    {
        public IQueryable<T> Include<T, TProperty>(IQueryable<T> source, Expression<Func<T, TProperty>> path)
            where T : class
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(source, path);
        }
    }
}
