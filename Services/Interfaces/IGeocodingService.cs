using Core.Models;

namespace Services.Interfaces;

public interface IGeocodingService
{
    Task<GeocoderResponseModel> GetCoordinatesAsync(string? requestCityName);
    Task<GeocoderResponseModel> GetCityAsync(double latitude, double longitude);
    
}