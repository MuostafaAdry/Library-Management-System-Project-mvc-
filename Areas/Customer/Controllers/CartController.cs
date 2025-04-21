using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Type = LibraryManagementSystem.Models.Type;

namespace LibraryManagementSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookRepository _bookRepository;
        public CartController(ICartRepository cartRepository,
            UserManager<ApplicationUser> userManager,
            IBookRepository bookRepository
            )
        {
            this._cartRepository = cartRepository;
            this._userManager = userManager;
            this._bookRepository = bookRepository;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var books = _cartRepository.Get(e => e.ApplicationUserId == userId,
                includeProps: e => e.Include(e => e.Book)).ToList();

            //var total = 0.0 ;
            //foreach (var item in books)
            //{
            //    if (item.Type==TypeStatues.Buying)
            //    {
            //        total = books.Sum(e => e.Book.BuyPrice * (1 - (e.Book.Offer ?? 0) / 100) * e.CopyCount);
            //    }
            //    else
            //    {
            //        total = books.Sum(e => e.Book.BorrowPrice  * e.CopyCount);
            //    }
            //}
            double total = books
                   .Where(e => e.Type == TypeStatues.Buying)
                   .Sum(e => e.Book.BuyPrice * (1 - (e.Book.Offer ?? 0) / 100) * e.CopyCount)
                   +
                   books
                   .Where(e => e.Type == TypeStatues.Borrowing)
                   .Sum(e => e.Book.BorrowPrice * e.CopyCount);

            ViewBag.total = total.ToString("F2");
            return View(books);
        }


        public IActionResult BuyingAddToCart(int id, int count)
        {
            var userId = _userManager.GetUserId(User);

            var bookInDb = _cartRepository.GetOne(e => e.BookId == id &&
                                                e.ApplicationUserId == userId &&
                                                e.Type == TypeStatues.Buying);

            var book = _bookRepository.GetOne(e => e.Id == id); // تأكد إنك عندك الريبو ده

            if (book == null)
            {
                TempData["Error"] = "Book not found";
                return RedirectToAction("Details", "Home", new { area = "Customer", id = id });
            }

            // التحقق من الكمية المطلوبة مقارنة بعدد النسخ المتاحة
            if (book.AvailableCopies < count)
            {
                TempData["Error"] = "We do not have enough books";
                return RedirectToAction("Details", "Home", new { area = "Customer", id = id });
            }

            if (bookInDb != null)
            {
                bookInDb.CopyCount += count;
                _cartRepository.Edit(bookInDb);
            }
            else
            {
                var cart = new Cart
                {
                    ApplicationUserId = userId,
                    CopyCount = count,
                    BookId = id,
                    Type = TypeStatues.Buying
                };
                _cartRepository.Create(cart);
            }

            _cartRepository.Commit();
            TempData["Success"] = "Added Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult BorrowingAddToCart(int id, int copyNum, DateOnly returnDate)
        {
            var userId = _userManager.GetUserId(User);

            var bookInDb = _cartRepository.GetOne(e => e.BookId == id &&
                                                e.ApplicationUserId == userId &&
                                                e.Type == TypeStatues.Borrowing);

            var book = _bookRepository.GetOne(e => e.Id == id);

            if (book == null)
            {
                TempData["Error"] = "Book not found";
                return RedirectToAction("Details", "Home", new { area = "Customer", id = id });

            }

            // التحقق من الكمية المطلوبة مقارنة بعدد النسخ المتاحة
            if (book.AvailableCopies < copyNum)
            {
                TempData["Error"] = "We do not have enough books";
                return RedirectToAction("Details", "Home", new { area = "Customer", id = id });

            }
            if (bookInDb != null)
            {
                bookInDb.CopyCount += copyNum;
                bookInDb.ReturnDate = returnDate;
                _cartRepository.Edit(bookInDb);
            }
            else
            {
                var cart = new Cart
                {
                    ApplicationUserId = userId,
                    CopyCount = copyNum,
                    BookId = id,
                    Type = TypeStatues.Borrowing,
                    ReturnDate = returnDate
                };
                _cartRepository.Create(cart);
            }



            _cartRepository.Commit();
            TempData["Success"] = "Borrowed Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }


        public IActionResult Increment(int id, TypeStatues type, int page)
        {
            var userId = _userManager.GetUserId(User);
            var book = _cartRepository.GetOne(e => e.ApplicationUserId == userId && e.BookId == id
            && e.Type == type);
            if (book != null)
            {
                book.CopyCount++;
                _cartRepository.Commit();
            }
            return RedirectToAction("Index", new { page = page });
        }

        public IActionResult Decrement(int id, TypeStatues type, int page)
        {
            var userId = _userManager.GetUserId(User);
            var book = _cartRepository.GetOne(e => e.ApplicationUserId == userId && e.BookId == id &&
               e.Type == type);
            if (book != null && book.CopyCount > 1)
            {
                book.CopyCount--;
                _cartRepository.Commit();
            }
            return RedirectToAction("Index", new { page = page });
        }

 
        public IActionResult UpdateDate(int id, TypeStatues type,DateOnly ReturnDate )
        {
            var userId = _userManager.GetUserId(User);
            var book = _cartRepository.GetOne(e => e.ApplicationUserId == userId && e.BookId == id
            && e.Type == type, tracked: true);
            if (book != null&&book.Type==type)
            {
                book.ReturnDate = ReturnDate;
                _cartRepository.Edit(book);
                _cartRepository.Commit();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var book = _cartRepository.GetOne(e => e.ApplicationUserId == userId && e.BookId == id);
            if (book != null)
            {
                _cartRepository.Delete(book);
                _cartRepository.Commit();
                TempData["Success"] = "Deleted Successfully";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Pay()
        {
            return View();
        }
    }
}
