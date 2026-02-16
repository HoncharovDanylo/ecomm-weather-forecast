using System.Text;
using Core.Interfaces;
using Core.Models;
using DomainEntities.Requests;
using DomainEntities.Responses;
using Infrastructure.Configs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Clients;

public class TelegramClient : ITelegramClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TelegramClient> _logger;
    private readonly IMainConfiguration _mainConfiguration;

    public TelegramClient(IHttpClientFactory httpClientFactory, ILogger<TelegramClient> logger, IMainConfiguration mainConfiguration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _mainConfiguration = mainConfiguration;
    }

    public async Task SendBroadcastAsync(BroadcastRequest broadcastRequest)
    {
        var client = _httpClientFactory.CreateClient(HttpClientsNames.TelegramBotApiClient);


        foreach (var message in broadcastRequest.Forecasts)
        {
            try
            { 
                var content = new StringContent(JsonConvert.SerializeObject(new TelegramMessageExchange() {ChatId = broadcastRequest.ChatId, Text = GetForecastText(message)}), Encoding.UTF8,
                    "application/json");
                await client.PostAsync(client.BaseAddress+_mainConfiguration.TelegramBotToken+"/sendMessage", content);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Error occured while sending broadcast");
            }

        }
        
    }

    private static string GetForecastText(GetWeatherForUserExchange request)
    {
        return  $"🌤 Weather in {request.CityName}:\n\n🗓 Date & Time: {request.Weather.Date}\n🌡 Temperature: {request.Weather.Temperature}°C\n↕ Min/Max Temp: {request.Weather.MinTemperature}°C / {request.Weather.MaxTemperature}°C\n🌬 Wind Speed: {request.Weather.WindSpeed} m/s\n💧 Humidity: {request.Weather.Humidity}%\n☁ Cloudiness: {request.Weather.Cloudiness}%\n🌅 Sunrise: {request.Weather.Sunrise}\n🌇 Sunset: {request.Weather.Sunset}\n🌟 Main: {request.Weather.Main}\n📝 Description: {request.Weather.Description}\n";
    }
}