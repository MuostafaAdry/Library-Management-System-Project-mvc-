using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPublisherRepository _publisherRepository
           ;
        private readonly IAuthorRepository _authorRepository;
        public BookController(IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            IPublisherRepository publisherRepository,
            IAuthorRepository authorRepository)
        {
            this._bookRepository = bookRepository;
            this._categoryRepository = categoryRepository;
            this._publisherRepository = publisherRepository;
            this._authorRepository = authorRepository;
        }

        public IActionResult Index(string query, int page = 1)
        {
            var books = _bookRepository.Get(includeProps: e => e
            .Include(e => e.Category)
            .Include(e => e.Author)
            .Include(e => e.Publisher));
            //filter
            if (query != null)
            {
                books = books.Where(e => e.Title.Contains(query)
               || e.Author.FullName.Contains(query)
               || e.Publisher.Name.Contains(query)
               || e.Category.Name.Contains(query)
                );
            }
            var paginationPages = (int)Math.Ceiling((decimal)books.Count() / 7);
            if (page < 1) page = 1;
            if (page > paginationPages && paginationPages > 0) page = paginationPages;
            books = books.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;
            return View(books.ToList());
        }

        //Create new product
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _categoryRepository.Get();
            ViewBag.categories = categories.ToList();

            var authors = _authorRepository.Get();
            ViewBag.authors = authors.ToList();

            var publishers = _publisherRepository.Get();
            ViewBag.publishers = publishers.ToList();

            return View(new Book());
        }

        [HttpPost]
        public IActionResult Create(Book book, IFormFile cover, int CategoryId)

        {
            if (ModelState.IsValid)
            {
                if (cover != null && cover.Length > 0)
                {
                    //save img in wwwroot
                    //1- create img name with".png"
                    var imgName = Guid.NewGuid().ToString() + Path.GetExtension(cover.FileName);
                    //2- choose path where  img will be stored
                    var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Covers", imgName);

                    //copy img in the wwwroot
                    using (var stream = System.IO.File.Create(imgPath))
                    {
                        cover.CopyTo(stream);
                    }
                    //save img path in db
                    book.Cover = imgName;
                }


                _bookRepository.Create(book);
                _bookRepository.Commit();
                TempData["Success"] = "Product Created Successfully !";
                return RedirectToAction("Index");
            }

            var categories = _categoryRepository.Get();
            ViewBag.categories = categories.ToList();
            var authors = _authorRepository.Get();
            ViewBag.authors = authors.ToList();

            var publishers = _publisherRepository.Get();
            ViewBag.publishers = publishers.ToList();


            return View(book);
        }


    }
}
