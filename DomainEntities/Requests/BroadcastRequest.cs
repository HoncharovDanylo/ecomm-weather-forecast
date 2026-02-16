using DomainEntities.Responses;

namespace DomainEntities.Requests;

public class BroadcastRequest
{
    public long ChatId { get; set; }
    public List<GetWeatherForUserExchange> Forecasts { get; set; }
}