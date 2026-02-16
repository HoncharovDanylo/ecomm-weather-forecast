using Newtonsoft.Json;

namespace DomainEntities.Responses;

public class GetWeatherResponseModel
{
    [JsonProperty("coord")]
    public CoordinateBlock Coordinates { get; set; }
    
    [JsonProperty("weather")]
    public WeatherBlock[] Weather { get; set; }
    
    [JsonProperty("base")]
    public string Base { get; set; }
    
    [JsonProperty("main")]
    public MainBlock Main { get; set; }
    
    [JsonProperty("visibility")]
    public int Visibility { get; set; }
    
    [JsonProperty("wind")]
    public WindBlock Wind { get; set; }
    
    [JsonProperty("clouds")]
    public CloudsBlock Clouds { get; set; }
    
    [JsonProperty("rain")]
    public RainBlock Rain { get; set; }
    
    [JsonProperty("dt")]
    public long Date { get; set; }
    
    [JsonProperty("sys")]
    public SysBlock Sys { get; set; }
    
    [JsonProperty("timezone")]
    public int Timezone { get; set; }
    
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("cod")]
    public int Cod { get; set; }
}

public class CoordinateBlock
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }
    
    [JsonProperty("lon")]
    public double Longitude { get; set; }
}

public class WeatherBlock
{
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("main")]
    public string Main { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("icon")]
    public string Icon { get; set; }
}

public class MainBlock
{
    [JsonProperty("temp")]
    public double Temperature { get; set; }
    
    [JsonProperty("feels_like")]
    public double FeelsLike { get; set; }
    
    [JsonProperty("temp_min")]
    public double MinTemperature { get; set; }
    
    [JsonProperty("temp_max")]
    public double MaxTemperature { get; set; }
    
    [JsonProperty("pressure")]
    public int Pressure { get; set; }
    
    [JsonProperty("humidity")]
    public int Humidity { get; set; }
    
    [JsonProperty("sea_level")]
    public int SeaLevel { get; set; }
    
    [JsonProperty("grnd_level")]
    public int GroundLevel { get; set; }
}

public class WindBlock
{
    [JsonProperty("speed")]
    public double Speed { get; set; }
    
    [JsonProperty("deg")]
    public int Degree { get; set; }
    
    [JsonProperty("gust")]
    public double Gust { get; set; }
}

public class CloudsBlock
{
    [JsonProperty("all")]
    public int Cloudiness { get; set; }
}

public class RainBlock
{
    [JsonProperty("1h")]
    public double RainVolume { get; set; }
}

public class SysBlock
{
    [JsonProperty("type")]
    public int Type { get; set; }
    
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("country")]
    public string Country { get; set; }
    
    [JsonProperty("sunrise")]
    public long Sunrise { get; set; }
    
    [JsonProperty("sunset")]
    public long Sunset { get; set; }
}