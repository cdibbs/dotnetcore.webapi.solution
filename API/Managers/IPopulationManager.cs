using API.Models.ViewModels;
using Data.Repositories.ReadOnly;
using Data.Utilities;
using Specifications;

namespace API.Managers
{
    public interface IPopulationManager : IBaseManager<V_Population, string>
    {
        IViewModel<V_Population, string>[] Filter(ISpecification<V_Population> spec, int page, int pageSize,
            SortSpecification[] sortSpecifications);
        IViewModel<V_Population, string> Get(string userId);
        IViewModel<V_Population, string> Get(ISpecification<V_Population> spec);
    }
}