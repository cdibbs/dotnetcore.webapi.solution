using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Models
{
    [Table("V_MyView")]
    public class V_MyView: IEntity<string>
    {
        [Column("ID_NUMBER")] // lol, no
        public string Id { get; set; }

        [Column("MemberType")]
        public string MemberType { get; set; }
    }
}
