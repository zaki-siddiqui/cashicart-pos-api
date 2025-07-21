////using Cashicart.Common.Exceptions;
////using Cashicart.Common.Interfaces;
////using Cashicart.Infrastructure.Data;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.Extensions.Logging;
////using System;
////using System.Linq;
////using System.Threading.Tasks;

////namespace Cashicart.Infrastructure.Repositories
////{
////    public class GenericRepository<T> : IRepository<T> where T : class
////    {
////        private readonly CashicartDbContext _context;
////        private readonly ILogger<GenericRepository<T>> _logger;

////        public GenericRepository(CashicartDbContext context, ILogger<GenericRepository<T>> logger)
////        {
////            _context = context ?? throw new ArgumentNullException(nameof(context));
////            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
////        }

////        public async Task<T> GetByIdAsync(Guid id)
////        {
////            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name == $"{typeof(T).Name}Id");
////            if (keyProperty == null)
////                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an ID property.");

////            var entity = await _context.Set<T>().FirstOrDefaultAsync(e =>
////                e != null && EF.Property<Guid>(e, keyProperty.Name) == id);
////            if (entity == null)
////                throw new NotFoundException($"{typeof(T).Name} not found.");
////            return entity;
////        }

////        public async Task AddAsync(T entity)
////        {
////            await _context.Set<T>().AddAsync(entity);
////            await _context.CommitAsync();
////            _logger.LogInformation("Added new {Type} entity", typeof(T).Name);
////        }

////        //public async Task UpdateAsync(T entity)
////        //{
////        //    _context.Set<T>().Update(entity);
////        //    await _context.CommitAsync();
////        //    _logger.LogInformation("Updated {Type} entity", typeof(T).Name);
////        //}

////        public async Task UpdateAsync(T entity, bool isTracked = false)
////        {
////            if (!isTracked)
////            {
////                // Only attach & mark modified if detached
////                _context.Set<T>().Attach(entity);
////                _context.Entry(entity).State = EntityState.Modified;
////            }
////            // No need to call Commit here; UnitOfWork will handle SaveChanges
////        }

////        public async Task Remove(T entity)
////        {
////            var isDeletedProp = typeof(T).GetProperty("IsDeleted");
////            if (isDeletedProp != null)
////            {
////                isDeletedProp.SetValue(entity, true);
////                await UpdateAsync(entity, true);
////            }
////            else
////            {
////                _context.Set<T>().Remove(entity);
////            }
////        }

////        //public async Task Remove(T entity)
////        //{
////        //    var prop = typeof(T).GetProperty("IsDeleted");
////        //    if (prop != null)
////        //    {
////        //        prop.SetValue(entity, true);
////        //        await UpdateAsync(entity);
////        //    }
////        //    else
////        //    {
////        //        _context.Set<T>().Remove(entity);
////        //        await Task.CompletedTask;
////        //    }
////        //}


////        public IQueryable<T> GetAll() => _context.Set<T>().AsQueryable();

////        public async Task<List<T>> GetAllAsync() => await _context.Set<T>().ToListAsync(); // New async method
////    }
////}

//using Cashicart.Common.Exceptions;
//using Cashicart.Common.Interfaces;
//using Cashicart.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Serilog.Core;

//namespace Cashicart.Infrastructure.Repositories
//{
//    public class GenericRepository<T> : IRepository<T> where T : class
//    {
//        private readonly CashicartDbContext _context;
//        private readonly ILogger<GenericRepository<T>> _logger;

//        public GenericRepository(CashicartDbContext context, ILogger<GenericRepository<T>> logger)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        /// <summary>
//        /// Get an entity by ID with tracking enabled.
//        /// </summary>
//        public async Task<T?> GetByIdAsync(Guid id)
//        {
//            var keyProperty = typeof(T).GetProperties()
//                .FirstOrDefault(p => p.Name == $"{typeof(T).Name}Id");

//            if (keyProperty == null)
//                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an ID property.");

//            return await _context.Set<T>()
//                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, keyProperty.Name) == id);
//        }

//        /// <summary>
//        /// Add entity to the context. SaveChanges is handled by UnitOfWork.
//        /// </summary>
//        public async Task AddAsync(T entity)
//        {
//            await _context.Set<T>().AddAsync(entity);
//        }

//        /// <summary>
//        /// Update entity. If tracked, do nothing extra; if detached, attach and mark as modified.
//        /// </summary>
//        public Task UpdateAsync(T entity)
//        {
//            _context.Set<T>().Update(entity);
//            return Task.CompletedTask;
//        }

//        ///// <summary>
//        ///// Soft delete if IsDeleted property exists, else hard delete.
//        ///// </summary>
//        //public async Task Remove(T entity)
//        //{
//        //    var prop = typeof(T).GetProperty("IsDeleted");
//        //    if (prop != null)
//        //    {
//        //        prop.SetValue(entity, true);
//        //        await UpdateAsync(entity, true);
//        //    }
//        //    else
//        //    {
//        //        _context.Set<T>().Remove(entity);
//        //    }

//        //}

//        public Task Remove(T entity)
//        {
//            _context.Set<T>().Remove(entity);
//            return Task.CompletedTask;
//        }

//        /// <summary>
//        /// Get all entities as IQueryable for further filtering.
//        /// </summary>
//        public IQueryable<T> GetAll() => _context.Set<T>().AsQueryable();

//        /// <summary>
//        /// Get all entities as list.
//        /// </summary>
//        public async Task<List<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

//    }
//}

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



