using API.Models.FilterModels;
using Data.Repositories.ReadOnly;
using Specifications;

namespace API.SpecificationProviders
{
    public interface IPopulationSpecificationProvider
    {
        ISpecification<T> ByUserId<T>(string userId) where T : V_Population;
        ISpecification<T> PopulationByFilter<T>(PopulationFilterModel filter) where T : V_Population;
        ISpecification<U> PopulationByUsername<U>(string username) where U : V_Population;
        ISpecification<T> PopulationFilterByUsername<T>(PopulationFilterModel filter) where T : V_Population;
    }
}