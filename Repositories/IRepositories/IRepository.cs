using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Repositories.IRepostories
{
    public interface IRepository<T> where T:class
    {
        public void Create(T entity);
        public void Create(List<T> entities);
        public void Edit(T entity);
        public void Delete(T entity);
        public void Delete(List<T> entities);
        public void Commit();

        public IQueryable<T> Get(
         Expression<Func<T, bool>>? filter = null,
          Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null,
        bool tracked = true);

        public T? GetOne(
            Expression<Func<T, bool>>? filter = null,
              Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null,
            bool tracked = true);
    }
}
