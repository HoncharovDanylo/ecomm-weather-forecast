using System.Net;
using Newtonsoft.Json;

namespace Core.Models;

public class GetForecastResponseModel
{
    [JsonProperty("cod")]
    public HttpStatusCode StatusCode { get; set; }
    
    [JsonProperty("message")]
    public int Message { get; set; }
    
    [JsonProperty("cnt")]
    public int Cnt { get; set; }
    
    [JsonProperty("list")]
    public List<WeatherDetails> List { get; set; }
    
    [JsonProperty("city")]
    public CityBlock City { get; set; }
}

public class WeatherDetails
{
    [JsonProperty("dt")]
    public int DateInt { get; set; }
    
    [JsonProperty("main")]
    public WeatherMain Main { get; set; }
    
    [JsonProperty("weather")]
    public List<WeatherBlock> Weather { get; set; }
    
    [JsonProperty("clouds")]
    public CloudsBlock Clouds { get; set; }
    
    [JsonProperty("wind")]
    public WindBlock Wind { get; set; }
    
    [JsonProperty("sys")]
    public SysBlock Sys { get; set; }
    
    [JsonProperty("dt_txt")]
    public DateTime Date { get; set; }
    
    [JsonProperty("visibility")]
    public int Visibility { get; set; }
    
    [JsonProperty("pop")]
    public double Pop { get; set; }
    
}

public class WeatherMain
{
    [JsonProperty("temp")]
    public double Temperature { get; set; }
    
    [JsonProperty("feels_like")]
    public double FeelsLike { get; set; }
    
    [JsonProperty("temp_min")]
    public double TempMin { get; set; }
    
    [JsonProperty("temp_max")]
    public double TempMax { get; set; }
    
    [JsonProperty("pressure")]
    public int Pressure { get; set; }
    
    [JsonProperty("sea_level")]
    public int SeaLevel { get; set; }
    
    [JsonProperty("grnd_level")]
    public int GroundLevel { get; set; }
    
    [JsonProperty("humidity")]
    public int Humidity { get; set; }
    
    [JsonProperty("temp_kf")]
    public double TemperatureCoef { get; set; }
}

public class WeatherBlock
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("main")]
    public string Main { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("icon")]
    public string Icon { get; set; }
}

public class CloudsBlock
{
    [JsonProperty("all")]
    public int All { get; set; }
}

public class WindBlock
{
    [JsonProperty("speed")]
    public double Speed { get; set; }
    
    [JsonProperty("deg")]
    public int Deg { get; set; }
    
    [JsonProperty("gust")]
    public double Gust { get; set; }
}

public class SysBlock
{
    [JsonProperty("pod")]
    public string Pod { get; set; }
}

public class CityBlock
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("coord")]
    public CoordBlock Coordinates { get; set; }
    
    [JsonProperty("country")]
    public string Country { get; set; }
    
    [JsonProperty("population")]
    public int Population { get; set; }
    
    [JsonProperty("timezone")]
    public int Timezone { get; set; }
    
    [JsonProperty("sunrise")]
    public int Sunrise { get; set; }
    
    [JsonProperty("sunset")]
    public int Sunset { get; set; }
}

public class CoordBlock
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }
    
    [JsonProperty("lon")]
    public double Longitude { get; set; }
}