
namespace Cashicart.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        Task CommitAsync();
    }
}
