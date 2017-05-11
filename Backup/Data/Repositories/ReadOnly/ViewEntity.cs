using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Repositories.ReadOnly
{
    public class ViewEntity : IEntity
    {
        [Key, Column("userId")]
        public string UserId { get; set; }

        [Column("Username")]
        public string Username { get; set; }
    }
}
