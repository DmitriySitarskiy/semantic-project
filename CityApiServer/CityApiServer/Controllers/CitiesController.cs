using CityApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CityApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private static List<City> citiesCache = new List<City>();
        private static readonly HttpClient httpClient = new HttpClient();

        // Завантаження списку міст
        [HttpGet]
        public async Task<IActionResult> GetCities([FromQuery] string name = null)
        {
            if (!citiesCache.Any())
            {
                string sparqlQuery = @"
                PREFIX dbo: <http://dbpedia.org/ontology/>
                PREFIX dbr: <http://dbpedia.org/resource/>
                PREFIX foaf: <http://xmlns.com/foaf/0.1/>
                SELECT DISTINCT ?city ?name ?population WHERE {
                    ?city a dbo:City ;
                          dbo:country dbr:Ukraine ;
                          foaf:name ?name .
                    OPTIONAL { ?city dbo:populationTotal ?population . }
                    FILTER (BOUND(?population) && ?population > 100000)
                }
                ORDER BY DESC(?population)
                ";

                string endpoint = $"http://dbpedia.org/sparql?query={Uri.EscapeDataString(sparqlQuery)}&format=application/json";

                try
                {
                    var response = await httpClient.GetStringAsync(endpoint);
                    var results = JObject.Parse(response)["results"]["bindings"];

                    int id = 1;
                    foreach (var result in results)
                    {
                        citiesCache.Add(new City
                        {
                            Id = id++,
                            Name = result["name"]["value"].ToString(),
                            Uri = result["city"]["value"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "Помилка завантаження даних", details = ex.Message });
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                var filteredCities = citiesCache.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
                return Ok(filteredCities);
            }

            return Ok(citiesCache);
        }

        // Завантаження деталей міста
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityDetails(int id)
        {
            var city = citiesCache.FirstOrDefault(c => c.Id == id);
            if (city == null)
                return NotFound(new { error = "Місто не знайдено" });

            string descriptionQuery = $@"
            PREFIX dbo: <http://dbpedia.org/ontology/>
            SELECT ?description ?population ?image WHERE {{
                <{city.Uri}> dbo:abstract ?description . 
                OPTIONAL {{ <{city.Uri}> dbo:populationTotal ?population. }} 
                OPTIONAL {{ <{city.Uri}> dbo:thumbnail ?image. }} 
                FILTER (lang(?description) = 'uk')
            }}";

            string endpoint = $"http://dbpedia.org/sparql?query={Uri.EscapeDataString(descriptionQuery)}&format=application/json";

            try
            {
                var response = await httpClient.GetStringAsync(endpoint);
                var data = JObject.Parse(response)["results"]["bindings"].FirstOrDefault();

                if (data != null)
                {
                    city.Description = data["description"]["value"].ToString();
                    city.Population = data["population"]?["value"]?.ToObject<int?>();
                    city.ImageUrl = data["image"]?["value"]?.ToString();
                }

                return Ok(city);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Не вдалося отримати деталі міста", details = ex.Message });
            }
        }
    }
}
