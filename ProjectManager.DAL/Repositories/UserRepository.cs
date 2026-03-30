using Microsoft.Data.SqlClient;
using ProjectManager.DAL.Connection;
using ProjectManager.DAL.Models;

namespace ProjectManager.DAL.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseConnection _db;

        public UserRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public User? CheckPassword(string email, string password)
        {
            using SqlConnection conn = _db.GetConnection();
            conn.Open();

            // Étape 1 : vérifier email + password via la SP du prof
            using SqlCommand cmd = new SqlCommand("SP_User_CheckPassword", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);

            object? result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value) return null;

            Guid employeeId = (Guid)result;

            // Étape 2 : récupérer les infos complètes
            using SqlCommand cmd2 = new SqlCommand(
                "SELECT u.UserId, u.Email, u.EmployeeId, " +
                "e.Firstname, e.Lastname, e.IsProjectManager " +
                "FROM [User] u " +
                "JOIN Employee e ON u.EmployeeId = e.EmployeeId " +
                "WHERE u.EmployeeId = @EmployeeId", conn);
            cmd2.Parameters.AddWithValue("@EmployeeId", employeeId);

            using SqlDataReader reader = cmd2.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = (Guid)reader["UserId"],
                    Email = reader["Email"].ToString(),
                    EmployeeId = (Guid)reader["EmployeeId"],
                    Employee = new Employee
                    {
                        EmployeeId = (Guid)reader["EmployeeId"],
                        Firstname = reader["Firstname"].ToString(),
                        Lastname = reader["Lastname"].ToString(),
                        IsProjectManager = (bool)reader["IsProjectManager"]
                    }
                };
            }
            return null;
        }
    }
}