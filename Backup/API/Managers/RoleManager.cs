using API.Authorization;
using API.SpecificationProviders;
using API.Validators;
using AutoMapper;
using Data;
using Serilog;

namespace API.Managers
{
    public class RoleManager : BaseManager<Role>
    {
        public RoleManager(IRepository repo, IMapper mapper, IValidator<Role> validator, ILogger logger, IAuthManager<Role> auth, IBaseSpecificationProvider<Role> specs)
            : base(repo, mapper, validator, logger, auth, specs)
        {
        }
    }
}