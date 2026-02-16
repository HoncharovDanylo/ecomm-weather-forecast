using DomainEntities.Enums;
using Newtonsoft.Json;

namespace Core.Models;

public class GetWeatherCoreModel
{
    [JsonProperty("ltd")]
    public double Latitude { get; set; }
    [JsonProperty("lng")]
    public double Longitude { get; set; }
    
    public string CityName { get; set; }
}