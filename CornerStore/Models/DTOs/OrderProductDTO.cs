
using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class OrderProductDTO
{
    public int ProductId { get; set; }
    public ProductDTO Product { get; set; } // Many-to-one relationship with Product
    public int OrderId { get; set; }
    public OrderDTO Order { get; set; } // Many-to-one relationship with Order
    [Required]
    public int Quantity { get; set; }
}