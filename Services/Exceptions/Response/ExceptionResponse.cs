
using Newtonsoft.Json;

namespace ClickCredit.Foundation.Exceptions.Models
{
    public class ExceptionResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
