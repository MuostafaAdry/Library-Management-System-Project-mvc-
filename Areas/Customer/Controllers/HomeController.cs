using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IBookRepository _bookRepository;
       
        public HomeController(IBookRepository bookRepository )
        {
            this._bookRepository = bookRepository;
         }
        public IActionResult NotFound()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Index()
        {
            var books = _bookRepository.Get(includeProps:e=>e
            .Include(e=>e.Author).Include(e=>e.Category).Include(e=>e.Publisher));
            return View(books.ToList());
        }

        public IActionResult Details(int id)
        {
            var bookDetails = _bookRepository.GetOne(e => e.Id == id,includeProps:e=>e
            .Include(e=>e.Category)
            .Include(e=>e.Author)
            .Include(e=>e.Publisher)
            );
            if (bookDetails!=null)
            {
                return View(bookDetails);
            }
            return View(NotFound());
        }


        public IActionResult DetailsBorrow(int id)
        {
      
            var bookDetails = _bookRepository.GetOne(e => e.Id == id, includeProps: e => e
            .Include(e => e.Category)
            .Include(e => e.Author)
            .Include(e => e.Publisher));
            if (bookDetails != null&& bookDetails.AvailableCopies>0)
            {
                return View(bookDetails);
            }
            return View(NotFound());
        }


    }
}
