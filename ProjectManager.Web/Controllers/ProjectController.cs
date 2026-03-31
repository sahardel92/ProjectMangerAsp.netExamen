using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.Services;
using ProjectManager.DAL.Models;
using ProjectManager.Web.Filters;

namespace ProjectManager.Web.Controllers
{
    [AuthFilter]
    public class ProjectController : Controller
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        public IActionResult Index()
        {
            string? employeeIdStr = HttpContext.Session.GetString("EmployeeId");
            if (employeeIdStr == null) return RedirectToAction("Login", "Account");

            Guid employeeId = Guid.Parse(employeeIdStr);
            bool isManager = HttpContext.Session.GetString("IsProjectManager") == "True";

            List<Project> projects = isManager
                ? _projectService.GetProjectsByManagerId(employeeId)
                : _projectService.GetProjectsByEmployeeId(employeeId);

            ViewBag.IsProjectManager = isManager;
            return View(projects);
        }

        public IActionResult Details(Guid id)
        {
            string? employeeIdStr = HttpContext.Session.GetString("EmployeeId");
            if (employeeIdStr == null) return RedirectToAction("Login", "Account");

            Guid employeeId = Guid.Parse(employeeIdStr);
            bool isManager = HttpContext.Session.GetString("IsProjectManager") == "True";

            ProjectDetailViewModel model = _projectService.GetProjectDetails(id, isManager, employeeId);
            return View(model);
        }
    }
}