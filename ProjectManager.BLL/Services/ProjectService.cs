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

        public Guid CreateProject(Guid managerId, string name, string description)
        {
            return _projectRepository.CreateProject(managerId, name, description);
        }

        public List<Project> GetProjectsByEmployeeId(Guid employeeId)
        {
            return _projectRepository.GetProjectsByEmployeeId(employeeId);
        }

        public List<Project> GetProjectsByManagerId(Guid managerId)
        {
            return _projectRepository.GetProjectsByManagerId(managerId);
        }

        public void UpdateDescription(Guid projectId, string description)
        {
            _projectRepository.UpdateDescription(projectId, description);
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
        public void AddMember(Guid employeeId, Guid projectId)
        {
            _projectRepository.AddMember(employeeId, projectId);
        }

        public void RemoveMember(Guid employeeId, Guid projectId)
        {
            _projectRepository.RemoveMember(employeeId, projectId);
        }
        public List<Employee> GetFreeEmployees()
        {
            return _projectRepository.GetFreeEmployees();
        }
    }
}