using Data;

namespace API.Models.ViewModels
{
    public class UserRoleViewModel : BaseViewModel, IViewModel<UserRole>
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }

        public UserViewModel User { get; set; }
        public RoleViewModel Role { get; set; }
    }
}