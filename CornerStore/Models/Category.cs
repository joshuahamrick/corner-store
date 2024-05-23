namespace CornerStore.Models;

public class Category
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public List<Product> Products { get; set; } // One-to-many relationship with Product
}
