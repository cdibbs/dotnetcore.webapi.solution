using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class UserRole : BaseEntity
    {
        public long UserId { get; set; }

        public long RoleId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}
