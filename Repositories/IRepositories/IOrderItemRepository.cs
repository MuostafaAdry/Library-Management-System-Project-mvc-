using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepostories;

namespace LibraryManagementSystem.Repositories.IRepositories
{
    public interface IOrderItemRepository:IRepository<OrderItem>
    {
        public void CreateRange(IEnumerable<OrderItem> shopItem);
    }
}
