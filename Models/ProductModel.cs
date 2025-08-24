using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProductModel
    {
        public int c_id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string c_name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal c_price { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int c_stock { get; set; }

        public DateTime c_created_at { get; set; } = DateTime.Now;
    }
}