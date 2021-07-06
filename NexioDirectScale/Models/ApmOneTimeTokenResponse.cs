using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nexio.Models
{
    public class ApmOneTimeTokenResponse
    {
        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; }
        
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        
        [JsonProperty(PropertyName = "expressIFrameUrl")]
        public string ExpressIFrameUrl { get; set; }
        
        [JsonProperty(PropertyName = "redirectUrls")]
        public List<RedirectUrl> RedirectUrls { get; set; }
        
        [JsonProperty(PropertyName = "buttonIFrameUrls")]
        public List<RedirectUrl> ButtonIFrameUrls { get; set; }
    }
}
