using API.Models;
using API.Models.ViewModels;
using Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using API.Managers;
using API.SpecificationProviders;

namespace API.Controllers
{
    [Route("[controller]")]
    public class RoleController : BaseApiController<Role, long>
    {
        public IRoleSpecificationProvider Specs { get; set; }
        public RoleController(IBaseManager<Role, long> manager, IRoleSpecificationProvider specs) : base(manager)
        {
            this.Specs = specs;
        }

        /// <summary>
        /// Pages through roles with the given filter.
        /// </summary>
        /// <param name="filter">A roles filter.</param>
        /// <returns>An ordered, paged list (size filter.PageSize) of role view models.</returns>
        [SwaggerResponse(200, typeof(RoleViewModel[]), "An array of Role view models.")]
        [HttpGet]
        public IViewModel<Role, long>[] Get([FromQuery] FilterModel filter)
            => Manager.Filter(Specs.RolesByFilter<Role>(filter), filter.Page ?? 0, filter.PageSize ?? 10, filter.SortSpecifications);
    }
}