using Newtonsoft.Json;

namespace Nexio.Models
{
    public class Customer
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty(PropertyName = "shipToAddressOne")]
        public string ShipToAddressOne { get; set; }
        
        [JsonProperty(PropertyName = "shipToPhone")]
        public string ShipToPhone { get; set; }
        
        [JsonProperty(PropertyName = "shipToCountry")]
        public string ShipToCountry { get; set; }
        
        [JsonProperty(PropertyName = "shippToAddressTwo")]
        public string ShipToAddressTwo { get; set; }
        
        [JsonProperty(PropertyName = "billToState")]
        public string BillToState { get; set; }
        
        [JsonProperty(PropertyName = "billToCity")]
        public string BillToCity { get; set; }
        
        [JsonProperty(PropertyName = "shipToPostal")]
        public string ShipToPostal { get; set; }
        
        [JsonProperty(PropertyName = "shipToCity")]
        public string ShipToCity { get; set; }
        
        [JsonProperty(PropertyName = "billToAddressOne")]
        public string BillToAddressOne { get; set; }
        
        [JsonProperty(PropertyName = "billToCountry")]
        public string BillToCountry { get; set; }
        
        [JsonProperty(PropertyName = "billToPostal")]
        public string BillToPostal { get; set; }
        
        [JsonProperty(PropertyName = "billToAddressTwo")]
        public string BillToAddressTwo { get; set; }
        
        [JsonProperty(PropertyName = "billToPhone")]
        public string BillToPhone { get; set; }
        
        [JsonProperty(PropertyName = "shipToState")]
        public string ShipToState { get; set; }

        [JsonProperty(PropertyName = "nationalIdentificationNumber")]
        public string NationalIdentificationNumber { get; set; }
    }
}