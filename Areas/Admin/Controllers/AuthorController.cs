using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorController(IAuthorRepository authorRepository)
        {
            this._authorRepository = authorRepository;
        }
        public IActionResult Index(string query, int page=1)
        {
            var authors = _authorRepository.Get();
            //filter
            if (query!=null)
            {
                authors = authors.Where(e => e.FullName.Contains(query));
            }
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)authors.Count() / 7);
            if (page > paginationPages) page = paginationPages;
            authors = authors.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;

            return View(authors.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _authorRepository.Create(author);
                _authorRepository.Commit();
                 TempData["Success"] = "Created Successfully";
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Edit(int authorId)
        {
            var author = _authorRepository.GetOne(e => e.Id == authorId);
            if (author!=null)
            {
                return View(author);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Author author)
        {
            if (ModelState.IsValid)
            {
                _authorRepository.Edit(author);
                _authorRepository.Commit();
                TempData["Success"] = "Updated Successfully";
            }
            return RedirectToAction("Index");

        }
        public IActionResult Delete(int authorId)
        {
            var author = _authorRepository.GetOne(e => e.Id == authorId);
            if (author!=null)
            {
                _authorRepository.Delete(author);
                _authorRepository.Commit();
                TempData["Success"] = "Deleted Successfully";
            }
            return RedirectToAction("Index");
        }

    }
}
