using Core.Interfaces;
using Core.Models;
using DomainEntities.Requests;
using DomainEntities.Responses;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;

namespace Hangfire.ScheduledJobs;

public class BroadcastHangfireTask
{
    private readonly ILogger<BroadcastHangfireTask> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IWeatherMapClient _weatherMapClient;
    private readonly ITelegramClient _telegramClient;
    
    public BroadcastHangfireTask(ILogger<BroadcastHangfireTask> logger, IUserRepository userRepository, IWeatherMapClient weatherMapClient, ITelegramClient telegramClient)
    {
        _logger = logger;
        _userRepository = userRepository;
        _weatherMapClient = weatherMapClient;
        _telegramClient = telegramClient;
    }
    
    public async Task ExecuteAsync(PerformContext context, IJobCancellationToken cancellationToken)
    {
       var jobId = context?.BackgroundJob?.Id ?? Guid.NewGuid().ToString();
        _logger.LogInformation($"({jobId}) {nameof(BroadcastHangfireTask)} job is started.");

        try
        {
            var users = await _userRepository.GetAll().Include(x=>x.SelectedCities).ToListAsync();

            foreach (var user in users)
            {
                var weathers = user.SelectedCities.DistinctBy(x=>x.CityName).Select(x =>
                    _weatherMapClient.GetCurrentWeatherAsync(new GetWeatherCoreModel()
                        { Latitude = x.Latitude, Longitude = x.Longitude, CityName = x.CityName}, cancellationToken.ShutdownToken));
                
                var result = await Task.WhenAll(weathers);

                var forecast = result.Select(x => new GetWeatherForUserExchange()
                {
                    CityName = x.Item1,
                    Weather = new CurrentWeather()
                    {
                        Temperature = x.Item2.Main.Temperature,
                        Icon = x.Item2.Weather[0].Icon,
                        Main = x.Item2.Weather[0].Main,
                        Description = x.Item2.Weather[0].Description,
                        Date = DateTimeOffset.Now,
                        MinTemperature = x.Item2.Main.MinTemperature,
                        MaxTemperature = x.Item2.Main.MaxTemperature,
                        WindSpeed = x.Item2.Wind.Speed,
                        Humidity = x.Item2.Main.Humidity,
                        Sunrise = TimeOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(x.Item2.Sys.Sunrise)
                            .LocalDateTime),
                        Sunset = TimeOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(x.Item2.Sys.Sunset)
                            .LocalDateTime),
                        Cloudiness = x.Item2.Clouds.Cloudiness
                    }
                });

                var broadcast = new BroadcastRequest()
                {
                    ChatId = user.ChatId,
                    Forecasts = forecast.ToList()
                };

                await _telegramClient.SendBroadcastAsync(broadcast);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"({jobId}): {nameof(BroadcastHangfireTask)} job is aborted with exception:\r\n");
            _logger.LogWarning(ex,
                $"{nameof(BroadcastHangfireTask)} Something went wrong. See inner exception");
        }
        finally
        {
            _logger.LogInformation($"({jobId}): {nameof(BroadcastHangfireTask)} job is finished.");
        }
    }
}