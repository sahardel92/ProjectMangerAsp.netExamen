using ProjectManager.DAL.Models;
using ProjectManager.DAL.Repositories;

namespace ProjectManager.BLL.Services
{
    public class ProjectService
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectService(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public List<Project> GetProjectsByEmployeeId(Guid employeeId)
        {
            return _projectRepository.GetProjectsByEmployeeId(employeeId);
        }

        public List<Project> GetProjectsByManagerId(Guid managerId)
        {
            return _projectRepository.GetProjectsByManagerId(managerId);
        }

        public ProjectDetailViewModel GetProjectDetails(Guid projectId, bool isManager, Guid employeeId)
        {
            Project? project = _projectRepository.GetProjectById(projectId);
            List<Employee> members = _projectRepository.GetMembersByProjectId(projectId);
            List<Post> posts = _projectRepository.GetPostsByProjectId(projectId, isManager, employeeId);

            Employee? manager = members.FirstOrDefault(e => e.IsProjectManager);

            return new ProjectDetailViewModel
            {
                Project = project,
                Members = members,
                Manager = manager,
                Posts = posts
            };
        }
    }
}