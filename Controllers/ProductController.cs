using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using First_Project.Models;

namespace First_Project.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    public IActionResult ShowStudent(Product product)
    {
        ViewBag.id = product.Prod_Id;
        ViewBag.name = product.Prod_name;
        ViewBag.qty = product.Prod_Qty;
        ViewBag.price = product.Prod_Price;
        return View();
    }

    
}
    