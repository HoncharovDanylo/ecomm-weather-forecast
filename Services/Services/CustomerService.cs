using Core.Interfaces;
using Core.Models;
using DomainEntities.Entities;
using DomainEntities.Enums;
using DomainEntities.Requests;
using DomainEntities.Responses;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;
using WeatherForecast.Api.Exceptions;

namespace Services.Services;

public class CustomerService : ICustomerService
{
    private readonly IUserRepository _userRepository;
    private readonly ISelectedCityRepository _selectedCityRepository;
    private readonly IGeocodingService _geocodingService;
    private readonly IWeatherMapClient _weatherMapClient;

    public CustomerService(IUserRepository userRepository, ISelectedCityRepository selectedCityRepository,
        IGeocodingService geocodingService, IWeatherMapClient weatherMapClient)
    {
        _userRepository = userRepository;
        _selectedCityRepository = selectedCityRepository;
        _geocodingService = geocodingService;
        _weatherMapClient = weatherMapClient;
    }

    public async Task CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.ChatId == request.ChatId, cancellationToken);
        if (existingUser is null)
        {
            var user = new User()
            {
                Username = request.Name,
                ChatId = request.ChatId
            };

            await _userRepository.AddAsync(user, cancellationToken);
        }
    }

    public async Task<CityResponse> AddCity(long chatId, string requestName, CancellationToken cancellationToken)
    {
        var geocodedData = await _geocodingService.GetCoordinatesAsync(requestName);

        var user = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.ChatId == chatId, cancellationToken) ??
                   throw new BadRequestException($"Chat with id {chatId} not found");

        var selectedCity = new SelectedCity()
        {
            CityName = requestName,
            FormatedCityName = geocodedData.City,
            Latitude = geocodedData.Latitude,
            Longitude = geocodedData.Longitude,
            UserId = user.Id,
            Status = CityStatus.PendingForConfirmation
        };

        await _selectedCityRepository.AddAsync(selectedCity, cancellationToken);

        var response = new CityResponse()
        {
            Id = selectedCity.Id,
            Name = selectedCity.FormatedCityName,
            Latitude = selectedCity.Latitude,
            Longitude = selectedCity.Longitude
        };

        return response;
    }

    public async Task<List<CityResponse>> GetCities(long chatId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAll().Include(x => x.SelectedCities)
                       .FirstOrDefaultAsync(u => u.ChatId == chatId, cancellationToken) ??
                   throw new BadRequestException($"Chat with id {chatId} not found");

        var cities = user.SelectedCities.Where(c => c.Status == CityStatus.Confirmed).DistinctBy(x=>x.CityName).Select(c => new CityResponse()
        {
            Id = c.Id,
            Latitude = c.Latitude,
            Longitude = c.Longitude,
            Name = c.CityName,
            Status = c.Status
        }).ToList();

        return cities;
    }

    public async Task DeleteCity(Guid cityId, CancellationToken cancellationToken)
    {
        var city = await _selectedCityRepository.GetAll().FirstOrDefaultAsync(c => c.Id == cityId, cancellationToken);

        if (city == null)
        {
            throw new BadRequestException($"City with id {cityId} not found");
        }

        await _selectedCityRepository.DeleteAsync(city, cancellationToken);
    }

    public async Task UpdateCityStatus(UpdateCityStatusRequest request, CancellationToken cancellationToken)
    {
        var city = await _selectedCityRepository.GetAll().FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken) ??
                   throw new BadRequestException($"City with id {request.Id} not found");

        city.Status = request.Status;

        await _selectedCityRepository.UpdateAsync(city, cancellationToken);

    }

    public async Task<GetWeatherForUserExchange> GetWeather(long chatId, GetWeatherForUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAll().Include(x => x.SelectedCities)
                       .FirstOrDefaultAsync(u => u.ChatId == chatId, cancellationToken) ??
                   throw new BadRequestException($"Chat with id {chatId} not found");
        var city = user.SelectedCities.FirstOrDefault(c => c.Id == request.CityId) ??
                   throw new BadRequestException($"City with id {request.CityId} not found");

        var weatherRequest = new GetWeatherCoreModel()
        {
            Latitude = city.Latitude,
            Longitude = city.Longitude
        };
        
        var weather = await _weatherMapClient.GetCurrentWeatherAsync(weatherRequest, cancellationToken);

        var response = new GetWeatherForUserExchange()
        {
            CityName = city.CityName,
            Weather = new CurrentWeather()
            {
                Temperature = weather.Item2.Main.Temperature,
                Icon = weather.Item2.Weather[0].Icon,
                Main = weather.Item2.Weather[0].Main,
                Description = weather.Item2.Weather[0].Description,
                Date = DateTimeOffset.Now,
                MinTemperature = weather.Item2.Main.MinTemperature,
                MaxTemperature = weather.Item2.Main.MaxTemperature,
                WindSpeed = weather.Item2.Wind.Speed,
                Humidity = weather.Item2.Main.Humidity,
                Sunrise = TimeOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(weather.Item2.Sys.Sunrise).LocalDateTime),
                Sunset = TimeOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(weather.Item2.Sys.Sunset).LocalDateTime),
                Cloudiness = weather.Item2.Clouds.Cloudiness
            }
        };
        return response;
    }
}

