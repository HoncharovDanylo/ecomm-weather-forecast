using Newtonsoft.Json;

namespace DomainEntities.Requests;

public class CreateCustomerRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("chatId")]
    public long ChatId { get; set; }
}