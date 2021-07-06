using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using Newtonsoft.Json;
using Nexio.Models;
using System;
using System.Text;

namespace Nexio.Api
{
    public class NexioCallbackEndpoint : IApiEndpoint
    {
        private readonly ILoggingService _loggingService;
        private readonly INexioService _nexioService;
        private readonly IRequestParsingService _requestParsing;

        public NexioCallbackEndpoint(ILoggingService loggingService, INexioService nexioService, IRequestParsingService requestParsing)
        {
            _loggingService = loggingService ?? throw new System.ArgumentNullException(nameof(loggingService));
            _nexioService = nexioService ?? throw new System.ArgumentNullException(nameof(nexioService));
            _requestParsing = requestParsing ?? throw new System.ArgumentNullException(nameof(requestParsing));
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "Nexio/Callback",
                RequireAuthentication = false
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            try
            {
                var callbackObj = _requestParsing.ParseBody<NexioCallback>(request);

                return new Ok(_nexioService.ProcessCallback(callbackObj));
            }
            catch (Exception e)
            {
                _loggingService.LogError(e, $"An issue occurred processing the Nexio/Callback callback request. {request.Body.ReadAsString()}", request.Body.ReadAsString());
                return new ApiResponse { Content = Encoding.Unicode.GetBytes("Nothing"), MediaType = "UrlEncoded", StatusCode = System.Net.HttpStatusCode.NotFound };
            }
        }
    }
}
