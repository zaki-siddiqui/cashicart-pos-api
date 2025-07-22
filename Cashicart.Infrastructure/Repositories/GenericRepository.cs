using Cashicart.Common.Interfaces;
using Cashicart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Cashicart.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly CashicartDbContext _context;
        private readonly ILogger<GenericRepository<T>> _logger;

        public GenericRepository(CashicartDbContext context, ILogger<GenericRepository<T>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var include in includes)
                query = query.Include(include);

            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name == $"{typeof(T).Name}Id");
            if (keyProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an ID property.");

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, keyProperty.Name) == id);
        }

        public async Task<List<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public IQueryable<T> GetAll() => _context.Set<T>().AsQueryable();

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public async Task UpdateAsync(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _context.Set<T>().Update(entity);
            await Task.CompletedTask; // EF will save changes via UnitOfWork
        }

        public async Task Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
            await Task.CompletedTask; // EF will save changes via UnitOfWork
        }
    }
}



