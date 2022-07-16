using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();
var app = builder.Build();

app.MapGet("/", () => "Hellooww!");
app.MapPost("/user", () => new { name = "Franchescolle Dadalto", idade = 34 });

app.MapPost("/product", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product);
});

// Informações via query params
//api.app.com/getproduct?datastart={date}&dataend={date}
// app.MapGet("/getproduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
// {
//     return dateStart + " - " + dateEnd;
// });

// Informações via rota
//api.app.com/getproduct/{code}
app.MapGet("/product/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.getBy(code);
    if (product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

app.MapGet("/getallproducts/", () =>
{
    return Results.Ok(ProductRepository.Products);
});

app.MapPut("/product", (Product product) =>
{
    var productSaved = ProductRepository.getBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok();
});

app.MapDelete("/product/{code}", ([FromRoute] string code) =>
{
    var productSaved = ProductRepository.getBy(code);
    ProductRepository.Remove(productSaved);
    return Results.Ok();
});

app.Run();

public static class ProductRepository
{
    public static List<Product> Products { get; set; }

    public static void Add(Product product)
    {
        if (Products == null)
            Products = new List<Product>();

        Products.Add(product);
    }

    public static Product getBy(string code)
    {
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer("Server=localhost,1433;Database=Products;User Id=sa;Password=YKcj10bc**;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES");
}