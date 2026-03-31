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
            using SqlCommand cmd = new SqlCommand("SP_Employee_Get_FromProjectId", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@projectId", projectId);
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
    }
}