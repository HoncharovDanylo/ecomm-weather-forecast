using Core.Models;
using DomainEntities.Responses;

namespace Core.Interfaces;

public interface IWeatherMapClient
{
    public Task<(string, GetWeatherResponseModel?)> GetCurrentWeatherAsync(GetWeatherCoreModel model,
        CancellationToken cancellationToken);
    public Task<GetForecastResponseModel?> GetForecastAsync(GetWeatherCoreModel model,
        CancellationToken cancellationToken);
}