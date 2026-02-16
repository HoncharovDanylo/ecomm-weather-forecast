using Newtonsoft.Json;

namespace DomainEntities.Responses;

public class GetWeatherForUserExchange
{

    [JsonProperty("city_name")]
    public string CityName { get; set; }

    [JsonProperty("weather")] 
    public CurrentWeather Weather { get; set; }

}
