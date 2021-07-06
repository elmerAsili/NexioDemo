using Newtonsoft.Json;

namespace Nexio.Models
{
    public class GatewayResponse
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("refNumber")]
        public string RefNumber { get; set; }

        [JsonProperty("gatewayName")]
        public string GatewayName { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}