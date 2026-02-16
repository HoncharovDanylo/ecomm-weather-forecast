using DomainEntities.Enums;
using Newtonsoft.Json;

namespace DomainEntities.Responses;

public class CityResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("id")]
    public Guid Id { get; set; }
    
    [JsonProperty("ltd")]
    public double Latitude { get; set; }
    
    [JsonProperty("lng")]
    public double Longitude { get; set; }
    
    [JsonProperty("status")]
    public CityStatus Status { get; set; }
}