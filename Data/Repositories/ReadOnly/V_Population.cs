using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Repositories.ReadOnly
{
    [Table("V_Population", Schema = "directory")]
    public class V_Population : ViewEntity
    {
        [Column("title")]
        public string Title { get; set; }

        [Column("mail")]
        public string Email { get; set; }

        [Column("givenName")]
        public string First { get; set; }

        [Column("initials")]
        public string Middle { get; set; }

        [Column("sn")]
        public string Last { get; set; }
    }
}
