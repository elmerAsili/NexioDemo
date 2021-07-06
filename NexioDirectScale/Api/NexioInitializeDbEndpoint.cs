using DirectScale.Disco.Extension.Api;
using System.Text;

namespace Nexio.Api
{
    public class NexioInitializeDbEndpoint : IApiEndpoint
    {
        private readonly INexioRepository _nexioRepository;

        public NexioInitializeDbEndpoint(INexioRepository nexioRepository)
        {
            _nexioRepository = nexioRepository ?? throw new System.ArgumentNullException(nameof(nexioRepository));
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "Nexio/InitializeDb",
                RequireAuthentication = false
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            try
            {
                return new Ok(_nexioRepository.CreateNexioTables());
            }
            catch (System.Exception)
            {
                return new ApiResponse { Content = Encoding.Unicode.GetBytes("Nothing"), MediaType = "JSON", StatusCode = System.Net.HttpStatusCode.NotFound };
            }
        }
    }
}
