namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Cover { get; set; }
        public string Description { get; set; } 
        public bool IsBorrow { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public int PublisherId { get; set; }
        public DateTime PublishedDate { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        
        public double BuyPrice { get; set; }
        public double BorrowPrice { get; set; }
        public double? Offer { get; set; }
        public Author Author { get; set; }
        public Category Category { get; set; }
        public Publisher Publisher { get; set; }
        public ICollection<Borrowing> Borrowings { get; set; }
    }
}
