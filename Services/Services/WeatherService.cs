using System.Diagnostics;
using System.Xml.XPath;
using Core.Interfaces;
using Core.Models;
using DomainEntities.Requests;
using DomainEntities.Responses;
using Services.Interfaces;
using WeatherForecast.Api.Exceptions;

namespace Services.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherMapClient _weatherMapClient;
    private readonly IGeocodingService _geocodingService; 

    public WeatherService(IWeatherMapClient weatherMapClient, IGeocodingService geocodingService)
    {
        _weatherMapClient = weatherMapClient;
        _geocodingService = geocodingService;
    }

    public async Task<GetForecastResponse> GetWeatherByCityAsync(string city, CancellationToken cancellationToken)
    {
        string.IsNullOrWhiteSpace(city).ThrowExceptionIfConditionTrue<BusinessLogicException>("City field cannot be empty");

        var geocodedData = await _geocodingService.GetCoordinatesAsync(city);

        var weatherForecast = await GetWeatherAsync(geocodedData, cancellationToken);
        return weatherForecast;
    }

    public async Task<GetForecastResponse> GetWeatherByCoordinatesAsync(GetWeatherRequest request,
        CancellationToken cancellationToken)
    {
        (!request.IsValid).ThrowExceptionIfConditionTrue<BusinessLogicException>("Invalid coordinates");

        var geocodedData = await _geocodingService.GetCityAsync(request.Latitude.Value, request.Longitude.Value);
        
        var weatherForecast = await GetWeatherAsync(geocodedData, cancellationToken);
        return weatherForecast;
    }

    #region Private methods
     private async Task<GetForecastResponse> GetWeatherAsync(GeocoderResponseModel request,
         CancellationToken cancellationToken)
    {
        var model = new GetWeatherCoreModel()
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };
        var response = await _weatherMapClient.GetForecastAsync(model, cancellationToken);

        var groupedForecast = response.List.GroupBy(x => x.Date.Date);

        var responseModel = new GetForecastResponse()
        {
            CityName = request.City,
            Weather = new WeatherModel()
            {
                Now = new CurrentWeather()
                {
                    Temperature = response.List[0].Main.Temperature,
                    Icon = response.List[0].Weather[0].Icon,
                    Main = response.List[0].Weather[0].Main,
                    Description = response.List[0].Weather[0].Description,
                    Date = DateTimeOffset.Now,
                    MinTemperature = response.List[0].Main.TempMin,
                    MaxTemperature = response.List[0].Main.TempMax,
                    WindSpeed = response.List[0].Wind.Speed,
                    Humidity = response.List[0].Main.Humidity,
                    Sunrise = TimeOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(response.City.Sunrise).LocalDateTime),
                    Sunset = TimeOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(response.City.Sunset).LocalDateTime),
                    Cloudiness = response.List[0].Clouds.All
                },
                Forecast = groupedForecast.Select(x => new DailyForecast()
                {
                    Date = x.Key,
                    MinTemperature = x.Min(y => y.Main.TempMin),
                    MaxTemperature = x.Max(y => y.Main.TempMax),
                    FeelsLike = x.Average(y=>y.Main.FeelsLike),
                    Icon = x.First().Weather[0].Icon,
                    Morning = GetWeatherForTime(x, DayTimeType.Morning),
                    Day = GetWeatherForTime(x, DayTimeType.Day),
                    Evening = GetWeatherForTime(x, DayTimeType.Evening),
                    Night = GetWeatherForTime(x, DayTimeType.Night),
                    Main = x.First().Weather[0].Main,
                    WindSpeed = x.First().Wind.Speed
                    
                }).ToList()
            }
        };
        return responseModel;
    }

    private HourlyForecast? GetWeatherForTime(IGrouping<DateTime, WeatherDetails> grouping, DayTimeType type)
    {
        var time = type switch
        {
            DayTimeType.Morning => grouping.Key.AddHours(6),
            DayTimeType.Day => grouping.Key.AddHours(12),
            DayTimeType.Evening => grouping.Key.AddHours(18),
            DayTimeType.Night => grouping.Key.AddHours(0),
            _ => grouping.Key
        };
        
        var weather = grouping.FirstOrDefault(x => x.Date >= time && x.Date < time.AddHours(6));

        return weather == null ? null : new HourlyForecast()
        {
            FeelsLike = weather.Main.FeelsLike,
            Humidity = weather.Main.Humidity,
            Icon = weather.Weather[0].Icon,
            Pop = weather.Pop,
            Pressure = weather.Main.Pressure,
            Temperature = weather.Main.Temperature,
            Time = TimeOnly.FromDateTime(weather.Date)
        };
    }
    #endregion
}