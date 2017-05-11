using API.Models;
using API.Models.FilterModels;
using API.Models.InputModels;
using API.Models.ViewModels;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using API.Managers;
using API.SpecificationProviders;

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UserController : BaseApiController<User, long>
    {
        public IUserSpecificationProvider Specs { get; set; }
        public UserController(IBaseManager<User, long> manager, IUserSpecificationProvider specs) : base(manager)
        {
            this.Specs = specs;
        }

        /// <summary>
        /// Pages through users with the given filter. Shows only those the current
        /// user is authorized to view.
        /// </summary>
        /// <param name="filter">A user filter.</param>
        /// <returns>An ordered, paged list (size filter.PageSize) of user view models.</returns>
        [SwaggerResponse(200, typeof(UserViewModel[]), "An array of User view models.")]
        [AcceptVerbs("SEARCH")]
        public IViewModel<User, long>[] Get([FromQuery] UserFilterModel filter)
            => Manager.Filter(Specs.UsersByFilter<User>(filter), filter.Page ?? 0, filter.PageSize ?? 10, filter.SortSpecifications);

        /*[HttpGet]
        public UserViewModel Get(string hawkId) 
            => Manager.Get(Specs.ByHawkId<User>(hawkId ?? User.Identity.GetUserId()));*/
        /// <summary>
        /// Gets an individual user by database id, if authorized to view them.
        /// </summary>
        /// <param name="id">The integer id of the user.</param>
        /// <returns>The requested view model of the user.</returns>
        [SwaggerResponse(200, typeof(UserViewModel), "A User view model.")]
        [HttpGet] public IViewModel<User, long> Get(long id) => Manager.Get(id);

        /// <summary>
        /// Creates a new user from input, if authorized. New user must exist in IDW users list.
        /// </summary>
        /// <param name="input">The user input object.</param>
        /// <returns>A view model of the created user object.</returns>
        [SwaggerResponse(200, typeof(UserViewModel), "A User view model representing the created object.")]
        [HttpPost] public IActionResult Post(UserInputModel input) => Ok(Manager.Add(input));

        /// <summary>
        /// Update an existing user from input, if authorized.
        /// </summary>
        /// <param name="input">The user input object.</param>
        /// <returns>A view model of the saved changes to the user.</returns>
        [SwaggerResponse(200, typeof(UserViewModel), "A User view model representing the edited object.")]
        [HttpPut] public IActionResult Put(UserInputModel input) => Ok(Manager.Update(input));

        /// <summary>
        /// Delete a user object (soft or otherwise), if permitted. May not delete self.
        /// May not delete if not in admin role.
        /// </summary>
        /// <param name="id">The id of the user to delete.</param>
        [SwaggerResponse(200, typeof(UserViewModel), "A User view model representing the deleted object.")]
        [HttpDelete] public IActionResult Delete(long id) => Ok(Manager.Delete(id));
    }
}