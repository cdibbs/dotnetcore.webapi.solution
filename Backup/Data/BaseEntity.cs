using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class BaseEntity : IBaseEntity
    {
        [Column(Order=1), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public long LastUpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        [Timestamp] public byte[] Version { get; set; }
    }
}
