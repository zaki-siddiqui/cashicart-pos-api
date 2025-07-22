using System.Linq.Expressions;

namespace Cashicart.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllAsync();
        IQueryable<T> GetAll();

        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task Remove(T entity);
    }
}
