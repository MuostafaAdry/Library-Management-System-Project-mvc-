using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }
        public IActionResult Index(string query, int page = 1)
        {
            var categories = _categoryRepository.Get();
            //filter
            if (query != null)
            {
                categories = categories.Where(e => e.Name.Contains(query));
            }
            var paginationPages = (int)Math.Ceiling((decimal)categories.Count() / 7);
            if (page > paginationPages) page = paginationPages;
            categories = categories.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;
            return View(categories.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category != null)
                {
                    _categoryRepository.Create(category);
                    _categoryRepository.Commit();
                    TempData["Success"] = "Created Successfully";
                }

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int categoryId)
        {
            var category = _categoryRepository.GetOne(e => e.Id == categoryId);
            if (category != null)
            {
                return View(category);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category != null)
                {
                    _categoryRepository.Edit(category);
                    _categoryRepository.Commit();
                    TempData["Success"] = "Updated Successfully";
                }

            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int categoryId)
        {
            var category = _categoryRepository.GetOne(e => e.Id == categoryId);
            if (category != null)
            {
                _categoryRepository.Delete(category);
                _categoryRepository.Commit();
                TempData["Success"] = "Deleted Successfully";
            }
            return RedirectToAction("Index");
        }
    }
}
