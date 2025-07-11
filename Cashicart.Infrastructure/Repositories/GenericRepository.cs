using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<T> GetByIdAsync(Guid id)
        {
            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name == $"{typeof(T).Name}Id");
            if (keyProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an ID property.");

            var entity = await _context.Set<T>().FirstOrDefaultAsync(e =>
                e != null && EF.Property<Guid>(e, keyProperty.Name) == id);
            if (entity == null)
                throw new NotFoundException($"{typeof(T).Name} not found.");
            return entity;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.CommitAsync();
            _logger.LogInformation("Added new {Type} entity", typeof(T).Name);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.CommitAsync();
            _logger.LogInformation("Updated {Type} entity", typeof(T).Name);
        }

        //public async Task Remove(T entity)
        //{
        //    _context.Set<T>().Remove(entity);
        //    await Task.CompletedTask;
        //}

        public async Task Remove(T entity)
        {
            var prop = typeof(T).GetProperty("IsDeleted");
            if (prop != null)
            {
                prop.SetValue(entity, true);
                await UpdateAsync(entity);
            }
            else
            {
                _context.Set<T>().Remove(entity);
                await Task.CompletedTask;
            }
        }


        public IQueryable<T> GetAll() => _context.Set<T>().AsQueryable();

        public async Task<List<T>> GetAllAsync() => await _context.Set<T>().ToListAsync(); // New async method
    }
}