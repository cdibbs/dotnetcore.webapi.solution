using API.Models;
using API.Models.InputModels;
using API.Models.ViewModels;
using AutoMapper;
using Data;
using Data.Repositories.ReadOnly;

namespace API.Mapping
{
    public class APIMappingProfile : Profile {
        public APIMappingProfile()
        {
            //TODO: Create mappings
            var view = CreateMap<V_Population, PersonViewModel>();
            CreateMap<V_Population, IViewModel<V_Population, string>>()
                .As<PersonViewModel>();
            view.ForMember(d => d.Id, o => o.MapFrom(s => s.UserId));

            SetupDbModel<User, UserViewModel>();
            SetupDbModel<Role, RoleViewModel>();
            SetupDbModel<UserRole, UserRoleViewModel>();

            var ui = CreateMap<UserInputModel, User>();
            var uim = CreateMap<User, UserInputModel>();
            var uri = CreateMap<UserRoleInputModel, UserRole>();

            var authMap = CreateMap<User, AuthorizationDetails>();
            authMap.ForMember(dest => dest.Username, o => o.MapFrom(src => src.Username));
            authMap.ForMember(dest => dest.Groups, o => o.MapFrom(src => src.UserRoles));

            var userRoleMap = CreateMap<UserRole, string>();
            userRoleMap.ConvertUsing(r => r.Role.RoleName);

            var idw = CreateMap<V_Population, IViewModel<V_Population, string>>();
            idw.As<PersonViewModel>();
        }

        public IMappingExpression<T, TViewModel> SetupDbModel<T, TViewModel>()
            where T: IBaseEntity
            where TViewModel: IViewModel<T>
        {            
            // To do: as security evolves, we might need to switch to a service locator
            // within BaseManager and derivatives. It would choose an appropriate view model
            // based on the current user/security context, instead of using the Mapper, directly.
            var map = CreateMap<T, TViewModel>().MaxDepth(3);
            CreateMap<T, IViewModel<T>>().MaxDepth(3)
                .As<TViewModel>();
            CreateMap<T, IViewModel<T, long>>().MaxDepth(3)
                .As<TViewModel>();
            return map;
        }
    }
}