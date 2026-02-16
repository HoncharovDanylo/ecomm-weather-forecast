using Newtonsoft.Json;

namespace DomainEntities.Requests;

public class GetWeatherRequest
{
    [JsonProperty("city")]
    public string? City { get; set; }
    [JsonProperty("ltd")]
    public double? Latitude { get; set; }
    [JsonProperty("lng")]
    public double? Longitude { get; set; }

    [JsonIgnore]
    public bool IsValid => !string.IsNullOrWhiteSpace(City) || (Latitude is >= -90 and <= 90 && Longitude is >= -180 and <= 180);

}