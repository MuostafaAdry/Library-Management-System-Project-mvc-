using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;

namespace LibraryManagementSystem.Repositories
{
    public class OrderRepository:Repository<Order>,IOrderRepository
    {
        private readonly ApplicationDbContext dbContext;
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
