using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        IQueryable<T> GetAll();

        Task Remove(T entity);

        // Recently added
        //Task<int> CommitAsync();
        Task<List<T>> GetAllAsync(); // New async method
    }
}