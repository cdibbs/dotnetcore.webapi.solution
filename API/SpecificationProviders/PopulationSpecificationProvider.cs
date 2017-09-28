using API.Models.FilterModels;
using Data.Repositories.ReadOnly;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;

namespace API.SpecificationProviders
{
    public class PopulationSpecificationProvider : BaseSpecificationProvider<V_MyView, string>, IPopulationSpecificationProvider
    {
        public ISpecification<T> ByUserId<T>(string userId) where T : V_MyView
            => Specification<T>.Start((T t) => t.Id == userId, userId);

        public ISpecification<U> PopulationByMemberType<U>(string type) where U : V_MyView
            => Specification<U>.Start((U t) => t.MemberType == type, type);

    }
}
