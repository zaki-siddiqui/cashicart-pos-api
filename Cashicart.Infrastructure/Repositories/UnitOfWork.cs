//using Cashicart.Common.Interfaces;
//using Cashicart.Infrastructure.Data;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Cashicart.Infrastructure.Repositories
//{
//    public class UnitOfWork : IUnitOfWork
//    {
//        private readonly CashicartDbContext _context;
//        private readonly ILoggerFactory _loggerFactory;
//        private readonly Dictionary<Type, object> _repositories;

//        public UnitOfWork(CashicartDbContext context, ILoggerFactory loggerFactory)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
//            _repositories = new Dictionary<Type, object>();
//        }

//        public IRepository<T> GetRepository<T>() where T : class
//        {
//            if (_repositories.ContainsKey(typeof(T)))
//            {
//                return (IRepository<T>)_repositories[typeof(T)];
//            }

//            var logger = _loggerFactory.CreateLogger<GenericRepository<T>>();
//            var repository = new GenericRepository<T>(_context, logger);
//            _repositories[typeof(T)] = repository;
//            return repository;
//        }

//        public async Task CommitAsync()
//        {
//            await _context.SaveChangesAsync();
//        }

//        public void Dispose()
//        {
//            _context.Dispose();
//        }
//    }
//}

using Cashicart.Common.Interfaces;
using Cashicart.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Cashicart.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CashicartDbContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(CashicartDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _loggerFactory = loggerFactory;
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IRepository<T>)_repositories[typeof(T)];

            var logger = _loggerFactory.CreateLogger<GenericRepository<T>>();
            var repo = new GenericRepository<T>(_context, logger);
            _repositories[typeof(T)] = repo;
            return repo;
        }

        public async Task CommitAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}




