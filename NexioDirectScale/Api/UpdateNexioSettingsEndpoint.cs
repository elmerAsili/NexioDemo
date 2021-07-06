using DirectScale.Disco.Extension.Api;
using Nexio.Models;
using System.Text;

namespace Nexio.Api
{
    public class UpdateNexioSettingsEndpoint : IApiEndpoint
    {
        private readonly INexioService _nexioService;
        private readonly IRequestParsingService _requestParsing;

        public UpdateNexioSettingsEndpoint(INexioService nexioService, IRequestParsingService requestParsing)
        {
            _nexioService = nexioService ?? throw new System.ArgumentNullException(nameof(nexioService));
            _requestParsing = requestParsing ?? throw new System.ArgumentNullException(nameof(requestParsing));
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "Nexio/UpdateSettings",
                RequireAuthentication = false
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            var rObject = _requestParsing.ParseBody<NexioSettings>(request);
            
            if (rObject == null)
            {
                return new ApiResponse { Content = Encoding.Unicode.GetBytes("Nothing"), MediaType = "JSON", StatusCode = System.Net.HttpStatusCode.NotFound };
            }

            try
            {
                return new Ok(_nexioService.UpdateNexioSettings(rObject));
            }
            catch (System.Exception)
            {
                return new ApiResponse { Content = Encoding.Unicode.GetBytes("Nothing"), MediaType = "JSON", StatusCode = System.Net.HttpStatusCode.NotFound };
            }
        }
    }
}
