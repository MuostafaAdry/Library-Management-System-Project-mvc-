using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PublisherController : Controller
    {
        private readonly IPublisherRepository _publisherRepository;
        public PublisherController(IPublisherRepository publisherRepository)
        {
            this._publisherRepository = publisherRepository;
        }
        public IActionResult Index(string query, int page = 1)
        {
            var categories = _publisherRepository.Get();
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
        public IActionResult Create(Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                if (publisher != null)
                {
                    _publisherRepository.Create(publisher);
                    _publisherRepository.Commit();
                    TempData["Success"] = "Created Successfully";
                }

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int publisherId)
        {
            var publisher = _publisherRepository.GetOne(e => e.Id == publisherId);
            if (publisher != null)
            {
                return View(publisher);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                if (publisher != null)
                {
                    _publisherRepository.Edit(publisher);
                    _publisherRepository.Commit();
                    TempData["Success"] = "Updated Successfully";
                }

            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int publisherId)
        {
            var publisher = _publisherRepository.GetOne(e => e.Id == publisherId);
            if (publisher != null)
            {
                _publisherRepository.Delete(publisher);
                _publisherRepository.Commit();
                TempData["Success"] = "Deleted Successfully";
            }
            return RedirectToAction("Index");
        }
    }
}

