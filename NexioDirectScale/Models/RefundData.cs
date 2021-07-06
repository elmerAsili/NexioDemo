using Newtonsoft.Json;

namespace Nexio.Models
{
    public class RefundData
    {
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "settlementCurrency")]
        public string SettlementCurrency { get; set; }
    }
}
