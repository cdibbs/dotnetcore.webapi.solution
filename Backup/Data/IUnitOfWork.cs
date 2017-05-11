using System;

namespace Data
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
    }
}
