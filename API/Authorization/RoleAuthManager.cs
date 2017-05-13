using System.Security.Principal;
using Data;
using Serilog;

namespace API.Authorization
{
    public class RoleAuthManager : BaseAuthManager<Role>
    {
        public RoleAuthManager(IPrincipal user, ILogger logger) : base(user, logger)
        {
        }
    }
}