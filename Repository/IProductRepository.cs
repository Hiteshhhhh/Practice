using Models;

namespace Repository
{
    public interface IProductRepository
    {
        // Product CRUD
        List<ProductModel> GetAllProducts();
        List<ProductModel> GetProductsByUser(int userId);
        ProductModel? GetProductById(int id);
        void AddProduct(ProductModel product);
        void UpdateProduct(ProductModel product);
        void DeleteProduct(int id);
        void ToggleProductStatus(int id, bool isActive);
        
        // Categories
        List<CategoryModel> GetAllCategories();
        
        // Statistics
        int GetTotalProductCount();
        int GetProductCountByUser(int userId);
        decimal GetTotalValue();
    }
}