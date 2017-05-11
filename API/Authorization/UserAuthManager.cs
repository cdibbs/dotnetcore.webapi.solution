using API.Models;
using Data;
using Specifications;

namespace API.Authorization
{
    public class UserAuthManager : BaseAuthManager<User>
    {
        public override bool MayGet => IsInRole(RoleType.Admin);
        public override bool MayAdd => IsInRole(RoleType.Admin);
        public override bool MayUpdate => MayAdd;
        public override bool MayDelete => MayAdd;

        public override ISpecification<User> GenerateFilterGet()
        {
            bool canGetAll = IsInRole(RoleType.Admin);
            string username = User?.Identity?.Name ?? null;
            return Specification<User>.Start((u) => 
                username != null && u.Username == username
                || canGetAll);
        }

        public override ISpecification<User> GenerateFilterUpdate()
            => GenerateFilterGet();

        /// <summary>
        /// Can delete if admin or shepherd, and its not the person, themself.
        /// </summary>
        /// <returns>A delete filter specification</returns>
        public override ISpecification<User> GenerateFilterDelete()
        {
            bool canGetAll = IsInRole(RoleType.Admin);
            string username = User?.Identity?.Name ?? null;
            return Specification<User>.Start((User u) => canGetAll && username != null && u.Username != username);
        }
    }
}