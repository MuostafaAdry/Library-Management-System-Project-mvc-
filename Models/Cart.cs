using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Models
{
    [PrimaryKey(nameof(BookId),nameof(ApplicationUserId))]
    public class Cart
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int CopyCount { get; set; }
    }
}
