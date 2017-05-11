using API.Models;
using API.SpecificationProviders;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]")]
    public class AuthorizationController : Controller
    {
        public string Key { get; set; }
        public ILogger Logger { get; set; }
        public IRepository Repo { get; set; }
        public IMapper Mapper { get; set; }
        public IUserSpecificationProvider Specs { get; set; }

        public AuthorizationController(
            string authKey,
            ILogger logger, IRepository repo, IMapper mapper, IUserSpecificationProvider specs)
        {
            this.Key = authKey;
            this.Logger = logger;
            this.Repo = repo;
            this.Mapper = mapper;
            this.Specs = specs;
        }

        /// <summary>
        /// Internal use. Enables the App Dev auth server to provide authorization info
        /// via the JSON Web Token (JWT) roles attribute.
        /// </summary>
        /// <param name="hawkId">The HawkId for whom to provide auth info.</param>
        /// <param name="key">API access key.</param>
        /// <returns>An object with a list of groups/roles.</returns>
        [SwaggerResponse(200, typeof(AuthorizationDetails), "An object describing the users' groups/roles.")]
        [HttpGet] public IActionResult Get(string username, string key) {
            if (string.CompareOrdinal(key, Key) != 0) {
                Logger.Error("Authorization detail called with invalid access token");
                return Unauthorized();
            }

            var user = Repo.FindOne(Specs.ByUsername<User>(username),
                track: false,
                includes: u => u.UserRoles.Select(ur => ur.Role));

            return Ok(
                Mapper.Map<AuthorizationDetails>(user) 
                // or, if not enrolled
                ?? new AuthorizationDetails() { Username = username, Groups = new string[0] });
        }
    }
}
