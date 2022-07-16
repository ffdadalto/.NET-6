using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);
var app = builder.Build();

app.MapPost("/product", (ProductRequest productRequest, ApplicationDbContext context) =>
{
    var category = context.Category.Where(c => c.Id == productRequest.CategoryId).First();

    var product = new Product
    {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };

    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var tagName in productRequest.Tags)
            product.Tags.Add(new Tag { Name = tagName });
    }

    context.Products.Add(product);
    context.SaveChanges();

    return Results.Created($"product/{product.Id} ", product);
});

// Informações via query params
//api.app.com/getproduct?datastart={date}&dataend={date}
// app.MapGet("/getproduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
// {
//     return dateStart + " - " + dateEnd;
// });

// Informações via rota
//api.app.com/getproduct/{code}

app.MapGet("/product/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .Where(p => p.Id == id).First();

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
