using Microsoft.Data.SqlClient;
using ProjectManager.DAL.Connection;
using ProjectManager.DAL.Models;

namespace ProjectManager.DAL.Repositories
{
    public class PostRepository
    {
        private readonly DatabaseConnection _db;

        public PostRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public void CreatePost(Guid employeeId, Guid projectId, string subject, string content)
        {
            using SqlConnection conn = _db.GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SP_Post_Insert", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@employeeId", employeeId);
            cmd.Parameters.AddWithValue("@projectId", projectId);
            cmd.Parameters.AddWithValue("@subject", subject);
            cmd.Parameters.AddWithValue("@content", content);
            cmd.ExecuteScalar();
        }

        public List<Post> GetPostsByEmployeeId(Guid employeeId)
        {
            List<Post> posts = new List<Post>();
            using SqlConnection conn = _db.GetConnection();
            conn.Open();

            string query =
                "SELECT DISTINCT p.PostId, p.Subject, p.[Content], p.SendDate, p.EmployeeId, p.ProjectId, " +
                "e.Firstname, e.Lastname, pr.Name AS ProjectName " +
                "FROM Post p " +
                "JOIN Employee e ON p.EmployeeId = e.EmployeeId " +
                "JOIN Project pr ON p.ProjectId = pr.ProjectId " +
                "WHERE pr.ProjectManagerId = @employeeId " +
                "OR EXISTS (" +
                "  SELECT 1 FROM TakePart tp " +
                "  WHERE tp.ProjectId = p.ProjectId " +
                "  AND tp.EmployeeId = @employeeId2 " +
                "  AND (tp.EndDate IS NULL OR p.SendDate <= tp.EndDate)" +
                ") " +
                "ORDER BY p.SendDate DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@employeeId", employeeId);
            cmd.Parameters.AddWithValue("@employeeId2", employeeId);

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
                    ProjectId = (Guid)reader["ProjectId"],
                    Employee = new Employee
                    {
                        Firstname = reader["Firstname"].ToString(),
                        Lastname = reader["Lastname"].ToString()
                    },
                    Project = new Project
                    {
                        Name = reader["ProjectName"].ToString()
                    }
                });
            }
            return posts;
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
    }
}