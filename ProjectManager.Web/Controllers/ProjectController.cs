using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.Web.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            
            string? email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "Account");

            string firstname = HttpContext.Session.GetString("UserFirstname")!;
            string lastname = HttpContext.Session.GetString("UserLastname")!;
            bool isManager = HttpContext.Session.GetString("IsProjectManager") == "True";

            ViewBag.Fullname = firstname + " " + lastname;
            ViewBag.IsProjectManager = isManager;

            return View();
        }
    }
}