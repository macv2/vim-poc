using Api.DockerDbClient.Features.Orders;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .Build();

Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .WriteTo.Console()
        .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

Log.Information("API Docker Db Client. Starting up");

try
{
    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.RequestBodyLogLimit = 4096;
        logging.ResponseBodyLogLimit = 4096;
    });

    builder.Services.AddTransient<GetOrders.Handler>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();

    var app = builder.Build();

    app.UseHttpLogging();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.MapGet("/getOrders", (GetOrders.Handler handler) => handler.Handle())
            .WithName("GetOrders")
            .WithOpenApi();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "API Docker Db Client. Host terminated unexpectedly");
}
finally
{
    Log.Information("API Docker Db Client. Shut down complete");
    Log.CloseAndFlush();
}
