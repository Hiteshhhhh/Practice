using System.Data;
using Models;
using Npgsql;

namespace Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly NpgsqlConnection conn;
        private readonly IHttpContextAccessor _accessor;

        public ProductRepository(IConfiguration config, IHttpContextAccessor accessor)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            conn = new NpgsqlConnection(connectionString);
            _accessor = accessor;
        }

        public List<ProductModel> GetAllProducts()
        {
            List<ProductModel> products = new List<ProductModel>();
            try
            {
                conn.Open();
                var query = @"
                    SELECT p.*, c.c_name as category_name, u.c_username as created_by_username 
                    FROM t_products p 
                    LEFT JOIN t_categories c ON p.c_category_id = c.c_id 
                    LEFT JOIN t_users u ON p.c_created_by = u.c_id 
                    ORDER BY p.c_created_at DESC";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        products.Add(MapReaderToProduct(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllProducts: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return products;
        }

        public List<ProductModel> GetProductsByUser(int userId)
        {
            List<ProductModel> products = new List<ProductModel>();
            try
            {
                conn.Open();
                var query = @"
                    SELECT p.*, c.c_name as category_name, u.c_username as created_by_username 
                    FROM t_products p 
                    LEFT JOIN t_categories c ON p.c_category_id = c.c_id 
                    LEFT JOIN t_users u ON p.c_created_by = u.c_id 
                    WHERE p.c_created_by = @userId 
                    ORDER BY p.c_created_at DESC";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        products.Add(MapReaderToProduct(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductsByUser: {ex.Message}");
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
            ProductModel? product = null;
            try
            {
                conn.Open();
                var query = @"
                    SELECT p.*, c.c_name as category_name, u.c_username as created_by_username 
                    FROM t_products p 
                    LEFT JOIN t_categories c ON p.c_category_id = c.c_id 
                    LEFT JOIN t_users u ON p.c_created_by = u.c_id 
                    WHERE p.c_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        product = MapReaderToProduct(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductById: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return product;
        }

        public void AddProduct(ProductModel product)
        {
            try
            {
                conn.Open();
                var query = @"
                    INSERT INTO t_products (c_name, c_description, c_price, c_stock_quantity, 
                                          c_category_id, c_image, c_is_active, c_created_by) 
                    VALUES (@name, @description, @price, @stock, @category, @image, @active, @createdBy)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", product.c_name);
                    cmd.Parameters.AddWithValue("@description", product.c_description ?? "");
                    cmd.Parameters.AddWithValue("@price", product.c_price);
                    cmd.Parameters.AddWithValue("@stock", product.c_stock_quantity);
                    cmd.Parameters.AddWithValue("@category", product.c_category_id);
                    cmd.Parameters.AddWithValue("@image", product.c_image ?? "");
                    cmd.Parameters.AddWithValue("@active", product.c_is_active);
                    cmd.Parameters.AddWithValue("@createdBy", product.c_created_by);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Product added successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddProduct: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public void UpdateProduct(ProductModel product)
        {
            try
            {
                conn.Open();
                var query = @"
                    UPDATE t_products 
                    SET c_name = @name, c_description = @description, c_price = @price, 
                        c_stock_quantity = @stock, c_category_id = @category, 
                        c_image = @image, c_is_active = @active, c_updated_at = CURRENT_TIMESTAMP 
                    WHERE c_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", product.c_id);
                    cmd.Parameters.AddWithValue("@name", product.c_name);
                    cmd.Parameters.AddWithValue("@description", product.c_description ?? "");
                    cmd.Parameters.AddWithValue("@price", product.c_price);
                    cmd.Parameters.AddWithValue("@stock", product.c_stock_quantity);
                    cmd.Parameters.AddWithValue("@category", product.c_category_id);
                    cmd.Parameters.AddWithValue("@image", product.c_image ?? "");
                    cmd.Parameters.AddWithValue("@active", product.c_is_active);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Product updated successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProduct: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public void DeleteProduct(int id)
        {
            try
            {
                conn.Open();
                var query = "DELETE FROM t_products WHERE c_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Product deleted successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteProduct: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public void ToggleProductStatus(int id, bool isActive)
        {
            try
            {
                conn.Open();
                var query = "UPDATE t_products SET c_is_active = @active WHERE c_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@active", isActive);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleProductStatus: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public List<CategoryModel> GetAllCategories()
        {
            List<CategoryModel> categories = new List<CategoryModel>();
            try
            {
                conn.Open();
                var query = "SELECT * FROM t_categories ORDER BY c_name";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        categories.Add(new CategoryModel
                        {
                            c_id = reader.GetInt32("c_id"),
                            c_name = reader.GetString("c_name"),
                            c_description = reader.IsDBNull("c_description") ? "" : reader.GetString("c_description"),
                            c_created_at = reader.GetDateTime("c_created_at")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllCategories: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return categories;
        }

        public int GetTotalProductCount()
        {
            int count = 0;
            try
            {
                conn.Open();
                var query = "SELECT COUNT(*) FROM t_products WHERE c_is_active = true";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalProductCount: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return count;
        }

        public int GetProductCountByUser(int userId)
        {
            int count = 0;
            try
            {
                conn.Open();
                var query = "SELECT COUNT(*) FROM t_products WHERE c_created_by = @userId AND c_is_active = true";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductCountByUser: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return count;
        }

        public decimal GetTotalValue()
        {
            decimal value = 0;
            try
            {
                conn.Open();
                var query = "SELECT COALESCE(SUM(c_price * c_stock_quantity), 0) FROM t_products WHERE c_is_active = true";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    value = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalValue: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return value;
        }

        private ProductModel MapReaderToProduct(NpgsqlDataReader reader)
        {
            return new ProductModel
            {
                c_id = reader.GetInt32("c_id"),
                c_name = reader.GetString("c_name"),
                c_description = reader.IsDBNull("c_description") ? "" : reader.GetString("c_description"),
                c_price = reader.GetDecimal("c_price"),
                c_stock_quantity = reader.GetInt32("c_stock_quantity"),
                c_category_id = reader.GetInt32("c_category_id"),
                c_image = reader.IsDBNull("c_image") ? "" : reader.GetString("c_image"),
                c_is_active = reader.GetBoolean("c_is_active"),
                c_created_by = reader.GetInt32("c_created_by"),
                c_created_at = reader.GetDateTime("c_created_at"),
                c_updated_at = reader.IsDBNull("c_updated_at") ? DateTime.Now : reader.GetDateTime("c_updated_at"),
                CategoryName = reader.IsDBNull("category_name") ? "" : reader.GetString("category_name"),
                CreatedByUsername = reader.IsDBNull("created_by_username") ? "" : reader.GetString("created_by_username")
            };
        }
    }
}