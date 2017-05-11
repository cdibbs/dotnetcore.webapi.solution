using API.Models.FilterModels;
using Data;
using Specifications;

namespace API.SpecificationProviders
{
    public interface IUserSpecificationProvider : IBaseSpecificationProvider<User>
    {
        ISpecification<T> ByUsername<T>(string username) where T : User;
        ISpecification<T> UsersByFilter<T>(UserFilterModel filter) where T : User;
    }
}