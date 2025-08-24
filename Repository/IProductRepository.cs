using Models;

namespace Repository
{
    public interface IProductRepository
    {
        List<ProductModel> GetAllProducts();
        ProductModel? GetProductById(int id);
        bool AddProduct(ProductModel product);
        bool UpdateProduct(ProductModel product);
        bool DeleteProduct(int id);
    }
}