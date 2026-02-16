using Newtonsoft.Json;

namespace Core.Models;

public class TelegramMessageExchange
{
    [JsonProperty("chat_id")]
    
    public long ChatId { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }
}