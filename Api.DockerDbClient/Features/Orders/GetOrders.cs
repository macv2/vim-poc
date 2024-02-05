using System.Data.SqlClient;
using Dapper;

namespace Api.DockerDbClient.Features.Orders;

public class GetOrders
{
    public class Handler(IConfiguration configuration)
    {
        public async Task<List<Order>> Handle()
        {
            await CreateDataBaseIfNotExistsAsync();
            var orders = await GetOrdersAsync();
            return orders;
        }

        private async Task<List<Order>> GetOrdersAsync()
        {
	        await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
	        const string query = "SELECT * FROM Orders";
	        var orders = (await connection.QueryAsync<Order>(query)).ToList();
	        return orders;
        }

        private async Task CreateDataBaseIfNotExistsAsync()
        {
            await using var createDatabaseConnection = new SqlConnection(configuration.GetConnectionString("MasterConnection"));
            const string createDatabaseQuery = """
                                  IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'DbInDockerForTest')
                                  begin
                                 	CREATE DATABASE [DbInDockerForTest]
                                  end
                                 """;
            
           await createDatabaseConnection.ExecuteAsync(createDatabaseQuery);
           
           await using var createTableConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            const string createTableQuery = """
                                             IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Orders')
                                             BEGIN
                                             	USE [DbInDockerForTest]
                                             	CREATE TABLE Orders(
                                             	[Id] INT PRIMARY KEY IDENTITY,
                                             	[CustomerName] VARCHAR(50),
                                             	[Description] VARCHAR(50),
                                             	Total DECIMAL);
                                             
                                             	INSERT INTO Orders (CustomerName, Description, Total)
                                             	VALUES
                                             		('Customer1', 'Description1', 100.00),
                                             		('Customer2', 'Description2', 150.50),
                                             		('Customer3', 'Description3', 200.25),
                                             		('Customer4', 'Description4', 75.80),
                                             		('Customer5', 'Description5', 120.00),
                                             		('Customer6', 'Description6', 90.75),
                                             		('Customer7', 'Description7', 180.50),
                                             		('Customer8', 'Description8', 50.25),
                                             		('Customer9', 'Description9', 300.00),
                                             		('Customer10', 'Description10', 110.75),
                                             		('Customer11', 'Description11', 75.00),
                                             		('Customer12', 'Description12', 160.25),
                                             		('Customer13', 'Description13', 90.50),
                                             		('Customer14', 'Description14', 200.75),
                                             		('Customer15', 'Description15', 120.25),
                                             		('Customer16', 'Description16', 85.00),
                                             		('Customer17', 'Description17', 250.50),
                                             		('Customer18', 'Description18', 95.75),
                                             		('Customer19', 'Description19', 180.00),
                                             		('Customer20', 'Description20', 110.25);

                                             END
                                             """;

            var result = await createTableConnection.ExecuteAsync(createTableQuery);
        }
    }
}