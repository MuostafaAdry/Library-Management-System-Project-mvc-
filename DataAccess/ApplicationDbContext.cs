using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models.ViewModel;

namespace LibraryManagementSystem.DataAccess
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContext ):base(dbContext)
        {
            
        }
        public DbSet<LibraryManagementSystem.Models.ViewModel.RegisterVM> RegisterVM { get; set; } = default!;
        //public DbSet<LibraryManagementSystem.Models.ViewModel.RegisterVM> RegisterVM { get; set; } = default!;
        //public DbSet<LibraryManagementSystem.Models.ViewModel.LoginVM> LoginVM { get; set; } = default!;
    }
}
