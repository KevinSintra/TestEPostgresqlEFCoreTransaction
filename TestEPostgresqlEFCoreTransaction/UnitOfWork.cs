using Microsoft.EntityFrameworkCore;
using System.Data;

namespace TestEPostgresqlEFCoreTransaction
{
    public interface IUnitOfWork : IDisposable
    {
        IBulkTestRepository AppResources { get; }

        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction GetRepoTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly TestDBContext _context;
        public IBulkTestRepository AppResources { get; private set; }

        public UnitOfWork(TestDBContext context)
        {
            _context = context;
            AppResources = new BulkTestRepository(context);
        }

        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction IUnitOfWork.GetRepoTransaction(IsolationLevel isolationLevel)
        {
            var result = _context.Database.BeginTransaction(isolationLevel);
            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
