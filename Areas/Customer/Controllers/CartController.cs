using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace LibraryManagementSystem.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookRepository _bookRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        public CartController(ICartRepository cartRepository,
            UserManager<ApplicationUser> userManager,
            IBookRepository bookRepository,
            IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository
            )
        {
            this._cartRepository = cartRepository;
            this._userManager = userManager;
            this._bookRepository = bookRepository;
            this._orderItemRepository = orderItemRepository;
            this._orderRepository = orderRepository;
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
                return RedirectToAction("DetailsBorrow", "Home", new { area = "Customer", id = id });

            }

            // التحقق من الكمية المطلوبة مقارنة بعدد النسخ المتاحة
            if (book.AvailableCopies < copyNum)
            {
                TempData["Error"] = "We do not have enough books";
                return RedirectToAction("DetailsBorrow", "Home", new { area = "Customer", id = id });

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


        public IActionResult UpdateDate(int id, TypeStatues type, DateOnly ReturnDate)
        {
            var userId = _userManager.GetUserId(User);
            var book = _cartRepository.GetOne(e => e.ApplicationUserId == userId && e.BookId == id
            && e.Type == type, tracked: true);
            if (book != null && book.Type == type)
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
            var cart = _cartRepository.Get(e => e.ApplicationUserId == _userManager.GetUserId(User), includeProps: e => e.Include(e => e.ApplicationUser)
            .Include(e => e.Book));
            var books = _bookRepository.Get().ToList();
            var order = new Order();
            var userId = _userManager.GetUserId(User);
            order.ApplicationUserId = userId;
            order.OrderDate = DateTime.Now;
            order.Status = OrderStatus.Pending;
            order.OrderTotal = cart.Sum(e => (e.Type == TypeStatues.Buying ? e.Book.BuyPrice : e.Book.BorrowPrice) * e.CopyCount);



            _orderRepository.Create(order);
            _orderRepository.Commit();



            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel",
            };

            //var Checktotal = cart.Sum(e => (e.Type == TypeStatues.Buying ? e.Book.BuyPrice : e.Book.BorrowPrice) * e.CopyCount);
            //if (Checktotal >= 30)
            //{
            //    TempData["Error"] = "Sorry, you can not pay if total less than 30 egp ,at lest 2 books";
            //    return RedirectToAction("Index");
            //}
            foreach (var item in cart)
            {


                var price = (long)((item.Type == TypeStatues.Buying
                      ? (item.Book.BuyPrice - (item.Book.Offer.HasValue ?
                      (item.Book.BuyPrice * item.Book.Offer.Value / 100) : 0))
                       : item.Book.BorrowPrice));
                options.LineItems.Add(

                                  new SessionLineItemOptions
                                  {
                                      PriceData = new SessionLineItemPriceDataOptions
                                      {
                                          Currency = "egp",
                                          ProductData = new SessionLineItemPriceDataProductDataOptions
                                          {
                                              Name = item.Book.Title,
                                              Description = item.Book.Description,
                                              Images = item.Book.Cover != null ? new List<string> { item.Book.Cover } : new List<string>(),
                                          },

                                          UnitAmount = price * 100

                                      },
                                      Quantity = item.CopyCount,
                                  }
                                  );


            }
            _bookRepository.Commit();

            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            _orderRepository.Commit();

            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (var item in cart)
            {
                var orderItem = new OrderItem()
                {
                    OrderId = order.Id,
                    BookId = item.Book.Id,
                    Count = item.CopyCount,
                };

                if (item.Type == TypeStatues.Buying)
                {
                    orderItem.BuyPrice = item.Book.BuyPrice;
                }
                else
                {
                    orderItem.BorrowPrice = item.Book.BorrowPrice;
                }

                if (order.SessionId != null)
                {
                    var book = books.FirstOrDefault(e => e.Id == item.BookId);

                    if (book != null)
                    {
                        book.AvailableCopies -= item.CopyCount;
                        book.TotalCopies -= item.CopyCount;
                    }

                }

                orderItems.Add(orderItem);
            }
            _bookRepository.Commit();

            _orderItemRepository.CreateRange(orderItems);
            _orderItemRepository.Commit();
            return Redirect(session.Url);
        }

    }
}
