using DomainEntities.Enums;
using DomainEntities.Requests;
using DomainEntities.Responses;
using Microsoft.AspNetCore.Mvc.Testing;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
namespace Tests.IntegrationTests;

public class CustomerControllerIntegrationTests : IClassFixture<WeatherForecastApplicationFactory>
{
    private readonly HttpClient _client;

    public CustomerControllerIntegrationTests(WeatherForecastApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Api-Key", "yOXP9L197QDKQZiMUioCfMSL71lJrYQz");
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnSuccess_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new CreateCustomerRequest { ChatId = 12345, Name = "John Doe" };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/users/create", content);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddCity_ShouldReturnSuccess_WhenUserExists()
    {
        // Arrange
        var chatId = 12345;
        var cityName = "Kyiv";
        var content = new StringContent(JsonConvert.SerializeObject(new { Name = cityName }), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/users/{chatId}/add-city", content);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetCities_ShouldReturnCities_WhenUserExists()
    {
        // Arrange
        var chatId = 12345;

        // Act
        var response = await _client.GetAsync($"/api/users/{chatId}/get-cities");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var cities = JsonConvert.DeserializeObject<List<CityResponse>>(responseString);
        Assert.NotEmpty(cities);
    }

    [Fact]
    public async Task DeleteCity_ShouldReturnSuccess_WhenCityExists()
    {
        // Arrange
        var cityId = "bf8f1289-2191-410f-bd53-346753b45f31";

        // Act
        var response = await _client.PostAsync($"/api/cities/delete/{cityId}", null);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateCityStatus_ShouldReturnSuccess_WhenCityExists()
    {
        // Arrange
        var request = new UpdateCityStatusRequest { Id = new Guid("ddd00185-e1bc-4ddb-bb68-e4f0e3210d88"), Status = CityStatus.Rejected };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync("/api/cities/update-status", content);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetWeather_ShouldReturnWeather_WhenUserAndCityExist()
    {
        // Arrange
        var chatId = 12345;
        var request = new GetWeatherForUserRequest { CityId = new Guid("ddd00185-e1bc-4ddb-bb68-e4f0e3210d88") };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/users/{chatId}/weather", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var weather = JsonConvert.DeserializeObject<GetWeatherForUserExchange>(responseString);
        Assert.NotNull(weather);
    }
}
