using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Repository;

namespace Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public ProductController(IProductRepository productRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("role");
            var userId = HttpContext.Session.GetInt32("userid");

            if (string.IsNullOrEmpty(role) || userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<ProductModel> products;

            switch (role)
            {
                case "Admin":
                    // Admin sees ALL products
                    products = _productRepository.GetAllProducts();
                    ViewBag.CanDelete = true;
                    ViewBag.CanEditAll = true;
                    ViewBag.CanToggleStatus = true;
                    break;
                case "User":
                    // User sees only their own products
                    products = _productRepository.GetProductsByUser(userId.Value);
                    ViewBag.CanDelete = false;
                    ViewBag.CanEditAll = false;
                    ViewBag.CanToggleStatus = false;
                    break;

                default:
                    return RedirectToAction("Login", "User");
            }

            ViewBag.UserRole = role;
            ViewBag.UserId = userId;
            ViewBag.TotalProducts = products.Count;
            ViewBag.TotalValue = _productRepository.GetTotalValue();

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "User");
            }

            try
            {
                var categories = _productRepository.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "c_id", "c_name");
                return View(new ProductModel());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading form: {ex.Message}";
                ViewBag.Categories = new SelectList(new List<CategoryModel>(), "c_id", "c_name");
                return View(new ProductModel());
            }
        }

        [HttpPost]
        public IActionResult Create(ProductModel product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var categories = _productRepository.GetAllCategories();
                    ViewBag.Categories = new SelectList(categories, "c_id", "c_name", product.c_category_id);
                    return View(product);
                }

                // Handle file upload
                if (product.Photo != null && product.Photo.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Photo.FileName);
                    var path = Path.Combine("wwwroot/images", fileName);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        product.Photo.CopyTo(stream);
                    }
                    product.c_image = fileName;
                }

                // Set created by user
                var userId = HttpContext.Session.GetInt32("userid");
                product.c_created_by = userId ?? 1;

                _productRepository.AddProduct(product);
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error creating product: {ex.Message}";
                var categories = _productRepository.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "c_id", "c_name", product.c_category_id);
                return View(product);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var role = HttpContext.Session.GetString("role");
            var userId = HttpContext.Session.GetInt32("userid");

            if (string.IsNullOrEmpty(role) || userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }

            // Check permissions
            if (role == "User" && product.c_created_by != userId)
            {
                TempData["Error"] = "You don't have permission to edit this product.";
                return RedirectToAction("Index");
            }

            var categories = _productRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "c_id", "c_name", product.c_category_id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(ProductModel product)
        {
            try
            {
                var role = HttpContext.Session.GetString("role");
                var userId = HttpContext.Session.GetInt32("userid");

                // Verify permissions
                var existingProduct = _productRepository.GetProductById(product.c_id);
                if (existingProduct == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                if (role == "User" && existingProduct.c_created_by != userId)
                {
                    TempData["Error"] = "You don't have permission to edit this product.";
                    return RedirectToAction("Index");
                }

                if (!ModelState.IsValid)
                {
                    var categories = _productRepository.GetAllCategories();
                    ViewBag.Categories = new SelectList(categories, "c_id", "c_name", product.c_category_id);
                    return View(product);
                }

                // Handle file upload
                if (product.Photo != null && product.Photo.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Photo.FileName);
                    var path = Path.Combine("wwwroot/images", fileName);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        product.Photo.CopyTo(stream);
                    }
                    product.c_image = fileName;
                }
                else
                {
                    // Keep existing image
                    product.c_image = existingProduct.c_image;
                }

                _productRepository.UpdateProduct(product);
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error updating product: {ex.Message}";
                var categories = _productRepository.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "c_id", "c_name", product.c_category_id);
                return View(product);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var role = HttpContext.Session.GetString("role");
            
            if (role != "Admin")
            {
                TempData["Error"] = "Only administrators can delete products.";
                return RedirectToAction("Index");
            }

            try
            {
                _productRepository.DeleteProduct(id);
                TempData["Success"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting product: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var role = HttpContext.Session.GetString("role");
            
            if (role == "User")
            {
                TempData["Error"] = "You don't have permission to change product status.";
                return RedirectToAction("Index");
            }

            try
            {
                var product = _productRepository.GetProductById(id);
                if (product != null)
                {
                    _productRepository.ToggleProductStatus(id, !product.c_is_active);
                    TempData["Success"] = $"Product {(product.c_is_active ? "deactivated" : "activated")} successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating product status: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }

            return View(product);
        }
    }
}