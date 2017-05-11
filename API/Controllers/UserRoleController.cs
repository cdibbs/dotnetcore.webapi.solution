using API.Managers;
using API.Models;
using API.Models.InputModels;
using API.Models.ViewModels;
using API.SpecificationProviders;
using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]")]
    public class UserRoleController : BaseApiController<UserRole, long>
    {
        public IUserRoleSpecificationProvider Specs { get; set; }
        public UserRoleController(IBaseManager<UserRole, long> manager, IUserRoleSpecificationProvider specs) : base(manager)
        {
            this.Specs = specs;
        }

        /// <summary>
        /// Pages through user roles with the given filter.
        /// </summary>
        /// <param name="filter">A user roles filter.</param>
        /// <returns>An ordered, paged list (size filter.PageSize) of user role view models.</returns>
        [SwaggerResponse(200, typeof(UserRoleViewModel[]), "An array of UserRole view models.")]
        [HttpGet] public IViewModel<UserRole, long>[] Get([FromQuery] FilterModel filter)
            => Manager.Filter(Specs.UserRolesByFilter<UserRole>(filter), filter.Page ?? 0, filter.PageSize ?? 10, filter.SortSpecifications);

        //get userrole by userid
        //[SwaggerResponse(HttpStatusCode.OK, "An array of UserRole for one user view models.",
        //    typeof(UserRoleViewModel[]))]
        //[HttpGet]
        //public IViewModel<UserRole, long>[] Get(long id) => Manager.Get(id);


        /// <summary>
        /// Adds a Role to a User by adding a UserRole join.
        /// </summary>
        /// <param name="input">An input object representing the join.</param>
        /// <returns>A view model representing the new UserRole.</returns>
        [SwaggerResponse(200, typeof(UserRoleViewModel), "A UserRole view model representing the created object.")]
        [HttpPost]
        public IActionResult Post(UserRoleInputModel input) => Ok(Manager.Add(input));

        /// <summary>
        /// Deletes a Role from a User by removing that UserRole join.
        /// </summary>
        /// <param name="id">The database id of the UserRole join.</param>
        /// <returns>A view model representing the deleted UserRole.</returns>
        [SwaggerResponse(200, typeof(UserRoleViewModel), "A UserRole view model representing the deleted object.")]
        [HttpDelete]
        public IActionResult Delete(long id) => Ok(Manager.Delete(id));
    }
}