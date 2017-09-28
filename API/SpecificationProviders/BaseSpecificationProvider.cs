using Data;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace API.SpecificationProviders
{
    public class BaseSpecificationProvider<T, TKey> : IBaseSpecificationProvider<T>
        where TKey: IComparable
        where T: IEntity<TKey>
    {
        public ISpecification<T> All() => Specification<T>.All();

        public ISpecification<T> None() => Specification<T>.None();

        public ISpecification<T> ById<T>(long id) where T: IBaseEntity
            => Specification<T>.Start((T t) => t.Id == id, id);
    }
}
