using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;

namespace Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        //Constructor
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: /User/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            if (string.IsNullOrEmpty(user.c_username) || string.IsNullOrEmpty(user.c_password))
            {
                ViewBag.Error = "Please enter username and password";
                return View();
            }

            var validUser = _userRepository.ValidateUser(user.c_username, user.c_password);

            if (validUser != null)
            {
                // Set session
                HttpContext.Session.SetInt32("userid", validUser.c_id);
                HttpContext.Session.SetString("username", validUser.c_username);
                HttpContext.Session.SetString("role", validUser.c_role);

                TempData["Success"] = $"Welcome {validUser.c_username}!";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }
        }

        // GET: /User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user.c_role = "User"; // Default role

            if (_userRepository.RegisterUser(user))
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "Registration failed. Username might already exist.";
                return View(user);
            }
        }

        // POST: /User/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}

