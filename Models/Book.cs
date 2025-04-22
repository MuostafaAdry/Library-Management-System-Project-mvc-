using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }
     
        public string? Cover { get; set; }
        [Required]
        public string Description { get; set; }
 
        public bool ?IsBorrow { get; set; }
        [Required]
        public string Title { get; set; }
        [ValidateNever]
        public int AuthorId { get; set; }
        [ValidateNever]
        public int CategoryId { get; set; }
        [ValidateNever]
        public int PublisherId { get; set; }
        [Required]
      
        public DateTime PublishedDate { get; set; }
        [Required]
        public int TotalCopies { get; set; }
        [Required]
        public int AvailableCopies { get; set; }
        [Required]

        public double BuyPrice { get; set; }
        [Required]
        public double BorrowPrice { get; set; }
        public double? Offer { get; set; }
        [ValidateNever]
        public Author Author { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public Publisher Publisher { get; set; }
        [ValidateNever]
        public ICollection<Borrowing> Borrowings { get; set; }
    }
}
