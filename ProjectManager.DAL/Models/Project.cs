namespace ProjectManager.DAL.Models
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime Creationdate { get; set; }
        public Guid ProjectManagerId { get; set; }
    }
}
