using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           SignInManager<ApplicationUser> signInManager
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (_roleManager.Roles.IsNullOrEmpty())
            {
               await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
               await _roleManager.CreateAsync(new IdentityRole("Admin"));
               await _roleManager.CreateAsync(new IdentityRole("Customer"));
                await _roleManager.CreateAsync(new IdentityRole("Company"));
            }
            return View(new RegisterVM());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ()
                {
                    UserName=registerVM.UserName,
                    Email=registerVM.Email,
                    Address=registerVM.Adress,
                    PhoneNumber=registerVM.PhoneNumber
                };

              var result=await _userManager.CreateAsync(applicationUser, registerVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, "Customer");
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                 }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }


            }
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var appUser =await _userManager.FindByEmailAsync(loginVM.Email);
                //if (appUser!= null&& appUser.LockoutEnabled && appUser.LockoutEnd > DateTimeOffset.Now)
                if (appUser!= null )
                {
                    var result =await _userManager.CheckPasswordAsync(appUser, loginVM.Password);

                    if (result)
                    {
                        await  _signInManager.SignInAsync(appUser, loginVM.RememberMe);
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Invalid Password Try Again!");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Invalid Email Try Again!");
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area="Identity" });
        }

        public IActionResult AccessDenied() => View();
    }
}
