using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.ViewModel;
using LibraryManagementSystem.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IApplicationUserRepository userRepository,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager
            )
        {
            this._userRepository = userRepository;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        public IActionResult Index(string query, int page = 1)
        {
            var users = _userRepository.Get();
            //filter
            if (query != null)
            {
                users = users.Where(e => e.UserName.Contains(query)
                || e.Email.Contains(query));
            }
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)users.Count() / 7);
            if (page < 1) page = 1;
            if (page > paginationPages && paginationPages > 0) page = paginationPages;
            users = users.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;
            return View(users.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.roles = roles;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterVM registerVM, string RoleType)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser
                {
                    UserName = registerVM.UserName,
                    Email = registerVM.Email,
                    Address = registerVM.Adress,
                    PhoneNumber = registerVM.PhoneNumber
                };

                var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, RoleType);
                    TempData["Success"] = "Account Created Successfully!";

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            var roles = _roleManager.Roles.ToList();
            ViewBag.roles = roles;
            return View();
        }

        public IActionResult Delete(string userId)
        {
            var userAcount = _userRepository.GetOne(e => e.Id == userId);
            if (userAcount != null)
            {
                _userRepository.Delete(userAcount);
                _userRepository.Commit();
                TempData["Success"] = "Account Deleted Successfully!";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Block(string userId)
        {
            var user = _userRepository.GetOne(e => e.Id == userId);
            if (user != null)
            {
                // Block for 15 days from now
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddDays(15);

                _userRepository.Edit(user);
                _userRepository.Commit();

                TempData["Success"] = "Account has been blocked for 15 days successfully!";
            }
            else
            {
                TempData["Error"] = "User not found.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult UnBlock(string userId)
        {
            var user = _userRepository.GetOne(e => e.Id == userId);
            if (user != null)
            {
                user.LockoutEnabled = false;
                user.LockoutEnd = null;

                _userRepository.Edit(user);
                _userRepository.Commit();

                TempData["Success"] = "UnBlocked Successfully!";
            }
            else
            {
                TempData["Error"] = "User not found.";
            }

            return RedirectToAction("Index");
        }
    }
}
