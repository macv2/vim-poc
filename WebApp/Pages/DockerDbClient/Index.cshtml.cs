using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace WebApp.Pages.DockerDbClient;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<IndexModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public List<OrderModel> OrderCollection { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        try
        {
            OrderCollection = await GetOrderFormDockerDbApi() ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invoke Docker Db Client Page. Error.");
        }
        return Page();
    }

    private async Task<List<OrderModel>?> GetOrderFormDockerDbApi()
    {
        var url = _configuration.GetValue<string>("DockerDbClientApiUrl");
        var httpClient = _httpClientFactory.CreateClient();
        var httpResponse = await httpClient.GetAsync(url);

        var jsonContent = await httpResponse.Content.ReadAsStreamAsync();
        var response = await JsonSerializer.DeserializeAsync<List<OrderModel>>(jsonContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;
        return response;
    }

    public class OrderModel
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? Description { get; set; }
        public decimal Total { get; set; }
    }
}
