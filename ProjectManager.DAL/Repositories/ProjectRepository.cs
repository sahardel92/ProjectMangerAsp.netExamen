using Microsoft.Data.SqlClient;
using ProjectManager.DAL.Connection;
using ProjectManager.DAL.Models;

namespace ProjectManager.DAL.Repositories
{
    public class ProjectRepository
    {
        private readonly DatabaseConnection _db;

        public ProjectRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public List<Project> GetProjectsByEmployeeId(Guid employeeId)
        {
            List<Project> projects = new List<Project>();
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SP_Project_Get_FromEmployeeId", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@employeeId", employeeId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                projects.Add(new Project
                {
                    ProjectId = (Guid)reader["ProjectId"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Creationdate = (DateTime)reader["Creationdate"],
                    ProjectManagerId = (Guid)reader["ProjectManagerId"]
                });
            }
            return projects;
        }

        public List<Project> GetProjectsByManagerId(Guid managerId)
        {
            List<Project> projects = new List<Project>();
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SP_Project_Get_FromProjectManagerId", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@projectManagerId", managerId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                projects.Add(new Project
                {
                    ProjectId = (Guid)reader["ProjectId"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Creationdate = (DateTime)reader["CreationDate"],
                    ProjectManagerId = (Guid)reader["ProjectManagerId"]
                });
            }
            return projects;
        }

        public Project? GetProjectById(Guid projectId)
        {
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SP_Project_Get_ById", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@projectId", projectId);
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Project
                {
                    ProjectId = (Guid)reader["ProjectId"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Creationdate = (DateTime)reader["CreationDate"],
                    ProjectManagerId = (Guid)reader["ProjectManagerId"]
                };
            }
            return null;
        }

        public List<Employee> GetMembersByProjectId(Guid projectId)
        {
            List<Employee> members = new List<Employee>();
            using SqlConnection conn = _db.GetConnection();
            conn.Open();

            string query =
                "SELECT e.EmployeeId, e.Firstname, e.Lastname, e.IsProjectManager, u.Email " +
                "FROM Employee e " +
                "JOIN [User] u ON u.EmployeeId = e.EmployeeId " +
                "JOIN TakePart tp ON tp.EmployeeId = e.EmployeeId " +
                "WHERE tp.ProjectId = @projectId AND tp.EndDate IS NULL " +
                "UNION " +
                "SELECT e.EmployeeId, e.Firstname, e.Lastname, e.IsProjectManager, u.Email " +
                "FROM Employee e " +
                "JOIN [User] u ON u.EmployeeId = e.EmployeeId " +
                "JOIN Project p ON p.ProjectManagerId = e.EmployeeId " +
                "WHERE p.ProjectId = @projectId2";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@projectId", projectId);
            cmd.Parameters.AddWithValue("@projectId2", projectId);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                members.Add(new Employee
                {
                    EmployeeId = (Guid)reader["EmployeeId"],
                    Firstname = reader["Firstname"].ToString(),
                    Lastname = reader["Lastname"].ToString(),
                    IsProjectManager = (bool)reader["IsProjectManager"],
                    Email = reader["Email"].ToString()
                });
            }
            return members;
        }

        public List<Post> GetPostsByProjectId(Guid projectId, bool isManager, Guid employeeId)
        {
            List<Post> posts = new List<Post>();
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            string spName = isManager
                ? "SP_Post_Get_FromProjectId_ProjectManager"
                : "SP_Post_Get_FromProjectId_WorkOnProject";
            using SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@projectId", projectId);
            if (!isManager)
                cmd.Parameters.AddWithValue("@employeeId", employeeId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                posts.Add(new Post
                {
                    PostId = (Guid)reader["PostId"],
                    Subject = reader["Subject"].ToString(),
                    Content = reader["Content"].ToString(),
                    SendDate = (DateTime)reader["SendDate"],
                    EmployeeId = (Guid)reader["EmployeeId"],
                    ProjectId = (Guid)reader["ProjectId"]
                });
            }
            return posts;
        }

        public Guid CreateProject(Guid managerId, string name, string description)
        {
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SP_Project_Insert", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@projectManagerId", managerId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@description", description);
            object? result = cmd.ExecuteScalar();
            return (Guid)result!;
        }

        public void UpdateDescription(Guid projectId, string description)
        {
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SP_Project_Update", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@projectId", projectId);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.ExecuteNonQuery();
        }

        public void AddMember(Guid employeeId, Guid projectId)
        {
            using SqlConnection conn = _db.GetConnection();
            conn.Open();

            using SqlCommand check = new SqlCommand(
                "SELECT COUNT(*) FROM TakePart WHERE EmployeeId = @employeeId AND ProjectId = @projectId AND EndDate IS NULL", conn);
            check.Parameters.AddWithValue("@employeeId", employeeId);
            check.Parameters.AddWithValue("@projectId", projectId);
            int count = (int)check.ExecuteScalar();
            if (count > 0) return;

            using SqlCommand cmd = new SqlCommand("SP_TakePart_Insert", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@employeeId", employeeId);
            cmd.Parameters.AddWithValue("@projectId", projectId);
            cmd.Parameters.AddWithValue("@startDate", DateTime.Now);
            cmd.ExecuteNonQuery();
        }

        public void RemoveMember(Guid employeeId, Guid projectId)
        {
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SP_TakePart_SetEnd", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@employeeId", employeeId);
            cmd.Parameters.AddWithValue("@projectId", projectId);
            cmd.Parameters.AddWithValue("@endDate", DateTime.Now);
            cmd.ExecuteNonQuery();
        }

        public List<Employee> GetFreeEmployees()
        {
            List<Employee> employees = new List<Employee>();
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SP_Employee_GetFree", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                employees.Add(new Employee
                {
                    EmployeeId = (Guid)reader["EmployeeId"],
                    Firstname = reader["Firstname"].ToString(),
                    Lastname = reader["Lastname"].ToString()
                });
            }
            return employees;
        }
    }
}