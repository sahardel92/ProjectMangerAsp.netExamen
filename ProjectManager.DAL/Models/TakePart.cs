namespace ProjectManager.DAL.Models
{
    public class TakePart
    {
        public Guid EmployeeId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Employee? Employee { get; set; }
    }
}
