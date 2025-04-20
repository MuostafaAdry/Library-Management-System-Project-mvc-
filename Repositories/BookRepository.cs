using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;

namespace LibraryManagementSystem.Repositories
{
    public class BookRepository:Repository<Book>,IBookRepository
    {
        private readonly ApplicationDbContext dbContext;
        public BookRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
