using CornerStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core and provides dummy value for testing
builder.Services.AddNpgsql<CornerStoreDbContext>(builder.Configuration["CornerStoreDbConnectionString"] ?? "testing");

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//endpoints go here

app.MapGet("/api/cashiers", async (CornerStoreDbContext db) =>
{
    var cashiersWithOrders = await db.Cashiers
        .Include(c => c.Orders)
        .ToListAsync();

    var cashierDTOs = cashiersWithOrders.Select(c => new CashierDTO
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Orders = c.Orders.Select(o => new OrderDTO
        {
            Id = o.Id,
            PaidOnDate = o.PaidOnDate,
        }).ToList()
    }).ToList();

    return Results.Ok(cashierDTOs);
});

app.MapGet("/api/cashiers/{id}", (int id, CornerStoreDbContext db) => 
{
    Cashier foundCashier = db.Cashiers
        .Include(c => c.Orders)
            .ThenInclude(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
        .SingleOrDefault(c => c.Id == id);
    
    if (foundCashier == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(foundCashier);
});

app.MapPost("/api/cashiers", (CornerStoreDbContext db, Cashier cashier) => {
    db.Cashiers.Add(cashier);
    db.SaveChanges();
    return Results.Created($"/api/cashiers/{cashier.Id}", cashier);
});

app.MapGet("/api/products", async (CornerStoreDbContext db, string? search) =>
{
    // Prepare the base query
    var query = db.Products.Include(p => p.Category).AsQueryable();

    // If a search query is provided, apply the filter
    if (!string.IsNullOrWhiteSpace(search))
    {
        search = search.ToLower();
        query = query.Where(p => p.ProductName.ToLower().Contains(search) || p.Category.CategoryName.ToLower().Contains(search));
    }

    // Execute the query and project the results to DTOs
    var products = await query
        .Select(p => new ProductDTO
        {
            Id = p.Id,
            ProductName = p.ProductName,
            Price = p.Price,
            Brand = p.Brand,
            Category = new CategoryDTO
            {
                Id = p.Category.Id,
                CategoryName = p.Category.CategoryName
            }
        })
        .ToListAsync();

    return Results.Ok(products);
});

app.MapPost("/api/products", (CornerStoreDbContext db, Product product) => 
{
    db.Products.Add(product);
    db.SaveChanges();
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapPut("/api/products/{id}", (int id, CornerStoreDbContext db, Product product) => 
{
    if (product.Id != id) 
    {
        return Results.BadRequest();
    }
    Product foundProduct = db.Products.SingleOrDefault(p => p.Id == id);
    if (product == null) 
    {
        return Results.BadRequest();    
    }
    foundProduct.ProductName = product.ProductName;
    foundProduct.Price = product.Price;
    foundProduct.Brand = product.Brand;
    foundProduct.CategoryId = product.CategoryId;
    foundProduct.Category = product.Category;
    foundProduct.OrderProducts = product.OrderProducts;
    db.SaveChanges();
    return Results.NoContent();
}); 

app.MapGet("/api/orders/{id}", (CornerStoreDbContext db, int id) => 
{
    Order foundOrder = db.Orders
    .Include(o => o.Cashier)
    .Include(o => o.OrderProducts)
        .ThenInclude(op => op.Product)
            .ThenInclude(p => p.Category)
    .SingleOrDefault(o => o.Id == id);
    if (foundOrder == null) 
    {
        return Results.BadRequest();
    }
    return Results.Ok(new OrderDTO
    {
        Id = foundOrder.Id, 
        Cashier = new CashierDTO
        {
            Id = foundOrder.Cashier.Id, 
            FirstName = foundOrder.Cashier.FirstName, 
            LastName = foundOrder.Cashier.LastName
        }, 
        CashierId = foundOrder.CashierId, 
        OrderProducts = foundOrder.OrderProducts.Select(op => new OrderProductDTO
        {
            OrderId = op.OrderId,
            Product = new ProductDTO
            {
                ProductName = op.Product.ProductName, 
                Brand = op.Product.Brand,
                Category = new CategoryDTO
                {
                    Id = op.Product.Category.Id, 
                    CategoryName = op.Product.Category.CategoryName
                },
                CategoryId = op.Product.CategoryId, 
                Id = op.Product.Id, 
                Price = op.Product.Price
            }
        }).ToList(),
        PaidOnDate = foundOrder.PaidOnDate
    });

});

app.MapGet("/api/orders", async (CornerStoreDbContext db, string? orderDate) => 
{
        var query = db.Orders.AsQueryable();
    DateTime? parsedOrderDate = null;
    if (!string.IsNullOrEmpty(orderDate) && DateTime.TryParse(orderDate, out DateTime parsedDate))
    {
        parsedOrderDate = parsedDate;
    }
    // If an order date query is provided, apply the filter
    if (parsedOrderDate.HasValue)
    {
        query = query.Where(o => o.PaidOnDate.HasValue && o.PaidOnDate.Value.Date == parsedOrderDate.Value.Date);
    }

    // Execute the query and project the results to DTOs
    var orders = await query
        .Select(o => new OrderDTO
        {
            Id = o.Id,
            PaidOnDate = o.PaidOnDate
        })
        .ToListAsync();

    return Results.Ok(orders);
});
app.MapDelete("/api/orders/{id}", (int id, CornerStoreDbContext db) => 
{
    Order foundOrder = db.Orders.SingleOrDefault(o => o.Id == id);
    db.Orders.Remove(foundOrder);
    db.SaveChanges();
    return Results.Ok("Item removed.");
});
app.Run();
//don't move or change this!
public partial class Program { }    