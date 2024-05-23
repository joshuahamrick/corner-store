using Microsoft.EntityFrameworkCore;
using CornerStore.Models;
public class CornerStoreDbContext : DbContext
{


    public DbSet<Cashier> Cashiers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }

 
    public CornerStoreDbContext(DbContextOptions<CornerStoreDbContext> context) : base(context)
    {

    }

    //allows us to configure the schema when migrating as well as seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Seed Cashiers
        modelBuilder.Entity<Cashier>().HasData(
            new Cashier { Id = 1, FirstName = "John", LastName = "Doe" },
            new Cashier { Id = 2, FirstName = "Jane", LastName = "Smith" },
            new Cashier { Id = 3, FirstName = "Michael", LastName = "Johnson" },
            new Cashier { Id = 4, FirstName = "Emily", LastName = "Davis" }
        );

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, CategoryName = "Beverages" },
            new Category { Id = 2, CategoryName = "Snacks" },
            new Category { Id = 3, CategoryName = "Dairy" },
            new Category { Id = 4, CategoryName = "Bakery" }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, ProductName = "Coca Cola", Price = 1.5m, Brand = "Coca Cola", CategoryId = 1 },
            new Product { Id = 2, ProductName = "Pepsi", Price = 1.5m, Brand = "Pepsi", CategoryId = 1 },
            new Product { Id = 3, ProductName = "Lays", Price = 2.0m, Brand = "Lays", CategoryId = 2 },
            new Product { Id = 4, ProductName = "Milk", Price = 1.2m, Brand = "DairyPure", CategoryId = 3 },
            new Product { Id = 5, ProductName = "Bread", Price = 1.0m, Brand = "Wonder", CategoryId = 4 },
            new Product { Id = 6, ProductName = "Cheese", Price = 3.0m, Brand = "Kraft", CategoryId = 3 },
            new Product { Id = 7, ProductName = "Donuts", Price = 0.5m, Brand = "Krispy Kreme", CategoryId = 4 }
        );

        // Seed Orders
        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, CashierId = 1, PaidOnDate = DateTime.Now },
            new Order { Id = 2, CashierId = 2, PaidOnDate = DateTime.Now },
            new Order { Id = 3, CashierId = 3, PaidOnDate = DateTime.Now },
            new Order { Id = 4, CashierId = 4, PaidOnDate = DateTime.Now }
        );

        // Seed OrderProducts
        modelBuilder.Entity<OrderProduct>().HasData(
            new OrderProduct { OrderId = 1, ProductId = 1, Quantity = 2 },
            new OrderProduct { OrderId = 1, ProductId = 3, Quantity = 1 },
            new OrderProduct { OrderId = 2, ProductId = 2, Quantity = 1 },
            new OrderProduct { OrderId = 2, ProductId = 4, Quantity = 2 },
            new OrderProduct { OrderId = 3, ProductId = 5, Quantity = 3 },
            new OrderProduct { OrderId = 3, ProductId = 6, Quantity = 1 },
            new OrderProduct { OrderId = 4, ProductId = 7, Quantity = 5 }
        );

        // Configure relationships
        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => new { op.OrderId, op.ProductId });

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderProducts)
            .HasForeignKey(op => op.OrderId);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(op => op.ProductId);
    }
}

 