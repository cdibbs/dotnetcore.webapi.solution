using API.Models;
using Data;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SpecificationProviders
{
    public class RoleSpecificationProvider : BaseSpecificationProvider<Role, long>, IRoleSpecificationProvider
    {
        public ISpecification<T> RolesByFilter<T>(FilterModel filter) where T : Role
        {
            var words = filter.Keywords;
            if (words == null) return Specification<T>.All();
            return Specification<T>.Start((T t) => words.All(kw =>
                    (t.Description.Contains(kw))
                    || (t.RoleName.Contains(kw))), words);
        }
    }
}
