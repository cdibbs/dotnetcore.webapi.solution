using Data.Utilities;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data
{
    public interface IRepository
    {
        void Save(bool allDataContext = false);
        void AddEntity<T>(T o) where T : BaseEntity;
        T Delete<T>(ISpecification<T> spec, bool hardDelete = false, params Expression<Func<T, object>>[] includes) where T : BaseEntity;

        IEnumerable<T> Page<T>(
            ISpecification<T> spec,
            int offsetPage = 0, int pageSize = 10,
            ISortFactory<T, long> sortFactory = null,
            bool track = false, bool incSoftDel = false,
            params Expression<Func<T, object>>[] includes)
            where T : BaseEntity;

        IQueryable<T> FindAll<T>(
            ISpecification<T> spec,
            bool track = false, bool incSoftDel = false, 
            params Expression<Func<T, object>>[] includes)
            where T : BaseEntity;

        T FindOne<T>(
            ISpecification<T> spec,
            bool track = false, bool incSoftDel = false,
            params Expression<Func<T, object>>[] includes)
            where T : BaseEntity;
    }
}