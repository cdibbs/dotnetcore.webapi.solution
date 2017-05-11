using Data;

namespace API.Models.InputModels
{
    public class UserRoleInputModel : BaseInputModel<UserRole>
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }

    }
}