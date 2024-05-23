using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class OrderProduct
{
    public List<int> ProductId { get; set; }
    public List<Product> Product { get; set; } // Many-to-one relationship with Product
    public int OrderId { get; set; }
    public Order Order { get; set; } // Many-to-one relationship with Order
    [Required]
    public int Quantity { get; set; }
}