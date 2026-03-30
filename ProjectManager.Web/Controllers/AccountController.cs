using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.Services;
using ProjectManager.DAL.Models;

namespace ProjectManager.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            User? user = _userService.Login(email, password);

            if (user == null)
            {
                ViewBag.Error = "Email ou mot de passe incorrect.";
                return View();
            }

            HttpContext.Session.SetString("UserEmail", user.Email!);
            HttpContext.Session.SetString("UserFirstname", user.Employee!.Firstname!);
            HttpContext.Session.SetString("UserLastname", user.Employee.Lastname!);
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("EmployeeId", user.EmployeeId.ToString());
            HttpContext.Session.SetString("IsProjectManager", user.Employee.IsProjectManager.ToString());

            return RedirectToAction("Index", "Project");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}


