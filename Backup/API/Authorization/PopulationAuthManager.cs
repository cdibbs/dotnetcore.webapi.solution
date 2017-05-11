using API.Models;
using Data.Repositories.ReadOnly;

namespace API.Authorization
{
    public class PopulationAuthManager : BaseAuthManager<V_Population>
    {
        public override bool MayGet => IsInRole(RoleType.Admin);
        public override bool MayAdd => false;
        public override bool MayUpdate => false;
        public override bool MayDelete => false;
    }
}