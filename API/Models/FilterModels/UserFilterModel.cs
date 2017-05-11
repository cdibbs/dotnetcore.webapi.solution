namespace API.Models.FilterModels
{
    public class UserFilterModel: FilterModel
    {
        public UserFilterType Type { get; set; } = UserFilterType.Keywords;
        public string Username { get; set; }
        public long RoleId { get; set; } 
        public long[] RoleIds { get; set; }

        public enum UserFilterType
        {
            Keywords = 0x1,
            Username = 0x2,
            RoleAndKeywords = 0x3,
            AnyRoleAndKeywords = 0x4
        }
    }
}