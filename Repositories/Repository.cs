using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using LibraryManagementSystem.Repositories.IRepostories;
using LibraryManagementSystem.DataAccess;

namespace LibraryManagementSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public DbSet<T> dbSet { get; set; }

        private readonly ApplicationDbContext dbContext;
        public Repository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }

        public void Create(T entity) => dbSet.Add(entity);
        public void Create(List<T> entities) => dbSet.AddRange(entities);
        public void Edit(T entity) => dbSet.Update(entity);
        public void Delete(T entity) => dbSet.Remove(entity);
        public void Delete(List<T> entities) => dbSet.RemoveRange(entities);
        public void Commit() => dbContext.SaveChanges();

        public IQueryable<T> Get(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null,
            bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProps != null)
            {
                query = includeProps(query);
            }



            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public T? GetOne(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null,
             bool tracked = true)
        {
            return Get(filter, includeProps, tracked).FirstOrDefault();
        }
    }
}
