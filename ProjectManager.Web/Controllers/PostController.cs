using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.Services;
using ProjectManager.DAL.Models;
using ProjectManager.Web.Filters;

namespace ProjectManager.Web.Controllers
{
    [AuthFilter]
    public class PostController : Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        public IActionResult Index()
        {
            string? employeeIdStr = HttpContext.Session.GetString("EmployeeId");
            if (employeeIdStr == null) return RedirectToAction("Login", "Account");

            Guid employeeId = Guid.Parse(employeeIdStr);
            List<Post> posts = _postService.GetPostsByEmployeeId(employeeId);
            return View(posts);
        }

        [HttpGet]
        public IActionResult Create(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Guid projectId, string subject, string content)
        {
            string? employeeIdStr = HttpContext.Session.GetString("EmployeeId");
            if (employeeIdStr == null) return RedirectToAction("Login", "Account");

            Guid employeeId = Guid.Parse(employeeIdStr);
            _postService.CreatePost(employeeId, projectId, subject, content);

            return RedirectToAction("Details", "Project", new { id = projectId });
        }
    }
}
