using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
    public class OrderItemRepository:Repository<OrderItem>,IOrderItemRepository
    {
        private readonly ApplicationDbContext dbContext;
        public OrderItemRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public void CreateRange(IEnumerable<OrderItem> shopItem)
        {
            dbContext.AddRange(shopItem);
        }
    }
}
