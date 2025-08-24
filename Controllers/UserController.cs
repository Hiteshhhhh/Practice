using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;

namespace Controllers
{
    public class UserController : Controller
    {
        // Private field to store our user repository
        // readonly means we can only set it in constructor
        private readonly IUserRepository _userRepository;

        // Constructor - runs when creating UserController
        // ASP.NET automatically provides userRepository (Dependency Injection)
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: /User/Login
        // Shows the login page when user visits /User/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Check if user is already logged in
            // Session stores temporary data for each user
            var existingUser = HttpContext.Session.GetString("username");
            
            if (!string.IsNullOrEmpty(existingUser))
            {
                // User already logged in, redirect to products
                return RedirectToAction("Index", "Product");
            }
            
            // User not logged in, show login page
            return View();
        }

        // POST: /User/Login
        // Processes login form when user clicks "Login" button
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            try
            {
                // Step 1: Validate input
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Username and password are required.";
                    return View(); // Show login page again with error
                }

                // Step 2: Check credentials in database
                var user = _userRepository.ValidateUser(username, password);
                
                if (user != null) // Login successful
                {
                    // Step 3: Store user info in session
                    HttpContext.Session.SetString("username", user.c_username);
                    HttpContext.Session.SetString("role", user.c_role);
                    HttpContext.Session.SetInt32("userid", user.c_id);

                    // Step 4: Show success message and redirect
                    TempData["Success"] = $"Welcome back, {user.c_username}!";
                    return RedirectToAction("Index", "Product");
                }
                else // Login failed
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Handle any errors (database connection, etc.)
                ViewBag.Error = $"Login error: {ex.Message}";
                return View();
            }
        }

        // GET: /User/Register
        // Shows registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserModel()); // Pass empty user model to form
        }

        // POST: /User/Register
        // Processes registration form
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            try
            {
                // Step 1: Check validation rules (from UserModel Data Annotations)
                if (!ModelState.IsValid)
                {
                    // Validation failed, show form with error messages
                    return View(user);
                }

                // Step 2: Check if username already exists
                if (_userRepository.IsUsernameExists(user.c_username))
                {
                    ViewBag.Error = "Username already exists. Please choose a different one.";
                    return View(user);
                }

                // Step 3: Check if email already exists
                if (_userRepository.IsEmailExists(user.c_email))
                {
                    ViewBag.Error = "Email already exists. Please use a different email.";
                    return View(user);
                }

                // Step 4: Set default role for security
                if (string.IsNullOrEmpty(user.c_role))
                {
                    user.c_role = "User"; // New users start as regular users
                }

                // Step 5: Save user to database
                bool isRegistered = _userRepository.RegisterUser(user);
                
                if (isRegistered)
                {
                    TempData["Success"] = "Registration successful! Please login.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Registration failed. Please try again.";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Registration error: {ex.Message}";
                return View(user);
            }
        }

        // GET: /User/Logout
        // Logs user out and clears session
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();
            
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        // GET: /User/Profile
        // Shows current user's profile information
        [HttpGet]
        public IActionResult Profile()
        {
            // Step 1: Check if user is logged in
            var userId = HttpContext.Session.GetInt32("userid");
            
            if (userId == null)
            {
                // User not logged in, redirect to login
                return RedirectToAction("Login");
            }

            // Step 2: Get user details from database
            var user = _userRepository.GetUserById(userId.Value);
            
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login");
            }

            // Step 3: Show profile page
            return View(user);
        }

        // GET: /User/ChangePassword
        // Shows change password form
        [HttpGet]
        public IActionResult ChangePassword()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("userid");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // POST: /User/ChangePassword
        // Processes password change
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                // Step 1: Get current user info
                var userId = HttpContext.Session.GetInt32("userid");
                var username = HttpContext.Session.GetString("username");

                if (userId == null || string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                // Step 2: Validate input
                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
                {
                    ViewBag.Error = "All fields are required.";
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.Error = "New password and confirm password don't match.";
                    return View();
                }

                if (newPassword.Length < 6)
                {
                    ViewBag.Error = "New password must be at least 6 characters long.";
                    return View();
                }

                // Step 3: Verify current password
                var user = _userRepository.ValidateUser(username, currentPassword);
                if (user == null)
                {
                    ViewBag.Error = "Current password is incorrect.";
                    return View();
                }

                // Step 4: Update password
                bool passwordUpdated = _userRepository.UpdatePassword(userId.Value, newPassword);
                
                if (passwordUpdated)
                {
                    TempData["Success"] = "Password changed successfully!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    ViewBag.Error = "Failed to update password. Please try again.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error changing password: {ex.Message}";
                return View();
            }
        }

        // GET: /User/ManageUsers (Admin Only)
        // Shows list of all users for admin management
        [HttpGet]
        public IActionResult ManageUsers()
        {
            // Step 1: Check if user is Admin
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index", "Product");
            }

            try
            {
                // Step 2: Get all users from database
                var users = _userRepository.GetAllUsers();
                return View(users);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading users: {ex.Message}";
                return View(new List<UserModel>());
            }
        }

        // POST: /User/UpdateUserRole (Admin Only)
        // Changes a user's role (User/Manager/Admin)
        [HttpPost]
        public IActionResult UpdateUserRole(int userId, string newRole)
        {
            // Step 1: Check admin permission
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index", "Product");
            }

            try
            {
                // Step 2: Validate role
                if (newRole != "User" && newRole != "Manager" && newRole != "Admin")
                {
                    TempData["Error"] = "Invalid role specified.";
                    return RedirectToAction("ManageUsers");
                }

                // Step 3: Prevent admin from removing their own admin role
                var currentUserId = HttpContext.Session.GetInt32("userid");
                if (currentUserId == userId && newRole != "Admin")
                {
                    TempData["Error"] = "You cannot remove your own admin privileges.";
                    return RedirectToAction("ManageUsers");
                }

                // Step 4: Update role in database
                bool roleUpdated = _userRepository.UpdateUserRole(userId, newRole);
                
                if (roleUpdated)
                {
                    TempData["Success"] = $"User role updated to {newRole} successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to update user role.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating role: {ex.Message}";
            }

            return RedirectToAction("ManageUsers");
        }

        // POST: /User/DeleteUser (Admin Only)
        // Deletes a user account
        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index", "Product");
            }

            try
            {
                // Prevent admin from deleting themselves
                var currentUserId = HttpContext.Session.GetInt32("userid");
                if (currentUserId == userId)
                {
                    TempData["Error"] = "You cannot delete your own account.";
                    return RedirectToAction("ManageUsers");
                }

                bool userDeleted = _userRepository.DeleteUser(userId);
                
                if (userDeleted)
                {
                    TempData["Success"] = "User deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete user.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToAction("ManageUsers");
        }

        // GET: /User/ForgotPassword
        // Shows forgot password form (placeholder)
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /User/ForgotPassword
        // Processes forgot password (placeholder - would typically send email)
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    ViewBag.Error = "Email is required.";
                    return View();
                }

                // Check if email exists
                if (_userRepository.IsEmailExists(email))
                {
                    // In real app, you would send reset email here
                    TempData["Success"] = "If this email exists, a password reset link has been sent.";
                    return RedirectToAction("Login");
                }
                else
                {
                    // Don't reveal if email exists for security
                    TempData["Success"] = "If this email exists, a password reset link has been sent.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error processing request: {ex.Message}";
                return View();
            }
        }
    }
}