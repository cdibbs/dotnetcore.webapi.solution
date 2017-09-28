using API.Models;
using Data;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SpecificationProviders
{
    public class UserRoleSpecificationProvider : BaseSpecificationProvider<UserRole, long>, IUserRoleSpecificationProvider
    {
        public ISpecification<T> UserRolesByFilter<T>(FilterModel filter) where T : UserRole
        {
            var words = filter.Keywords;
            if (words == null) return Specification<T>.All();
            return Specification<T>.Start((T t) => words.All(kw =>
                (t.Role.Description.Contains(kw))
                || (t.Role.RoleName.Contains(kw))), words);
        }
    }
}
