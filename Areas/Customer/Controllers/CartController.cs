using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var books = _cartRepository.Get(e => e.ApplicationUserId == userId, includeProps: e => e.
            Include(e => e.Book));

            var total = books.Sum(e => e.Book.BuyPrice * e.CopyCount);
            ViewBag.total = total;
            return View(books.ToList());
        }


        //public IActionResult AddToCart(int id, int count)
        //{
        //    var userId = _userManager.GetUserId(User);

        //    var book = _bookRepository.GetOne(e => e.Id == id);
        //    if (book != null)
        //    {
        //        if (book.AvailableCopies < count)
        //        {
        //            var cart = new Cart
        //            {
        //                ApplicationUserId = userId,
        //                CopyCount = count,
        //                BookId = id
        //            };

        //            var bookInDb = _cartRepository.GetOne(e => e.BookId == id && e.ApplicationUserId == userId);
        //            if (bookInDb != null)
        //                bookInDb.CopyCount = count;
        //            else
        //                _cartRepository.Create(cart);

        //            _cartRepository.Commit();
        //        }
        //        else
        //        {
        //            TempData["Success"] = "Sorry you can not buy this amount We Have only @{bookInDb.Book.AvailableCopies}!";
        //        }
        //    }
        //    else
        //    {
        //        TempData["Success"] = "not found";
        //    }



        //    return RedirectToAction("Index", "Home", new { area = "Customer" });

        //}
        public IActionResult AddToCart(int id, int count)
        {
            var userId = _userManager.GetUserId(User);
            var bookInDb = _cartRepository.GetOne(e => e.BookId == id && e.ApplicationUserId == userId);

            // لو الكتاب مش موجود في الكارت
            if (bookInDb == null)
            {
                var book = _bookRepository.GetOne(b => b.Id == id); // هات الكتاب
                if (book == null || book.AvailableCopies < count)
                {
                    TempData["Success"] = $"Sorry, you can’t buy this amount. We have only {book?.AvailableCopies ?? 0}!";
                    return RedirectToAction("Details", "Home", new { area = "Customer", id = id });
                }

                var cart = new Cart
                {
                    ApplicationUserId = userId,
                    CopyCount = count,
                    BookId = id
                };
                _cartRepository.Create(cart);
            }
            else
            {
                if (bookInDb.Book.AvailableCopies < count)
                {
                    TempData["Success"] = $"Sorry, you can’t buy this amount. We have only {bookInDb.Book.AvailableCopies}!";
                    return RedirectToAction("Details", "Home", new { area = "Customer",id=id });
                }

                bookInDb.CopyCount = count;
            }

            _cartRepository.Commit();
            TempData["Success"] = "Added Successfully";


            return RedirectToAction("index", "Home", new { area = "Customer" });
        }


        public IActionResult Increment(int id, int page)
        {
            var userId = _userManager.GetUserId(User);
            var book = _cartRepository.GetOne(e => e.ApplicationUserId == userId && e.BookId == id);
            if (book != null)
            {
                book.CopyCount++;
                _cartRepository.Commit();
            }
            return RedirectToAction("Index", new { page = page });
        }

        public IActionResult Decrement(int id, int page)
        {
            var userId = _userManager.GetUserId(User);
            var book = _cartRepository.GetOne(e => e.ApplicationUserId == userId && e.BookId == id);
            if (book != null && book.CopyCount > 1)
            {
                book.CopyCount--;
                _cartRepository.Commit();
            }
            return RedirectToAction("Index", new { page = page });
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
    }
}
