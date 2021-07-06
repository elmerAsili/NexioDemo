using Newtonsoft.Json;

namespace Nexio.Models
{
    public class RefundRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "data")]
        public RefundData Data { get; set; }
    }
}
