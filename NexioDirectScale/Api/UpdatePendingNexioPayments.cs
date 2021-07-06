using DirectScale.Disco.Extension.Api;
using Nexio.Models;
using System.Text;

namespace Nexio.Api
{
    public class UpdatePendingNexioPayments : IApiEndpoint
    {
        private readonly INexioService _nexioService;
        private readonly IRequestParsingService _requestParsing;

        public UpdatePendingNexioPayments(INexioService nexioService, IRequestParsingService requestParsing)
        {
            _nexioService = nexioService ?? throw new System.ArgumentNullException(nameof(nexioService));
            _requestParsing = requestParsing ?? throw new System.ArgumentNullException(nameof(requestParsing));
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "Nexio/UpdatePendingPayments",
                RequireAuthentication = false
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            try
            {
                var rObject = _requestParsing.ParseBody<UpdatePendingPaymentsRequest>(request);

                return new Ok(_nexioService.UpdatePendingPayments(rObject));
            }
            catch (System.Exception)
            {
                return new ApiResponse { Content = Encoding.Unicode.GetBytes("Nothing"), MediaType = "JSON", StatusCode = System.Net.HttpStatusCode.NotFound };
            }
        }
    }
}
