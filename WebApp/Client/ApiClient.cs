using System.Text;
using System.Text.Json;

namespace WebApp.Client;

// TODO: Parametro configuration is unread
public class ApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    public async Task<TResponse> Get<TResponse>(string url) where TResponse : ResponseBase, new()
    {
        var client = httpClientFactory.CreateClient();

        var httpResponse = await client.GetAsync(url);

        if (!httpResponse.IsSuccessStatusCode)
        {
            return new TResponse
            {
                IsSuccess = false,
            };
        }

        var apiResponse = await GetAPIResponseAsync<TResponse>(httpResponse);
        apiResponse.IsSuccess = true;

        return apiResponse;
    }

    public async Task<TResponse> Post<TRequest, TResponse>(string url, TRequest request) where TResponse : ResponseBase, new()
    {
        var client = httpClientFactory.CreateClient();

        var httpRequest = CreateHttpRequest(request, HttpMethod.Post, url, "application/json");

        var httpResponse = await client.SendAsync(httpRequest);

        if (!httpResponse.IsSuccessStatusCode)
        {
            return new TResponse
            {
                IsSuccess = false
            };
        }

        var apiResponse = await GetAPIResponseAsync<TResponse>(httpResponse);
        apiResponse.IsSuccess = true; 

        return apiResponse;
    }

    private static HttpRequestMessage CreateHttpRequest<T>(T request, HttpMethod method, string url, string mediaType)
    {
        var content = new StringContent(SerializeRequest(request), Encoding.UTF8, mediaType);

        return new HttpRequestMessage
        {
            Method = method,
            RequestUri = new Uri(url),
            Content = content
        };
    }

    private async Task<T> GetAPIResponseAsync<T>(HttpResponseMessage httpResponse)
    {
        var jsonContent = await httpResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonContent, JsonSerializerOptions)!;
    }

    private static string SerializeRequest<T>(T request) => JsonSerializer.Serialize(request);

    private static JsonSerializerOptions JsonSerializerOptions => new() { PropertyNameCaseInsensitive = true };
}

public class ResponseBase
{
    public bool IsSuccess { get; set; }
}
