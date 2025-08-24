using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UserModel
    {
        public int c_id { get; set; }
        
        [Required(ErrorMessage = "Username is required")]
        public string c_username { get; set; } = "";
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string c_email { get; set; } = "";
        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string c_password { get; set; } = "";
        
        public string c_role { get; set; } = "User";
        public DateTime c_created_at { get; set; }
        
        [Compare("c_password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}