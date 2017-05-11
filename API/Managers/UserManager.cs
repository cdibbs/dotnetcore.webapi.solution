using System;
using System.Linq;
using System.Linq.Expressions;
using Data;
using API.Exceptions;
using API.Models.InputModels;
using API.Models.ViewModels;
using Specifications;
using API.Authorization;
using API.SpecificationProviders;
using API.Validators;
using AutoMapper;
using Serilog;

namespace API.Managers
{
    public class UserManager : BaseManager<User>, IUserManager
    {
        public UserManager(IRepository repo, IMapper mapper, IValidator<User> validator, ILogger logger, IAuthManager<User> auth, IUserSpecificationProvider specs)
            : base(repo, mapper, validator, logger, auth, specs)
        {
            FilterIncludes = new Expression<Func<User, object>>[]
            {
                u => u.UserRoles.Select(ur => ur.Role)
            };
            GetIncludes = this.FilterIncludes;
            UseHardDeletes = false;
        }

        // Add is overridden because we might need to set IsDeleted = true on an existing record
        // instead of actually inserting a new user.
        public override IViewModel<User, long> Add(BaseInputModel<User> input)
        {
            Auth.AuthorizeAdd();
            var uinput = input as UserInputModel;
            Logger.Information($"Adding user {uinput?.Username}.");
            var inactiveUser = Repo.FindOne<User>(Specification<User>.Start(u => u.Username == uinput.Username), incSoftDel: true, track: true);
            if (inactiveUser != null)
            {
                Logger.Information($"User was inactive, Id:{inactiveUser.Id}. Trying to reactivate.");
                if (!inactiveUser.IsDeleted)
                {
                    throw new InvalidInputException($"User {inactiveUser.Id} already exists.");
                }
                Logger.Information($"Reactivating {typeof(User).FullName}");
                Validator.Validate(input);
                inactiveUser.IsDeleted = false;
                //Repo.AddOrUpdate(inactiveUser);
                Repo.Save(true);
                Logger.Information($"Reactivated: {!inactiveUser.IsDeleted}.");
                return Mapper.Map<IViewModel<User>>(inactiveUser);
            }
            else
            {
                return base.Add(input);
            }
        }
    }
}