namespace ProjectManager.DAL.Models
{
    public class ProjectDetailViewModel
    {
        public Project? Project { get; set; }
        public List<Employee> Members { get; set; } = new List<Employee>();
        public Employee? Manager { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}