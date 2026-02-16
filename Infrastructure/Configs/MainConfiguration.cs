namespace Infrastructure.Configs;

public class MainConfiguration : IMainConfiguration
{
    public string GeocodingApiKey { get; set; }
    public string OpenWeatherApiKey { get; set; }
    
    public string InternalApiKey { get; set; }
    public string TelegramBotToken { get; set; }
    public bool IsTelegramIntegrationEnabled { get; set; }
    public string TelegramBotApi { get; set; }

    public string OpenWeatherUrl { get; set; }
}