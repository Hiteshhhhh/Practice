using System.Data;
using Models;
using Npgsql;

namespace Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly NpgsqlConnection conn;

        public ProductRepository(IConfiguration config)
        {
            conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public List<ProductModel> GetAllProducts()
        {
            var products = new List<ProductModel>();
            try
            {
                conn.Open();
                var query = "SELECT * FROM t_products ORDER BY c_created_at DESC";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new ProductModel
                            {
                                c_id = Convert.ToInt32(reader["c_id"]),
                                c_name = reader["c_name"].ToString(),
                                c_price = Convert.ToDecimal(reader["c_price"]),
                                c_stock = Convert.ToInt32(reader["c_stock"]),
                                c_created_at = Convert.ToDateTime(reader["c_created_at"])
                            });
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
            return products;
        }

        public ProductModel? GetProductById(int id)
        {
            try
            {
                conn.Open();
                var query = "SELECT * FROM t_products WHERE c_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ProductModel
                            {
                                c_id = Convert.ToInt32(reader["c_id"]),
                                c_name = reader["c_name"].ToString(),
                                c_price = Convert.ToDecimal(reader["c_price"]),
                                c_stock = Convert.ToInt32(reader["c_stock"]),
                                c_created_at = Convert.ToDateTime(reader["c_created_at"])
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

        public bool AddProduct(ProductModel product)
        {
            try
            {
                conn.Open();
                var query = "INSERT INTO t_products (c_name, c_price, c_stock) VALUES (@name, @price, @stock)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", product.c_name);
                    cmd.Parameters.AddWithValue("@price", product.c_price);
                    cmd.Parameters.AddWithValue("@stock", product.c_stock);

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

        public bool UpdateProduct(ProductModel product)
        {
            try
            {
                conn.Open();
                var query = "UPDATE t_products SET c_name = @name, c_price = @price, c_stock = @stock WHERE c_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", product.c_id);
                    cmd.Parameters.AddWithValue("@name", product.c_name);
                    cmd.Parameters.AddWithValue("@price", product.c_price);
                    cmd.Parameters.AddWithValue("@stock", product.c_stock);

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

        public bool DeleteProduct(int id)
        {
            try
            {
                conn.Open();
                var query = "DELETE FROM t_products WHERE c_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

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