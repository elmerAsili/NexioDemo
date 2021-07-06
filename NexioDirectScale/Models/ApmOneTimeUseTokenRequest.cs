using Newtonsoft.Json;

namespace Nexio.Models
{
    public class ApmOneTimeUseTokenRequest
    {
        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }

        [JsonProperty(PropertyName = "processingOptions")]
        public ProcessingOptions ProcessingOptions { get; set; }

        [JsonProperty(PropertyName = "uiOptions")]
        public UiOptions UiOptions { get; set; }

        [JsonProperty(PropertyName = "installment")]
        public Installment Installment { get; set; }
    }

    public class ProcessingOptions
    {
        [JsonProperty(PropertyName = "webhookUrl")]
        public string WebhookUrl { get; set; }
        
        [JsonProperty(PropertyName = "webhookFailUrl")]
        public string WebhookFailUrl { get; set; }
        
        [JsonProperty(PropertyName = "merchantId")]
        public string MerchantId { get; set; }
        
        [JsonProperty(PropertyName = "paymentOptionTag")]
        public string PaymentOptionTag { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }
        
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        
        [JsonProperty(PropertyName = "customer")]
        public Customer Customer { get; set; }
        
        [JsonProperty(PropertyName = "customerRedirectUrl")]
        public string CustomerRedirectUrl { get; set; }

        [JsonProperty(PropertyName = "descriptor")]
        public string Descriptor { get; set; }
    }

    public class UiOptions
    {
        [JsonProperty(PropertyName = "displaySubmitButton")]
        public bool DisplaySubmitButton { get; set; }
    }

    public class Installment
    {
        [JsonProperty(PropertyName = "period")]
        public int Period { get; set; }
    }
}
