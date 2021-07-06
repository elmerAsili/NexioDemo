using Newtonsoft.Json;

namespace Nexio.Models
{
    public class NexioCallback
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "merchantId")]
        public string MerchantId { get; set; }
        
        [JsonProperty(PropertyName = "transactionDate")]
        public string TransactionDate { get; set; }
        
        [JsonProperty(PropertyName = "transactionStatus")]
        public string TransactionStatus { get; set; }
        
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }
        
        [JsonProperty(PropertyName = "transactionType")]
        public string TransactionType { get; set; }
        
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        
        [JsonProperty(PropertyName = "gatewayResponse")]
        public GatewayResponse GatewayResponse { get; set; }
        
        [JsonProperty(PropertyName = "data")]
        public CallbackData Data { get; set; }

    }

    public class CallbackData
    {
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }
        
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        
        [JsonProperty(PropertyName = "settlementCurrency")]
        public string SettlementCurrency { get; set; }
        
        [JsonProperty(PropertyName = "customer")]
        public Customer Customer { get; set; }
    }
}
