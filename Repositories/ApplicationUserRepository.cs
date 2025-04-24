using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;

namespace LibraryManagementSystem.Repositories
{
    public class ApplicationUserRepository:Repository<ApplicationUser>,IApplicationUserRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ApplicationUserRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
