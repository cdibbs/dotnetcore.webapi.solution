using API.Models.ViewModels;
using Data.Models;
using Data.Repositories.ReadOnly;
using Data.Utilities;
using Specifications;

namespace API.Managers
{
    public interface IPopulationManager : IBaseManager<V_MyView, string>
    {
        IViewModel<V_MyView, string>[] Filter(ISpecification<V_MyView> spec, int page, int pageSize,
            SortSpecification[] sortSpecifications);
        IViewModel<V_MyView, string> Get(string userId);
        IViewModel<V_MyView, string> Get(ISpecification<V_MyView> spec);
    }
}