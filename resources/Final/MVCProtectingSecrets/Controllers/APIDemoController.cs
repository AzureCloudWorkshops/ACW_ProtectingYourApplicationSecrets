using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using MVCProtectingSecrets.Models;
using System.Text.Json;

namespace MVCProtectingSecrets.Controllers
{
    public class APIDemoController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly TelemetryClient _telemetryClient;

        public APIDemoController(ILogger<HomeController> logger, TelemetryClient telemetryClient
                            , IConfiguration configuration)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            _telemetryClient.TrackPageView("APIDemo/Index");
            var apiEndpoint = _configuration["VehicleAPI:BaseURL"];
            var getModelsForMake = _configuration["VehicleAPI:GetModelsForMake"];

            var apiEndpointWithMake = $"{apiEndpoint}{getModelsForMake}ford?format=json";

            var response = await _httpClient.GetAsync(apiEndpointWithMake);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                
                var vehicles = JsonSerializer.Deserialize<AllVehicles>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(vehicles.Results);
            }

            // Handle error response
            ViewBag.Error = "Error fetching data.";
            return View(new List<Vehicle>());
        }
    }
}
