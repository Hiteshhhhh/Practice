using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;

namespace Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: /Product/Index
        [HttpGet]
        public IActionResult Index()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("userid");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var products = _productRepository.GetAllProducts();
            ViewBag.UserRole = HttpContext.Session.GetString("role");
            ViewBag.Username = HttpContext.Session.GetString("username");
            
            return View(products);
        }

        // GET: /Product/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("userid");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction("Index");
            }

            ViewBag.UserRole = HttpContext.Session.GetString("role");
            return View(product);
        }

        // GET: /Product/Create
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("userid");
            var role = HttpContext.Session.GetString("role");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Only admin can create products
            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Only admin can create products.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        public IActionResult Create(ProductModel product)
        {
            var userId = HttpContext.Session.GetInt32("userid");
            var role = HttpContext.Session.GetString("role");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Only admin can create products.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (_productRepository.AddProduct(product))
            {
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Error creating product";
                return View(product);
            }
        }

        // GET: /Product/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("userid");
            var role = HttpContext.Session.GetString("role");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Only admin can edit products.";
                return RedirectToAction("Index");
            }

            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        public IActionResult Edit(ProductModel product)
        {
            var userId = HttpContext.Session.GetInt32("userid");
            var role = HttpContext.Session.GetString("role");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Only admin can edit products.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (_productRepository.UpdateProduct(product))
            {
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Error updating product";
                return View(product);
            }
        }

        // POST: /Product/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("userid");
            var role = HttpContext.Session.GetString("role");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Only admin can delete products.";
                return RedirectToAction("Index");
            }

            if (_productRepository.DeleteProduct(id))
            {
                TempData["Success"] = "Product deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Error deleting product";
            }

            return RedirectToAction("Index");
        }
    }
}