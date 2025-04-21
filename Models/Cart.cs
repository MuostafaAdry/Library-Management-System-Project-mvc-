using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Models
{
    [PrimaryKey(nameof(BookId),nameof(ApplicationUserId),nameof(Type))]
    public class Cart
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public TypeStatues Type { get; set; }
        public int CopyCount { get; set; }

        public DateOnly? ReturnDate { get; set; }
    }


    public enum TypeStatues
    {
        Buying = 0,
        Borrowing = 1,

    }
}
