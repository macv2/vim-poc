using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                            new WeatherForecast
                            (
                                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                Random.Shared.Next(-20, 55),
                                summaries[Random.Shared.Next(summaries.Length)]
                            ))
                    .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

app.MapGet("/products/GetProducts", async () =>
{
    var con = new SqlConnection(builder.Configuration.GetConnectionString("Default"));
    string sql = "SELECT * FROM Product";
    var result = (await con.QueryAsync<Product>(sql)).ToList();

    var response = new ProductListResponse();
    var productList = new List<Product>();
    foreach (var item in result)
    {
        productList.Add(item);
    }
    response.Products = productList;
    return response;
})
    .WithName("GetProducts")
    .WithOpenApi();

app.MapPost("/products/AddProduct", async (AddProductRequest request) =>
{
    var con = new SqlConnection(builder.Configuration.GetConnectionString("Default"));
    string sql = """
    INSERT INTO [dbo].[Product]
          ([Name]
          ,[Price]
          ,[Quantity])
    VALUES
          (@name
          ,@price
          ,@quantity)
    """;
    var result = await con.ExecuteAsync(sql, new {name = request.Name, price = request.Price, quantity = request .Quantity});

    var response = new AddProductResponse();
    response.Message = result > 0 ? "Insertado exitosamente" : "Ocurrió un error al guardar el producto";
    return response;
})
    .WithName("AddProduct")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class Product 
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class AddProductRequest
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class AddProductResponse
{
    public string Message { get; set; }
}

public class ProductListResponse
{
    public List<Product> Products { get; set; }
}
