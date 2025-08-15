using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using First_Project.Models;

namespace First_Project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        Student student = new Student()
        {
            roll = 1,
            age = 24,
            gender = "Male",
            fee = 35000,
            Name = "Hitesh Mishra",
        };
        return View(student);
    }
    public IActionResult About()
    {
        return View();
    }
    public IActionResult ShowStudent()
    {
        ViewBag.id = Request.Form["id"];
        ViewBag.name = Request.Form["name"];
        ViewBag.age = Request.Form["age"];
        ViewBag.gender = Request.Form["gender"];
        ViewBag.fee = Request.Form["fee"];
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult MyForm()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
