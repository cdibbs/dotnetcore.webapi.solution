using System.Security.Principal;
using API.Models;
using Data;
using Serilog;
using Specifications;

namespace API.Authorization
{
    public class UserRoleAuthManager : BaseAuthManager<UserRole, long>
    {
        public UserRoleAuthManager(IPrincipal user, ILogger logger) : base(user, logger)
        {
        }

        public override bool MayGet => IsInRole(RoleType.Admin);
        public override bool MayAdd => IsInRole(RoleType.Admin);
        public override bool MayUpdate => MayAdd;
        public override bool MayDelete => MayAdd;

        public override ISpecification<UserRole> GenerateFilterGet()
        {
            bool canGetAll = IsInRole(RoleType.Admin);
            var username = User?.Identity?.Name ?? null;
            return Specification<UserRole>.Start(u =>
                (u.User != null && username != null && u.User.Username == username)
                || canGetAll);
        }

        public override ISpecification<UserRole> GenerateFilterUpdate()
            => GenerateFilterGet();

        /// <summary>
        /// Can delete if admin or shepherd, and its not the person, themself.
        /// </summary>
        /// <returns>A delete filter specification</returns>
        public override ISpecification<UserRole> GenerateFilterDelete()
        {
            bool canGetAll = IsInRole(RoleType.Admin);
            string username = User.Identity.Name;
            return Specification<UserRole>.Start((UserRole u) => canGetAll && u.User != null && 
                !string.IsNullOrEmpty(username) && u.User.Username != username);
        }
    }
}