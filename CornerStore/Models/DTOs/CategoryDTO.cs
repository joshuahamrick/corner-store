namespace CornerStore.Models;

public class CategoryDTO
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public List<ProductDTO> Products { get; set; } // One-to-many relationship with Product
}
