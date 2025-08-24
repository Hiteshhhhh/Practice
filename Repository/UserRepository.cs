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
            var connectionString = config.GetConnectionString("DefaultConnection");
            conn = new NpgsqlConnection(connectionString);
        }

        public UserModel? ValidateUser(string username, string password)
        {
            UserModel? user = null;
            try
            {
                conn.Open();
                var query = "SELECT * FROM t_users WHERE c_username = @username AND c_password = @password";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            c_id = reader.GetInt32("c_id"),
                            c_username = reader.GetString("c_username"),
                            c_email = reader.GetString("c_email"),
                            c_role = reader.GetString("c_role"),
                            c_created_at = reader.GetDateTime("c_created_at")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ValidateUser: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return user;
        }

        public bool RegisterUser(UserModel user)
        {
            try
            {
                conn.Open();
                var query = "INSERT INTO t_users (c_username, c_email, c_password, c_role) VALUES (@username, @email, @password, @role)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", user.c_username);
                    cmd.Parameters.AddWithValue("@email", user.c_email);
                    cmd.Parameters.AddWithValue("@password", user.c_password);
                    cmd.Parameters.AddWithValue("@role", user.c_role);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisterUser: {ex.Message}");
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public bool IsUsernameExists(string username)
        {
            try
            {
                conn.Open();
                var query = "SELECT COUNT(*) FROM t_users WHERE c_username = @username";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IsUsernameExists: {ex.Message}");
                return true; // Safe default
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public bool IsEmailExists(string email)
        {
            try
            {
                conn.Open();
                var query = "SELECT COUNT(*) FROM t_users WHERE c_email = @email";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IsEmailExists: {ex.Message}");
                return true; // Safe default
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public UserModel? GetUserById(int id)
        {
            UserModel? user = null;
            try
            {
                conn.Open();
                var query = "SELECT * FROM t_users WHERE c_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            c_id = reader.GetInt32("c_id"),
                            c_username = reader.GetString("c_username"),
                            c_email = reader.GetString("c_email"),
                            c_role = reader.GetString("c_role"),
                            c_created_at = reader.GetDateTime("c_created_at")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserById: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return user;
        }

        public List<UserModel> GetAllUsers()
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                conn.Open();
                var query = "SELECT * FROM t_users ORDER BY c_created_at DESC";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            c_id = reader.GetInt32("c_id"),
                            c_username = reader.GetString("c_username"),
                            c_email = reader.GetString("c_email"),
                            c_role = reader.GetString("c_role"),
                            c_created_at = reader.GetDateTime("c_created_at")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllUsers: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return users;
        }
    }
}