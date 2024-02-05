namespace Api.DockerDbClient.Features.Orders;

public class Order
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public string? Desciption { get; set; }
    public decimal Total { get; set; }
}