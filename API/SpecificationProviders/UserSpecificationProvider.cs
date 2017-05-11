using API.Models.FilterModels;
using Data;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SpecificationProviders
{
    public class UserSpecificationProvider : BaseSpecificationProvider<User>, IUserSpecificationProvider
    {
        public ISpecification<T> ByUsername<T>(string username) where T : User
            => Specification<T>.Start((T t) => t.Username == username, username);

        public ISpecification<T> UsersByFilter<T>(UserFilterModel filter) where T : User
        {
            var keywords = filter.Keywords;
            ISpecification<T> userSpec;
            if (keywords == null)
            {
                userSpec = Specification<T>.All();
            }
            else
            {
                userSpec = Specification<T>.Start(
                    t => keywords.All(
                             kw => kw == null || kw.Trim() == string.Empty
                                   || (t.Username != null && t.Username.Contains(kw))
                                   || (t.First != null && t.First.Contains(kw))
                                   || (t.Last != null && t.Last.Contains(kw))), keywords);
            }
            if (filter.Type == UserFilterModel.UserFilterType.Keywords)
            {
                return userSpec;
            }
            else if (filter.Type == UserFilterModel.UserFilterType.Username)
            {
                var username = filter.Username;
                return Specification<T>.Start((T t) => username == t.Username, username);
            }
            else if (filter.Type == UserFilterModel.UserFilterType.RoleAndKeywords)
            {
                var roleId = filter.RoleId;
                var role = Specification<T>.Start(t => roleId == 0 || t.UserRoles.Any(ur => ur.RoleId == roleId), roleId);
                return userSpec.And(role);
            }
            else if (filter.Type == UserFilterModel.UserFilterType.AnyRoleAndKeywords)
            {
                if (filter.RoleIds == null)
                    return userSpec;

                var role = Specification<T>.None();
                foreach (var roleId in filter.RoleIds)
                    role = role.Or(u => u.UserRoles.Any(ur => ur.RoleId == roleId));

                userSpec.Metadata = filter.RoleIds;
                return userSpec.And(role);
            }
            throw new NotImplementedException($"User filter type {filter.Type} has not been implemented.");
        }
    }
}
