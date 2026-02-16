using System.Text;
using Core.Interfaces;
using Core.Models;
using DomainEntities.Responses;
using Newtonsoft.Json;

namespace Core.Clients;

public class WeatherMapClient : IWeatherMapClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherMapClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(string, GetWeatherResponseModel?)> GetCurrentWeatherAsync(GetWeatherCoreModel model,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(HttpClientsNames.WeatherForecastApiClient);
        
        var response = await client.GetAsync(client.BaseAddress+"weather"+"?lat="+model.Latitude+"&lon="+model.Longitude+"&appid="+client.DefaultRequestHeaders.GetValues("X-Api-Key").FirstOrDefault()+"&units=metric", cancellationToken);
        
        return (model.CityName, JsonConvert.DeserializeObject<GetWeatherResponseModel>(await response.Content.ReadAsStringAsync(cancellationToken)));
    }

    public async Task<GetForecastResponseModel?> GetForecastAsync(GetWeatherCoreModel model,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(HttpClientsNames.WeatherForecastApiClient);
        
        var response = await client.GetAsync(client.BaseAddress+"forecast?lat="+model.Latitude+"&lon="+model.Longitude+"&appid="+client.DefaultRequestHeaders.GetValues("X-Api-Key").FirstOrDefault()+"&units=metric", cancellationToken);
        
        return JsonConvert.DeserializeObject<GetForecastResponseModel>(await response.Content.ReadAsStringAsync(cancellationToken));
    }
}