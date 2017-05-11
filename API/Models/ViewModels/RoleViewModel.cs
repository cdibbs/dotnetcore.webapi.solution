using Data;

namespace API.Models.ViewModels
{
    public class RoleViewModel : BaseViewModel, IViewModel<Role>
    {
        public string RoleName { get; set; }
        public string Description { get; set; }

        public UserRoleViewModel[] UserRoles { get; set; }
    }
}