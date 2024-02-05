using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.RemoteClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokeController : ControllerBase
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public PokeController(IHttpClientFactory httpClientFactory)
        {
            _baseUrl = "https://pokeapi.co/api/v2/";
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Obtiene una lista de Pokémon.
        /// </summary>
        /// <param name="limit">Número máximo de Pokémon a recuperar.</param>
        /// <returns>Lista de Pokémon.</returns>
        [HttpGet("get")]
        [ProducesResponseType(typeof(PokeApiResponse), 200)]
        [ProducesResponseType(400)] 

        public async Task<IActionResult> Get(int limit = 150)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}pokemon?limit={limit}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<PokeApiResponse>();

                return Ok(content);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error al obtener la lista de Pokémon.");
            }
        }
    }

    record PokeApiResponse
    {
        public int Count { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public List<PokeModel> Results { get; set; }
    }

    record PokeModel
    {
        public string Id => Url.Replace("https://pokeapi.co/api/v2/pokemon/", "").Replace("/", "");
        public string Name { get; set; }
        public string Url{ get; set; }
        public string UrlImage => $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/dream-world/{Id}.svg";
    }
}
