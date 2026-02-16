namespace Infrastructure.Configs;

public interface IMainConfiguration
{
    public string GeocodingApiKey { get; }
    public string OpenWeatherApiKey { get; }
    public string TelegramBotToken { get; }
    public string InternalApiKey { get; set; }
    public bool IsTelegramIntegrationEnabled { get; }
    public string TelegramBotApi { get;  }
    public string OpenWeatherUrl { get; }
}