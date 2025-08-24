using System.Data;
using Models;
using Npgsql;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection conn;

        public UserRepository(IConfiguration config)
        {
            conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public UserModel? ValidateUser(string username, string password)
        {
            try
            {
                conn.Open();
                var query = "SELECT * FROM t_users WHERE c_username = @username AND c_password = @password";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                c_id = Convert.ToInt32(reader["c_id"]),
                                c_username = reader["c_username"].ToString(),
                                c_role = reader["c_role"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return null;
        }

        public bool RegisterUser(UserModel user)
        {
            try
            {
                conn.Open();
                var query = "INSERT INTO t_users (c_username, c_password, c_role) VALUES (@username, @password, @role)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", user.c_username);
                    cmd.Parameters.AddWithValue("@password", user.c_password);
                    cmd.Parameters.AddWithValue("@role", user.c_role);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
}