using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);
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
