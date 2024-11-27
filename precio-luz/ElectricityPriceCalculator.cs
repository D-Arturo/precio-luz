using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace precio_luz;

public class ElectricityPriceCalculator
{
    private readonly HttpClient _client;

    public ElectricityPriceCalculator( HttpClient client)
    {
        _client = client;
    }

    public async Task<List<HourlyInfo>> GetElectricityPriceAsync()
    {
        var url = "https://api.esios.ree.es/indicators/1001";
        
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Add("Accept", "application/vnd.esios-api-v1+json");

        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);
        
        var hourlyPrices = HourlyInfoFromJsonResponse(json);

        return hourlyPrices;
    }

    private List<HourlyInfo>? HourlyInfoFromJsonResponse(JObject json)
    {
        var hourlyPrices = HourlyPrices();
        
        if (!hourlyPrices.Any()) return new List<HourlyInfo> { new("No disponible", "No disponible") };

        List<HourlyInfo> hourlyInfo = new();

        foreach (var (dateTime, price) in hourlyPrices)
        {
            hourlyInfo.Add(new HourlyInfo(dateTime, price));
        }
        
        return hourlyInfo;
        
        
        List<(string DateTime, string? Price)>? HourlyPrices()
        {
            // Filtra los valores solo para "Península" y extrae la lista de precios y fechas
            return json["indicator"]?["values"]
                .Where(v => v["geo_name"]?.ToString() == "Península")
                .Select(v => (
                    DateTime: DateTime.Parse(v["datetime"]?.ToString()).ToString("HH:mm"),
                    Price: v["value"]?.ToString()
                ))
                .ToList();
        }
    }
}