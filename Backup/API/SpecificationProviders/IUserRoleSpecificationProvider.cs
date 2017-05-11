using API.Models;
using Data;
using Specifications;

namespace API.SpecificationProviders
{
    public interface IUserRoleSpecificationProvider : IBaseSpecificationProvider<UserRole>
    {
        ISpecification<T> UserRolesByFilter<T>(FilterModel filter) where T : UserRole;
    }
}