using API.Managers;
using API.Models;
using API.Models.FilterModels;
using API.Models.ViewModels;
using API.SpecificationProviders;
using Data.Repositories.ReadOnly;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [Authorize]
    public class PopulationController : BaseApiController<V_Population, string>
    {
        public IPopulationSpecificationProvider Specs { get; set; }
        public PopulationController() : base(null) { }
        public PopulationController(IBaseManager<V_Population, string> manager, IPopulationSpecificationProvider specs) : base(manager)
        {
            this.Specs = specs;
        }

        [HttpGet]
        [SwaggerResponse(200, typeof(PersonViewModel[]), "Array of ViewModels matching the filter.")]
        public IViewModel<V_Population, string>[] Get([FromQuery] PopulationFilterModel filter)
            => Manager.Filter(Specs.PopulationByFilter<V_Population>(filter), filter.Page ?? 0, filter.PageSize ?? 10, filter.SortSpecifications);
    }
}
