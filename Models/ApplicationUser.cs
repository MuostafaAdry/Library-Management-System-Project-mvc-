using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Address { get; set; }
        public ICollection<Borrowing> Borrowings { get; set; }
    }
}
