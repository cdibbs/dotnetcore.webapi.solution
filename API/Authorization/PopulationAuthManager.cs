using System.Security.Principal;
using API.Models;
using Data.Models;
using Data.Repositories.ReadOnly;
using Serilog;

namespace API.Authorization
{
    public class PopulationAuthManager : BaseAuthManager<V_MyView, string>
    {
        public PopulationAuthManager(IPrincipal user, ILogger logger) : base(user, logger)
        {
        }

        public override bool MayGet => IsInRole(RoleType.Admin);
        public override bool MayAdd => false;
        public override bool MayUpdate => false;
        public override bool MayDelete => false;
    }
}