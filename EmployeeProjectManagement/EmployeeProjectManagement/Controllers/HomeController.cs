using EmployeeProjectManagement.Enum;
using EmployeeProjectManagement.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmployeeProjectManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context= new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public async Task<ActionResult> Index()
        {
            var user = await GetUserAsync();
            if (user!=null)
            {
                if (await UserManager.IsInRoleAsync(user.Id, RolesEnum.Manager.ToString()))
                {
                    return RedirectToAction(nameof(ManagerController.Index), "Manager");
                }
                else if (await UserManager.IsInRoleAsync(user.Id,RolesEnum.Employee.ToString()))
                {
                    return RedirectToAction(nameof(ManagerController.Index), "Employee");
                }
                else
                {
                    return View();
                }
            }
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private async Task<ApplicationUser> GetUserAsync()
        {
            return string.IsNullOrEmpty(HttpContext.User.Identity.Name) ? null : await _context.Users.Where(u => u.UserName == HttpContext.User.Identity.Name).FirstOrDefaultAsync();
        }
    }
}