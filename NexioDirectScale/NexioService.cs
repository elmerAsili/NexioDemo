using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using Newtonsoft.Json;
using Nexio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Nexio
{
    public interface INexioService
    {
        ApmOneTimeTokenResponse GetApmOneTimeUseToken(ApmOneTimeUseTokenRequest request);
        NexioSettings GetNexioSettings();
        RefundResponse RefundPayment(RefundRequest request, double paymentAmount);
        HttpStatusCode UpdateNexioSettings(NexioSettings settings);
        string ProcessCallback(NexioCallback data);
        HttpStatusCode UpdatePendingPayments(UpdatePendingPaymentsRequest request);
    }

    public class NexioService : INexioService
    {
        private readonly ILoggingService _loggingService;
        private readonly INexioRepository _nexioRepository;
        private readonly NexioSettings nexioSettings;
        private readonly IOrderService _orderService;

        public NexioService(ILoggingService loggingService, INexioRepository nexioRepository, IOrderService orderService)
        {
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _nexioRepository = nexioRepository ?? throw new ArgumentNullException(nameof(nexioRepository));
            nexioSettings = GetNexioSettings();
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public HttpStatusCode UpdateNexioSettings(NexioSettings settings)
        {
            return _nexioRepository.UpdateNexioSettings(settings);
        }

        public NexioSettings GetNexioSettings()
        {
            return _nexioRepository.GetNexioSettings();
        }

        public string ProcessCallback(NexioCallback data)
        {
            try
            {
                var orderInfo = _orderService.GetOrders(new int[] { int.Parse(data?.Data?.Customer?.OrderNumber.Split('-')[0]) })[0];

                if (orderInfo == null)
                {
                    _loggingService.LogError(new ArgumentNullException(), $"Nexio.NexioServce.ProcessCallback - Order not found.", data);
                }

                if (orderInfo.Status == OrderStatus.Processing || orderInfo.Status == OrderStatus.PartialPaid
                    || orderInfo.Status == OrderStatus.PendingFraudReview || orderInfo.Status.Equals("Card under review.")
                    || orderInfo.Status == OrderStatus.Declined)
                {
                    var orderPayment = orderInfo.Payments.Where(x => x.TransactionNumber != null && x.TransactionNumber.Equals(data?.Id)).FirstOrDefault();
                    
                    if (orderPayment == null) orderPayment = orderInfo.Payments.Where(x => x.PaymentStatus.Equals(PaymentStatus.Pending.ToString())).OrderByDescending(x => x.PaymentId).FirstOrDefault(x => x.Merchant > 9900);

                    if (orderInfo.Status == OrderStatus.Declined && !orderPayment.Status.ToLower().Contains("pending")) return string.Empty;

                    var paymentStatusUpdate = new OrderPaymentStatusUpdate
                    {
                        AuthorizationNumber = orderPayment.AuthorizationNumber,
                        OrderPaymentId = orderPayment.PaymentId,
                        PayType = "Charge",
                        ReferenceNumber = orderPayment.Reference,
                        SavedPaymentId = orderPayment.SavedPaymentId,
                        TransactionNumber = orderPayment.TransactionNumber,
                        ResponseId = "0"
                    };

                    if (data.TransactionStatus.Equals("settled", StringComparison.InvariantCultureIgnoreCase))
                    {
                        paymentStatusUpdate.PaymentStatus = PaymentStatus.Accepted;
                        paymentStatusUpdate.ResponseDescription = "Success";
                    }
                    else if (data.TransactionStatus.Equals("declined", StringComparison.InvariantCultureIgnoreCase))
                    {
                        paymentStatusUpdate.PaymentStatus = PaymentStatus.Rejected;
                        paymentStatusUpdate.ResponseDescription = "Rejected";
                    }
                    else
                    {
                        _loggingService.LogError(new Exception(), $"Nexio.NexioServce.ProcessCallback - Callback transaction status neither settled or declined.", data);

                        return string.Empty;
                    }

                    _orderService.FinalizeOrderPaymentStatus(paymentStatusUpdate);
                    orderInfo = _orderService.GetOrders(new int[] { orderInfo.OrderNumber })[0];

                    if (orderInfo.Status == OrderStatus.Paid)
                    {
                        _orderService.FinalizeAcceptedOrder(orderInfo);
                    }
                    else if (orderInfo.Status == OrderStatus.Declined)
                    {
                        _orderService.FinalizeNonAcceptedOrder(orderInfo);
                    }
                    else
                    {
                        _loggingService.LogError(new ArgumentNullException(), $"Nexio.NexioServce.ProcessCallback - Callback processed but order ({orderInfo.OrderNumber}) is neither Paid or Declined. Order Status: {orderInfo.Status}", data);
                    }
                }
            }
            catch (Exception e)
            {
                _loggingService.LogError(e, $"Nexio.NexioServce.ProcessCallback - Unhandled exception thrown.", data);
            }

            return "1|OK";
        }

        public ApmOneTimeTokenResponse GetApmOneTimeUseToken(ApmOneTimeUseTokenRequest request)
        {
            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var creds = Convert.ToBase64String(Encoding.ASCII.GetBytes(nexioSettings.Username + ":" + nexioSettings.Password));
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Authorization", "Basic " + creds } };
            var responseObj = PostJson<object>(json, $"{nexioSettings.BaseApiUrl}/apm/v3/token", headers).ToString();
            var apmTokenResponse = JsonConvert.DeserializeObject<ApmOneTimeTokenResponse>(responseObj);

            return apmTokenResponse;
        }

        public RefundResponse RefundPayment(RefundRequest request, double paymentAmount)
        {
            RefundResponse refundResponse;
            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var creds = Convert.ToBase64String(Encoding.ASCII.GetBytes(nexioSettings.Username + ":" + nexioSettings.Password));
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Authorization", "Basic " + creds } };

            try
            {
                if (paymentAmount.Equals(request.Data.Amount))
                {
                    refundResponse = PostJson<RefundResponse>(json, $"{nexioSettings.BaseApiUrl}/apm/v3/void", headers);

                    if (!refundResponse.Message.Equals("success", StringComparison.OrdinalIgnoreCase))
                    {
                        refundResponse = PostJson<RefundResponse>(json, $"{nexioSettings.BaseApiUrl}/apm/v3/refund", headers);
                    }
                }
                else
                {
                    refundResponse = PostJson<RefundResponse>(json, $"{nexioSettings.BaseApiUrl}/apm/v3/refund", headers);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return refundResponse;
        }

        private T PostJson<T>(string data, string url, Dictionary<string, string> headers = null, string contentType = "application/json") where T : class
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "POST";

            if (headers != null && headers.Any())
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    httpWebRequest.Headers.Add(header.Key, header.Value);
                }
            }

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch (WebException e)
            {
                var resp = e.Response as HttpWebResponse;

                using (var streamReader = new StreamReader(resp.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
        }

        public HttpStatusCode UpdatePendingPayments(UpdatePendingPaymentsRequest request)
        {
            var pendingOrders = _nexioRepository.GetPendingOrders(request.StartDate, request.EndDate);

            foreach (var order in pendingOrders)
            {
                try
                {
                    var transaction = GetNexioTransaction(order.OrderId).Rows.OrderByDescending(x => x.TransactionDate).FirstOrDefault();

                    if (transaction == null)
                    {
                        _loggingService.LogError(new Exception(), $"NexioApm.NexioService.UpdatePendingPayments - No nexio transaction could be found for order {order.OrderId}.");
                    }
                    else if ((transaction.TransactionStatus != (int)TransactionStatus.voided
                            && transaction.TransactionStatus != (int)TransactionStatus.fraudReject
                            && transaction.TransactionStatus != (int)TransactionStatus.declined
                            && transaction.TransactionStatus != (int)TransactionStatus.settled
                            && transaction.TransactionStatus != (int)TransactionStatus.error
                            )
                            || transaction.Plugin == null)
                    {
                        _loggingService.LogError(new Exception(), $"NexioApm.NexioService.UpdatePendingPayments - Pending Nexio payment is in an incomplete status with Nexio.", JsonConvert.SerializeObject(transaction));

                        continue;
                    }

                    var orderInfo = _orderService.GetOrders(new int[] { order.OrderId }).FirstOrDefault();

                    if (orderInfo != null
                        && (orderInfo.Status == OrderStatus.Processing || orderInfo.Status == OrderStatus.PartialPaid
                        || orderInfo.Status == OrderStatus.PendingFraudReview || orderInfo.Status.Equals("Card under review.")
                        || orderInfo.Status == OrderStatus.Declined
                        || orderInfo.Status.Equals("Shipped", StringComparison.OrdinalIgnoreCase)))
                    {
                        var orderPayment = orderInfo.Payments.Where(x => x.TransactionNumber != null && x.TransactionNumber.Equals(transaction?.Plugin?.OriginalId)).FirstOrDefault();

                        if (orderPayment == null)
                        {
                            var mostRecentPayment = orderInfo.Payments.OrderByDescending(x => x.PaymentId).FirstOrDefault();
                            if (mostRecentPayment.PaymentStatus == PaymentStatus.Pending.ToString())
                                orderPayment = mostRecentPayment;
                        }

                        var paymentStatusUpdate = new OrderPaymentStatusUpdate
                        {
                            AuthorizationNumber = orderPayment.AuthorizationNumber,
                            OrderPaymentId = orderPayment.PaymentId,
                            PayType = PaymentTypes.Charge,
                            ReferenceNumber = orderPayment.Reference,
                            SavedPaymentId = orderPayment.SavedPaymentId,
                            TransactionNumber = orderPayment.TransactionNumber,
                            ResponseId = "0"
                        };

                        if (transaction == null
                            || transaction.TransactionStatus == (int)TransactionStatus.voided
                            || transaction.TransactionStatus == (int)TransactionStatus.fraudReject
                            || transaction.TransactionStatus == (int)TransactionStatus.declined
                            || transaction.TransactionStatus == (int)TransactionStatus.error
                            )
                        {
                            paymentStatusUpdate.PaymentStatus = PaymentStatus.Rejected;
                            paymentStatusUpdate.ResponseDescription = "Rejected";
                        }
                        else if (transaction.TransactionStatus == (int)TransactionStatus.settled)
                        {
                            paymentStatusUpdate.PaymentStatus = PaymentStatus.Accepted;
                            paymentStatusUpdate.ResponseDescription = "Success";
                        }
                        else
                        {
                            continue;
                        }

                        _orderService.FinalizeOrderPaymentStatus(paymentStatusUpdate);
                        orderInfo = _orderService.GetOrders(new int[] { orderInfo.OrderNumber }).FirstOrDefault();

                        _loggingService.LogError(new Exception(), $"NexioApm.NexioService.UpdatePendingPayments - Finalizing Order {orderInfo.OrderNumber}.", JsonConvert.SerializeObject(orderInfo));

                        if (orderInfo.Status == OrderStatus.Paid || orderInfo.IsPaid)
                        {
                            _orderService.FinalizeAcceptedOrder(orderInfo);
                        }
                        else if (orderInfo.Status == OrderStatus.Declined)
                        {
                            _orderService.FinalizeNonAcceptedOrder(orderInfo);
                        }
                        else
                        {
                            _loggingService.LogError(new Exception(), $"NexioApm.NexioService.UpdatePendingPayments - The status of Order {orderInfo.OrderNumber} is neither Paid or Declined. Order and payment will not be updated.", JsonConvert.SerializeObject(orderInfo));
                        }
                    }
                    else
                    {
                        _loggingService.LogError(new Exception(), $"NexioApm.NexioService.UpdatePendingPayments - Pending Nexio payment order is already in a completed status.", JsonConvert.SerializeObject(orderInfo));
                    }

                }
                catch (Exception e)
                {
                    _loggingService.LogError(new Exception(), $"NexioApm.NexioService.UpdatePendingPayments - An error occurred attempting to process status check from Nexio.", e, JsonConvert.SerializeObject(order));
                }
            }

            return HttpStatusCode.OK;
        }

        private TransactionInfo GetNexioTransaction(int orderId)
        {
            var parameters = $"?plugin.orderNumber={orderId}";
            var creds = Convert.ToBase64String(Encoding.ASCII.GetBytes(nexioSettings.Username + ":" + nexioSettings.Password));
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Authorization", "Basic " + creds } };
            var response = GetJson<TransactionInfo>($"{nexioSettings.BaseApiUrl}/transaction/v3{parameters}", headers);

            return response;
        }

        private T GetJson<T>(string url, Dictionary<string, string> headers = null, string contentType = "application/json") where T : class
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "GET";

            if (headers != null && headers.Any())
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    httpWebRequest.Headers.Add(header.Key, header.Value);
                }
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch (WebException e)
            {
                var resp = e.Response as HttpWebResponse;

                using (var streamReader = new StreamReader(resp.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
        }
    }
}