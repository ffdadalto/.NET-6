using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hellooww!");
app.MapPost("/user", () => new { name = "Franchescolle Dadalto", idade = 34 });

app.MapPost("/saveproduct", (Product product) =>
{
    ProductRepository.Add(product);
});

// Informações via query params
//api.app.com/getproduct?datastart={date}&dataend={date}
app.MapGet("/getproduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return dateStart + " - " + dateEnd;
});

// Informações via rota
//api.app.com/getproduct/{code}
app.MapGet("/getproduct/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.getBy(code);
    return product;
});

app.MapGet("/getallproducts/", () =>
{
    return ProductRepository.Products;    
});

app.MapPut("/editproduct", (Product product) =>
{
    var productSaved = ProductRepository.getBy(product.Code);
    productSaved.Name = product.Name;
});

app.MapDelete("/deleteproduct/{code}", ([FromRoute] string code) =>
{
    var productSaved = ProductRepository.getBy(code);
    ProductRepository.Remove(productSaved);
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
    public string Code { get; set; }
    public string Name { get; set; }
}