
namespace CornerStore.Models;

public class OrderDTO
{
    public int Id { get; set; }
    public int CashierId { get; set; }
    public CashierDTO Cashier { get; set; } // Many-to-one relationship with Cashier
    public DateTime? PaidOnDate { get; set; }
    public List<OrderProductDTO> OrderProducts { get; set; } // One-to-many relationship with OrderProduct

    // Computed property for Total
    public decimal Total
    {
        get
        {
              if (OrderProducts == null)
        {
            return 0m; // Or throw an exception if a total is always expected
        }
            decimal total = 0M;
            foreach (var orderProduct in OrderProducts)
            {
                if (orderProduct.Product != null) {
                total += orderProduct.Product.Price * orderProduct.Quantity;
                }
            }
            return total;
        }
    }
}