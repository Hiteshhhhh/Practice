using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProductModel
    {
        public int c_id { get; set; }
        
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100)]
        public string c_name { get; set; } = "";
        
        public string c_description { get; set; } = "";
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal c_price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int c_stock_quantity { get; set; }
        
        [Required(ErrorMessage = "Category is required")]
        public int c_category_id { get; set; }
        
        public string c_image { get; set; } = "";
        public bool c_is_active { get; set; } = true;
        public int c_created_by { get; set; }
        public DateTime c_created_at { get; set; }
        public DateTime c_updated_at { get; set; }
        
        // Navigation properties (for display)
        public string CategoryName { get; set; } = "";
        public string CreatedByUsername { get; set; } = "";
        
        // For file upload
        public IFormFile? Photo { get; set; }
    }
}