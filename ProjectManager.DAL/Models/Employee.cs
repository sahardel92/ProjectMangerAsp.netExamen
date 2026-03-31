namespace ProjectManager.DAL.Models
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public DateTime Hiredate { get; set; }
        public bool IsProjectManager { get; set; }
        public string? Email { get; set; }
    }
}