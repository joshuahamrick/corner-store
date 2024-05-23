namespace CornerStore.Models;

public class CashierDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}"; // Computed property for FullName
    public List<OrderDTO>? Orders { get; set; } // One-to-many relationship with Order
}