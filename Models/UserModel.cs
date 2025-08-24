using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UserModel
    {
        public int c_id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string c_username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string c_password { get; set; }

        public string c_role { get; set; } = "User";
    }
}