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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name, string description)
        {
            string? employeeIdStr = HttpContext.Session.GetString("EmployeeId");
            if (employeeIdStr == null) return RedirectToAction("Login", "Account");

            Guid managerId = Guid.Parse(employeeIdStr);
            Guid projectId = _projectService.CreateProject(managerId, name, description);
            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpGet]
        public IActionResult EditDescription(Guid id)
        {
            string? employeeIdStr = HttpContext.Session.GetString("EmployeeId");
            if (employeeIdStr == null) return RedirectToAction("Login", "Account");

            Guid employeeId = Guid.Parse(employeeIdStr);
            ProjectDetailViewModel model = _projectService.GetProjectDetails(id, true, employeeId);
            return View(model);
        }

        [HttpPost]
        public IActionResult EditDescription(Guid id, string description)
        {
            _projectService.UpdateDescription(id, description);
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public IActionResult AddMember(Guid id)
        {
            List<Employee> freeEmployees = _projectService.GetFreeEmployees();
            ViewBag.ProjectId = id;
            return View(freeEmployees);
        }

        [HttpPost]
        public IActionResult AddMember(Guid projectId, Guid employeeId)
        {
            _projectService.AddMember(employeeId, projectId);
            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpPost]
        public IActionResult RemoveMember(Guid projectId, Guid employeeId)
        {
            _projectService.RemoveMember(employeeId, projectId);
            return RedirectToAction("Details", new { id = projectId });
        }
    }
}