using Core.Interfaces;
using DomainEntities.Requests;
using DomainEntities.Responses;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WeatherForecast.Api.Controllers;

public class PublicController : ControllerBase
{
    private readonly ILogger<PublicController> _logger;
    private readonly IWeatherService _weatherService;
    private readonly ITelegramClient _telegramClient;


    public PublicController(ILogger<PublicController> logger, IWeatherService weatherService, ITelegramClient telegramClient)
    {
        _logger = logger;
        _weatherService = weatherService;
        _telegramClient = telegramClient;
    }
    

    [HttpPost("weather")]
    public async Task<IActionResult> GetWeatherByCoordinates([FromBody] GetWeatherRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requested weather for coordinates: {0}, {1}", request.Latitude, request.Longitude);
        GetForecastResponse weather;
        if (string.IsNullOrWhiteSpace(request.City))
        {
            weather = await _weatherService.GetWeatherByCoordinatesAsync(request, cancellationToken);
        }
        else
        {
            weather = await _weatherService.GetWeatherByCityAsync(request.City, cancellationToken);
        }
        return Ok(weather);
    }

    [HttpGet("test")]
    public async Task<IActionResult> Teat()
    {
        var model = new BroadcastRequest()
        {
            ChatId = 895238316,
            Forecasts = new List<GetWeatherForUserExchange>()
            {
                new GetWeatherForUserExchange()
            }
        };
        
        await _telegramClient.SendBroadcastAsync(model);
        return Ok();
    }
    
}