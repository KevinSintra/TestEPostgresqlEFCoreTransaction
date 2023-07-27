using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace TestEPostgresqlEFCoreTransaction
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> All();
        Task<bool> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<IList<T>> Get(Expression<Func<T, bool>> expression);
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected TestDBContext context;
        protected DbSet<T> dbSet;

        public GenericRepository(TestDBContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        async Task<IList<T>> IGenericRepository<T>.Get(Expression<Func<T, bool>> expression)
        {
            return await dbSet.Where(expression).AsNoTracking().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> All()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<bool> Add(T entity)
        {
            dbSet.Add(entity);
            await context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> Update(T entity)
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> Delete(T entity)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }
    }

    public interface IBulkTestRepository : IGenericRepository<pg_stat_statements_record>
    {
        Task<bool> Add(IList<pg_stat_statements_record> entitys);
    }

    public class BulkTestRepository : GenericRepository<pg_stat_statements_record>, IBulkTestRepository
    {
        public BulkTestRepository(TestDBContext context) : base(context) { }

        async Task<bool> IBulkTestRepository.Add(IList<pg_stat_statements_record> entitys)
        {
            await base.context.BulkInsertAsync(entitys);
            await base.context.BulkSaveChangesAsync(); // 在交易時會加速作業
            return true;
        }
    }

}

