using DomainEntities.Requests;
using DomainEntities.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Interfaces;

namespace WeatherForecast.Api.Controllers;

[Authorize(Policy = Constants.TelegramPolicy)]
[Route(Constants.ApiRoot)]
public class TelegramController : ControllerBase
{
    private readonly ILogger<TelegramController> _logger;
    private readonly ICustomerService _customerService;

    public TelegramController(ILogger<TelegramController> logger, ICustomerService customerService)
    {
        _logger = logger;
        _customerService = customerService;
    }
    
    [HttpPost("users/create")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"{nameof(CreateCustomer)} request {JsonConvert.SerializeObject(request)}");
        
        await _customerService.CreateCustomerAsync(request, cancellationToken);
        
        return Ok();
    }

    [HttpPost("users/{chatId}/add-city")]
    public async Task<IActionResult> AddCityForCustomer(long chatId, [FromBody] AddCityRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding city {0} for user : {1}", request.Name, chatId);
        
        CityResponse response = await _customerService.AddCity(chatId, request.Name, cancellationToken);
        
        return Ok(response);
    }
    
    [HttpPost("cities/delete/{cityId}")]
    public async Task<IActionResult> DeleteCity(Guid cityId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting city with id: {0}", cityId);
        
        await _customerService.DeleteCity(cityId, cancellationToken);
        
        return Ok();
    }
    
    [HttpGet("users/{chatId}/get-cities")]
    public async Task<IActionResult> GetCitiesForCustomer(long chatId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requested list of cities for user : {0}", chatId);
        
        List<CityResponse> cities = await _customerService.GetCities(chatId, cancellationToken);
        return Ok(cities);
    }
    
    [HttpPatch("cities/update-status")]
    public async Task<IActionResult> UpdateCityStatus([FromBody] UpdateCityStatusRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating city status for city with id: {0}", request.Id);
        
        await _customerService.UpdateCityStatus(request, cancellationToken);
        return Ok();
    }
    
    [HttpPost("users/{chatId}/weather")]
    public async Task<IActionResult> GetWeatrherForUser(long chatId, [FromBody] GetWeatherForUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requested weather for user : {0}", chatId);
        
        var cities = await _customerService.GetWeather(chatId, request, cancellationToken);
        return Ok(cities);
    }
}