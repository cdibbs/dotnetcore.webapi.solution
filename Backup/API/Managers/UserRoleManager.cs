using API.Authorization;
using API.SpecificationProviders;
using API.Validators;
using AutoMapper;
using Data;
using Serilog;
using System;
using System.Linq.Expressions;

namespace API.Managers
{
    public class UserRoleManager : BaseManager<UserRole>
    {
        public UserRoleManager(IRepository repo, IMapper mapper, IValidator<UserRole> validator, ILogger logger, IAuthManager<UserRole> auth, IBaseSpecificationProvider<UserRole> specs)
            : base(repo, mapper, validator, logger, auth, specs)
        {
            FilterIncludes = new Expression<Func<UserRole, object>>[] {
                (UserRole ur) => ur.User,
                (UserRole ur) => ur.Role
            };
            GetIncludes = FilterIncludes;
            DeleteIncludes = FilterIncludes;
        }
    }
}