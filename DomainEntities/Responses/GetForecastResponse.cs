using Newtonsoft.Json;

namespace DomainEntities.Responses;


public class GetForecastResponse
{
    [JsonProperty("city_name")]
    public string CityName { get; set; }

    [JsonProperty("weather")]
    public WeatherModel Weather { get; set; }
}

public class WeatherModel
{
    [JsonProperty("now")]
    public CurrentWeather Now { get; set; }
    
    [JsonProperty("forecast")]
    public List<DailyForecast> Forecast { get; set; }
}

public class CurrentWeather
{
    [JsonProperty("date_time")]
    public DateTimeOffset Date { get; set; }
    
    [JsonProperty("temp")]
    public double Temperature { get; set; }
    
    [JsonProperty("temp_min")]
    public double MinTemperature { get; set; }
    
    [JsonProperty("temp_max")]
    public double MaxTemperature { get; set; }
    
    [JsonProperty("wind_speed")]
    public double WindSpeed { get; set; }
    
    [JsonProperty("humidity")]
    public int Humidity { get; set; }
    
    [JsonProperty("sunrise")]
    public TimeOnly Sunrise { get; set; }
    
    [JsonProperty("sunset")]
    public TimeOnly Sunset { get; set; }
    
    [JsonProperty("cloudiness")]
    public int Cloudiness { get; set; }
    
    [JsonProperty("icon")]
    public string Icon { get; set; }
    
    [JsonProperty("main")]
    public string Main { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
}

public class DailyForecast
{
    [JsonProperty("date_time")]
    public DateTimeOffset Date { get; set; }
    
    [JsonProperty("temp_min")]
    public double MinTemperature { get; set; }
    
    [JsonProperty("temp_max")]
    public double MaxTemperature { get; set; }
    
    [JsonProperty("feels_like")]
    public double FeelsLike { get; set; }
    
    [JsonProperty("icon")]
    public string Icon { get; set; }
    
    [JsonProperty("morning")]
    public HourlyForecast? Morning { get; set; }
    
    [JsonProperty("day")]
    public HourlyForecast? Day { get; set; }
    
    [JsonProperty("evening")]
    public HourlyForecast? Evening { get; set; }
    
    [JsonProperty("night")]
    public HourlyForecast? Night { get; set; }
    
    [JsonProperty("main")]
    public string Main { get; set; }
    
    [JsonProperty("wind_speed")]
    public double WindSpeed { get; set; }
}

public class HourlyForecast
{
    [JsonProperty("time")]
    public TimeOnly Time { get; set; }
    
    [JsonProperty("temp")]
    public double Temperature { get; set; }
 
    [JsonProperty("feels_like")]
    public double FeelsLike { get; set; }
    
    [JsonProperty("humidity")]
    public int Humidity { get; set; }
    
    [JsonProperty("pop")]
    public double Pop { get; set; }
    
    [JsonProperty("pressure")]
    public int Pressure { get; set; }
    
    [JsonProperty("icon")]
    public string Icon { get; set; }

}