using Core.Models;
using Geocoding;
using Geocoding.Google;
using Infrastructure.Configs;
using Services.Interfaces;

namespace Services.Services;

public class GeocodingService : IGeocodingService
{
    public readonly IMainConfiguration _mainConfiguration;

    public GeocodingService(IMainConfiguration configuration)
    {
        _mainConfiguration = configuration;
    }

    public async Task<GeocoderResponseModel> GetCoordinatesAsync(string? requestCityName)
    {
        IGeocoder geocoder = new GoogleGeocoder(_mainConfiguration.GeocodingApiKey);
        
        var address = (await geocoder.GeocodeAsync(requestCityName)).Select(x=> new GeocoderResponseModel()
        {
            City = x.FormattedAddress,
            Longitude = x.Coordinates.Longitude,
            Latitude = x.Coordinates.Latitude
        }).First();

        return address;
    }

    public async Task<GeocoderResponseModel> GetCityAsync(double latitude, double longitude)
    {
        IGeocoder geocoder = new GoogleGeocoder(_mainConfiguration.GeocodingApiKey);
        
        var address = (await geocoder.ReverseGeocodeAsync(latitude, longitude)).Select(x=> new GeocoderResponseModel()
        {
            City = x.FormattedAddress,
            Longitude = x.Coordinates.Longitude,
            Latitude = x.Coordinates.Latitude
        }).First();

        return address;
    }
}