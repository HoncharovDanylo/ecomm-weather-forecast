using DomainEntities.Requests;
using DomainEntities.Responses;

namespace Services.Interfaces;

public interface IWeatherService
{
    Task<GetForecastResponse> GetWeatherByCityAsync(string city, CancellationToken cancellationToken);
    Task<GetForecastResponse> GetWeatherByCoordinatesAsync(GetWeatherRequest request,
        CancellationToken cancellationToken);
}