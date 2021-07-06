using Newtonsoft.Json;

namespace Nexio.Models
{
    public class TransactionInfo
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("rows")]
        public Transaction[] Rows { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }
    }

    public class Transaction
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }

        [JsonProperty("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonProperty("amount")]
        public double? Amount { get; set; }

        [JsonProperty("transactionStatus")]
        public int? TransactionStatus { get; set; }

        [JsonProperty("kount")]
        public Kount Kount { get; set; }

        [JsonProperty("plugin")]
        public Plugin Plugin { get; set; }
    }

    public class Plugin
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("originalId")]
        public string OriginalId { get; set; }

        [JsonProperty("invoice")]
        public string Invoice { get; set; }

        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("pluginType")]
        public int? PluginType { get; set; }

        [JsonProperty("transactionId")]
        public int? TransactionId { get; set; }
    }

    public class Kount
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
