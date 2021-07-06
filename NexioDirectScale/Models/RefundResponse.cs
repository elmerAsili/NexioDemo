using Newtonsoft.Json;

namespace Nexio.Models
{
    public class RefundResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }
        
        [JsonProperty("transactionDate")]
        public string TransactionDate { get; set; }
        
        [JsonProperty("amount")]
        public double Amount { get; set; }
        
        [JsonProperty("transactionTypep")]
        public string TransactionType { get; set; }
        
        [JsonProperty("currency")]
        public string Currency { get; set; }
        
        [JsonProperty("gatewayResponse")]
        public GatewayResponse GatewayResponse { get; set; }

        [JsonProperty("data")]
        public RefundData Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
