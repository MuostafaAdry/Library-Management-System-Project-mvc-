﻿namespace LibraryManagementSystem.Models
{
    public class Borrowing
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
