namespace CornerStore.Models;

public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Brand { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } // Many-to-one relationship with Category
    public List<OrderProduct> OrderProducts { get; set; } // One-to-many relationship with OrderProduct
}
