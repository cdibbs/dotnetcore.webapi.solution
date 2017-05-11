using Data;

namespace API.Models.InputModels
{
    public class UserInputModel : BaseInputModel<User>
    {
        public string Username { get; set; }
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
        public string Email { get; set; }
        public bool EmailNotifications { get; set; }
    }
}