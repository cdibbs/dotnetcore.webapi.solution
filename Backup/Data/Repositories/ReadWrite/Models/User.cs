using System.Collections.Generic;

namespace Data
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
        public string Email { get; set; }
        public bool EmailNotifications { get; set; }
    
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
