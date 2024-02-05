using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using static WebApp.Pages.LocalDbClient.LocalDbPageModel;
using System.Net.Http;
using WebApp.Client;
using WebApp.Pages.LocalDbClient;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WebApp.Pages.RemoteClient;

public class IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration) : PageModel
{
    public List<PokemonModel> PokemonList { get; set; } = new();
    [BindProperty]
    public int Limit { get; set; }

    public async Task<IActionResult> OnGet()
    {
        try
        {
            var client = new ApiClient(httpClientFactory, configuration);

            var response = await client.Get<PokeResult>($"{configuration.GetValue<string>("RemoteApiUrl")}get?limit={Limit}");

            if (response.IsSuccess)
            {
                foreach (var item in response.Results)
                {
                    PokemonList.Add(new PokemonModel {Id = item.Id, Name = item.Name, ImageUrl = item.UrlImage });
                }
            }
        }
        catch (Exception)
        {

        }
        return Page();
    }

    public class PokeResult : ResponseBase
    {
        public int Count { get; set; }
        public string? Next { get; set; }
        public string? Previous { get; set; }
        public PokeModel[] Results { get; set; }
    }

    public class PokeModel
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? UrlImage { get; set; }
    }

    public class PokemonModel
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
    }
}
