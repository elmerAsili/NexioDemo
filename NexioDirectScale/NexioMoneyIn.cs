using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using Nexio.Models;
using System;
using System.Linq;

namespace Nexio
{
    public class NexioMoneyIn : RedirectMoneyInMerchant
    {
        private readonly IAssociateService _associateService;
        private readonly ILoggingService _loggingService;
        private readonly INexioService _nexioService;
        private readonly IOrderService _orderService;
        private readonly ISettingsService _settingsService;
        private readonly MerchantInfo _merchantInfo;

        public NexioMoneyIn(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService, MerchantInfo merchantInfo) : base(merchantInfo)
        {
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _nexioService = nexioService ?? throw new ArgumentNullException(nameof(nexioService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _merchantInfo = merchantInfo;
        }

        public override PaymentResponse ChargePayment(int associateId, int orderNumber, Address billingAddress, double amount, string currencyCode, string redirectUrl)
        {
            var associateInfo = _associateService.GetAssociate(associateId);
            var orderInfo = _orderService.GetOrders(new int[] { orderNumber })[0];
            var environment = _settingsService.ExtensionContext().EnvironmentType == EnvironmentType.Live ? string.Empty : "stage";
            var rootUrl = $"https://{_settingsService.ExtensionContext().ClientId}.corpadmin.directscale{environment}.com";

            if (associateInfo == null)
            {
                var ex = new ArgumentNullException($"Associate {associateId} could not be found. Order aborted.");
                _loggingService.LogError(ex, $"Associate {associateId} could not be found. Order aborted.");

                throw ex;
            }

            if (orderInfo == null)
            {
                var ex = new ArgumentNullException($"Order {orderNumber} could not be found. Order aborted.");
                _loggingService.LogError(ex, $"Order {orderNumber} could not be found. Order aborted.");

                throw ex;
            }

            var directScaleResponse = new PaymentResponse
            {
                Status = PaymentStatus.Pending,
                Response = "Pending",
                ResponseId = "0",
                TransactionNumber = Guid.NewGuid().ToString(),
                Amount = amount,
                OrderNumber = orderNumber,
                PaymentType = "Charge",
                Currency = currencyCode.ToUpper(),
                Merchant = MerchantInfo.Id,
                Redirect = true
            };

            var request = new ApmOneTimeUseTokenRequest
            {
                Data = new Data
                {
                    Amount = amount,
                    Currency = currencyCode.ToUpper(),
                    Customer = new Customer
                    {
                        FirstName = associateInfo.DisplayFirstName,
                        LastName = associateInfo.DisplayLastName,
                        Email = associateInfo.EmailAddress,
                        OrderNumber = orderNumber.ToString(),
                        ShipToAddressOne = orderInfo.Packages?.FirstOrDefault()?.ShippingAddress?.AddressLine1?? associateInfo.Address.AddressLine1,
                        ShipToAddressTwo = orderInfo.Packages?.FirstOrDefault()?.ShippingAddress?.AddressLine2 ?? associateInfo.Address.AddressLine2,
                        ShipToCity = orderInfo.Packages?.FirstOrDefault()?.ShippingAddress?.City ?? associateInfo.Address.City,
                        ShipToState = orderInfo.Packages?.FirstOrDefault()?.ShippingAddress?.State?.ToUpper() ?? associateInfo.Address.State?.ToUpper(),
                        ShipToPostal = orderInfo.Packages?.FirstOrDefault()?.ShippingAddress?.PostalCode ?? associateInfo.Address.PostalCode,
                        ShipToCountry = orderInfo.Packages?.FirstOrDefault()?.ShippingAddress?.CountryCode?.ToUpper() ?? associateInfo.Address.CountryCode?.ToUpper(),
                        BillToAddressOne = associateInfo.Address.AddressLine1,
                        BillToAddressTwo = associateInfo.Address.AddressLine2,
                        BillToCity = associateInfo.Address.City,
                        BillToState = associateInfo.Address.State?.ToUpper(),
                        BillToPostal = associateInfo.Address.PostalCode,
                        BillToCountry = associateInfo.Address.CountryCode?.ToUpper(),
                        NationalIdentificationNumber = associateInfo.TaxId
                    },
                    Descriptor = ""
                },
                ProcessingOptions = new ProcessingOptions
                {
                    WebhookUrl = $"{rootUrl}/Command/ClientAPI/Nexio/Callback",
                    WebhookFailUrl = $"{rootUrl}/Command/ClientAPI/Nexio/Callback"
                },
                UiOptions = new UiOptions
                {
                    DisplaySubmitButton = true
                }
            };

            var nexioResponse = _nexioService.GetApmOneTimeUseToken(request);

            directScaleResponse.RedirectURL = $"{rootUrl}/Corporate/Other?l=0&id=Merchants/NexioApmIframe&nexioRedirectUrl={nexioResponse.ExpressIFrameUrl}&returnUrl={redirectUrl}";

            return directScaleResponse;
        }

        public override PaymentResponse RefundPayment(string payerId, int orderNumber, string currencyCode, double paymentAmount, double refundAmount, string referenceNumber, string transactionNumber, string authorizationCode)
        {
            var refundRequest = new RefundRequest
            {
                Id = transactionNumber,
                Data = new RefundData
                {
                    Amount = paymentAmount
                }
            };

            var nexioResponse = _nexioService.RefundPayment(refundRequest, paymentAmount);

            var res = new PaymentResponse
            {
                Amount = refundAmount,
                AuthorizationCode = authorizationCode,
                Currency = currencyCode.ToUpper(),
                Merchant = _merchantInfo.Id,
                OrderNumber = orderNumber,
                TransactionNumber = transactionNumber,
                ResponseId = nexioResponse.GatewayResponse.Result == "200" ? "A" : "F" ,
                Response = nexioResponse.Message,
                PaymentType = "Credit"
            };
           
            return res;
        }
    }
}
