using API.Models.FilterModels;
using Data.Models;
using Data.Repositories.ReadOnly;
using Specifications;

namespace API.SpecificationProviders
{
    public interface IPopulationSpecificationProvider : IBaseSpecificationProvider<V_MyView>
    {
        ISpecification<T> ByUserId<T>(string userId) where T : V_MyView;
        ISpecification<T> PopulationByMemberType<T>(string memberType) where T : V_MyView;

    }
}