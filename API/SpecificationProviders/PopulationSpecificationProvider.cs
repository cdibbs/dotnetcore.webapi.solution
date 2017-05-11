using API.Models.FilterModels;
using Data.Repositories.ReadOnly;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SpecificationProviders
{
    public class PopulationSpecificationProvider : BaseSpecificationProvider<V_Population>, IPopulationSpecificationProvider
    {
        public ISpecification<T> ByUserId<T>(string userId) where T : V_Population
            => Specification<T>.Start((T t) => t.UserId == userId, userId);

        public ISpecification<U> PopulationByUsername<U>(string username) where U : V_Population
            => Specification<U>.Start((U t) => t.Username == username, username);

        public ISpecification<T> PopulationFilterByUsername<T>(PopulationFilterModel filter) where T : V_Population
        {
            var words = filter.Keywords;
            if (words == null) return Specification<T>.All();
            return Specification<T>.Start((T t) => words.All(kw => t.Username.StartsWith(kw)), words);
        }

        public ISpecification<T> PopulationByFilter<T>(PopulationFilterModel filter) where T : V_Population
        {
            if (filter.Type == PopulationFilterModel.PopulationFilterType.Keywords)
            {
                var words = filter.Keywords;
                if (words == null) return Specification<T>.All();
                return Specification<T>.Start((T t) =>
                    words.All(kw =>
                        kw == null || kw.Trim() == ""
                        || (t.Last.Contains(kw))
                        || (t.First.Contains(kw))
                        || (t.Email.Contains(kw))
                        || (t.Username.Contains(kw))), words) /* empty filter is 'all' */;
            }

            var username = filter.Username;
            return Specification<T>.Start((T t) => username == t.Username, username);
        }
    }
}
