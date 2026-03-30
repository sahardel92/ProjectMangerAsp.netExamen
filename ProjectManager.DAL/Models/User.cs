namespace ProjectManager.DAL.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
