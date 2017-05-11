using API.Models;
using Data;
using Specifications;

namespace API.SpecificationProviders
{
    public interface IRoleSpecificationProvider
    {
        ISpecification<T> RolesByFilter<T>(FilterModel filter) where T : Role;
    }
}