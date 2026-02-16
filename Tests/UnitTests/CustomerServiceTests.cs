using Services.Interfaces;
using Moq;
using Services.Services;
using Core.Interfaces;
using Core.Models;
using DomainEntities.Entities;
using DomainEntities.Enums;
using DomainEntities.Requests;
using DomainEntities.Responses;
using Repositories.Interfaces;
using WeatherForecast.Api.Exceptions;
using MockQueryable;
using Newtonsoft.Json;

namespace Tests.UnitTests;

public class CustomerServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ISelectedCityRepository> _cityRepoMock;
    private readonly Mock<IGeocodingService> _geocodingServiceMock;
    private readonly Mock<IWeatherMapClient> _weatherClientMock;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _cityRepoMock = new Mock<ISelectedCityRepository>();
        _geocodingServiceMock = new Mock<IGeocodingService>();
        _weatherClientMock = new Mock<IWeatherMapClient>();

        _service = new CustomerService(
            _userRepoMock.Object,
            _cityRepoMock.Object,
            _geocodingServiceMock.Object,
            _weatherClientMock.Object
        );

    }

    [Fact]
    public async Task CreateCustomerAsync_ShouldNotAddUser_WhenUserAlreadyExists()
    {
        // Arrange
        var request = new CreateCustomerRequest { ChatId = 12345, Name = "John Doe" };
        var existingUser = new User { ChatId = 12345, Username = "John Doe" };
        var users = new List<User> { existingUser }.AsQueryable().BuildMock();

        _userRepoMock.Setup(r => r.GetAll()).Returns(users);

        // Act
        await _service.CreateCustomerAsync(request, CancellationToken.None);

        // Assert
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task AddCity_ShouldAddCity_WhenUserExists()
    {
        // Arrange
        long chatId = 12345;
        var user = new User { ChatId = chatId, SelectedCities = new List<SelectedCity>() };
        var users = new List<User> { user }.AsQueryable().BuildMock();

        _userRepoMock.Setup(r => r.GetAll()).Returns(users);
        _cityRepoMock.Setup(r => r.AddAsync(It.IsAny<SelectedCity>(), CancellationToken.None)).Returns(Task.CompletedTask);
        _geocodingServiceMock.Setup(r => r.GetCoordinatesAsync(It.IsAny<string>())).Returns(() =>
            Task.FromResult(new GeocoderResponseModel
            {
                City = "Kyiv",
                Latitude = 50.45,
                Longitude = 30.52
            }));

        // Act
        await _service.AddCity(chatId, "Kyiv", CancellationToken.None);

        // Assert
        _cityRepoMock.Verify(r => r.AddAsync(It.Is<SelectedCity>(c => c.CityName == "Kyiv" && c.UserId == user.Id), CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetCities_ShouldThrowBusinessLogicExeption_WhenUserNotExists()
    {
        // Arrange
        long chatId = 12345;

        var users = new List<User>().AsQueryable().BuildMock();

        _userRepoMock.Setup(r => r.GetAll()).Returns(users);

        // Act
        var result = _service.GetCities(chatId, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BadRequestException>(async () => await result);
    }

    [Fact]
    public async Task CreateCustomerAsync_ShouldAddUser_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new CreateCustomerRequest { ChatId = 12345, Name = "John Doe" };
        var users = new List<User>().AsQueryable().BuildMock();

        _userRepoMock.Setup(r => r.GetAll()).Returns(users);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>(), CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        await _service.CreateCustomerAsync(request, CancellationToken.None);

        // Assert
        _userRepoMock.Verify(
            r => r.AddAsync(It.Is<User>(u => u.ChatId == request.ChatId && u.Username == request.Name), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AddCity_ShouldThrowBadRequestException_WhenUserNotExists()
    {
        // Arrange
        long chatId = 12345;
        var users = new List<User>().AsQueryable().BuildMock();

        _userRepoMock.Setup(r => r.GetAll()).Returns(users);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.AddCity(chatId, "Kyiv", CancellationToken.None));
    }

    [Fact]
    public async Task GetCities_ShouldReturnCities_WhenUserExists()
    {
        // Arrange
        long chatId = 12345;
        var user = new User
        {
            ChatId = chatId,
            SelectedCities = new List<SelectedCity>
            {
                new SelectedCity { CityName = "Kyiv", Status = CityStatus.Confirmed }
            }
        };
        var users = new List<User> { user }.AsQueryable().BuildMock();

        _userRepoMock.Setup(r => r.GetAll()).Returns(users);

        // Act
        var result = await _service.GetCities(chatId, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Kyiv", result.First().Name);
    }

    [Fact]
    public async Task DeleteCity_ShouldDeleteCity_WhenCityExists()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var city = new SelectedCity { Id = cityId };
        var cities = new List<SelectedCity> { city }.AsQueryable().BuildMock();

        _cityRepoMock.Setup(r => r.GetAll()).Returns(cities);
        _cityRepoMock.Setup(r => r.DeleteAsync(It.IsAny<SelectedCity>(), CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteCity(cityId, CancellationToken.None);

        // Assert
        _cityRepoMock.Verify(r => r.DeleteAsync(It.Is<SelectedCity>(c => c.Id == cityId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteCity_ShouldThrowBadRequestException_WhenCityNotExists()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var cities = new List<SelectedCity>().AsQueryable().BuildMock();

        _cityRepoMock.Setup(r => r.GetAll()).Returns(cities);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.DeleteCity(cityId, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCityStatus_ShouldUpdateStatus_WhenCityExists()
    {
        // Arrange
        var request = new UpdateCityStatusRequest { Id = Guid.NewGuid(), Status = CityStatus.Confirmed };
        var city = new SelectedCity { Id = request.Id, Status = CityStatus.PendingForConfirmation };
        var cities = new List<SelectedCity> { city }.AsQueryable().BuildMock();

        _cityRepoMock.Setup(r => r.GetAll()).Returns(cities);
        _cityRepoMock.Setup(r => r.UpdateAsync(It.IsAny<SelectedCity>(), CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateCityStatus(request, CancellationToken.None);

        // Assert
        _cityRepoMock.Verify(
            r => r.UpdateAsync(It.Is<SelectedCity>(c => c.Id == request.Id && c.Status == request.Status), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateCityStatus_ShouldThrowBadRequestException_WhenCityNotExists()
    {
        // Arrange
        var request = new UpdateCityStatusRequest { Id = Guid.NewGuid(), Status = CityStatus.Confirmed };
        var cities = new List<SelectedCity>().AsQueryable().BuildMock();

        _cityRepoMock.Setup(r => r.GetAll()).Returns(cities);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateCityStatus(request, CancellationToken.None));
    }

    [Fact]
    public async Task GetWeather_ShouldReturnWeather_WhenUserAndCityExist()
    {
        // Arrange
        long chatId = 12345;
        var request = new GetWeatherForUserRequest { CityId = Guid.NewGuid() };
        var user = new User
        {
            ChatId = chatId,
            SelectedCities = new List<SelectedCity>
            {
                new SelectedCity { Id = request.CityId, CityName = "Kyiv", Latitude = 50.45, Longitude = 30.52 }
            }
        };
        var users = new List<User> { user }.AsQueryable().BuildMock();
        var weatherResponse =  JsonConvert.DeserializeObject<GetWeatherResponseModel>(Constants.CurrentWeather);
    
        _userRepoMock.Setup(r => r.GetAll()).Returns(users);
        _weatherClientMock.Setup(r => r.GetCurrentWeatherAsync(It.IsAny<GetWeatherCoreModel>(), CancellationToken.None))
            .ReturnsAsync((null, weatherResponse));
    
        // Act
        var result = await _service.GetWeather(chatId, request, CancellationToken.None);
    
        // Assert
        Assert.Equal("Kyiv", result.CityName);
        Assert.Equal(20, result.Weather.Temperature);
    }

    [Fact]
    public async Task GetWeather_ShouldThrowBadRequestException_WhenUserNotExists()
    {
        // Arrange
        long chatId = 12345;
        var request = new GetWeatherForUserRequest { CityId = Guid.NewGuid() };
        var users = new List<User>().AsQueryable().BuildMock();

        _userRepoMock.Setup(r => r.GetAll()).Returns(users);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.GetWeather(chatId, request, CancellationToken.None));
    }
}