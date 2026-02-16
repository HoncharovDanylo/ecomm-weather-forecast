using DomainEntities.Requests;
using DomainEntities.Responses;

namespace Services.Interfaces;

public interface ICustomerService
{
    Task CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken);
    Task<CityResponse> AddCity(long chatId, string requestName, CancellationToken cancellationToken);
    Task<List<CityResponse>> GetCities(long chatId, CancellationToken cancellationToken);
    Task DeleteCity(Guid cityId, CancellationToken cancellationToken);
    Task UpdateCityStatus(UpdateCityStatusRequest request, CancellationToken cancellationToken);
    Task<GetWeatherForUserExchange> GetWeather(long chatId, GetWeatherForUserRequest request,
        CancellationToken cancellationToken);
}