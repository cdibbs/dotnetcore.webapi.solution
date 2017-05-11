using System;

namespace Data
{
    public interface IBaseEntity : IEntity
    {
        long Id { get; set; }
        DateTime Created { get; set; }
        DateTime LastUpdated { get; set; }
        long LastUpdatedBy { get; set; }
        bool IsDeleted { get; set; }
        byte[] Version { get; set; }
    }
}