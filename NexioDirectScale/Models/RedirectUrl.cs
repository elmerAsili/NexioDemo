using Newtonsoft.Json;

namespace Nexio.Models
{
    public class RedirectUrl
    {
        [JsonProperty(PropertyName = "paymentMethod")]
        public string PaymentMethod { get; set; }
        
        [JsonProperty(PropertyName = "Url")]
        public string Url { get; set; }
    }
}