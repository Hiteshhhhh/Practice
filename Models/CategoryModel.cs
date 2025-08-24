namespace Models
{
    public class CategoryModel
    {
        public int c_id { get; set; }
        public string c_name { get; set; } = "";
        public string c_description { get; set; } = "";
        public DateTime c_created_at { get; set; }
    }
}