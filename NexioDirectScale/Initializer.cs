using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Api;
using Nexio.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Nexio
{
    public static class Initializer
    {
        public static void UseNexio(this IServiceCollection services)
        {
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInMxn>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInUsd>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInAud>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInCad>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInEur>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInGbp>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInJpy>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInKrw>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInNzd>();
            services.AddSingleton<IMoneyInMerchant, NexioMoneyInTwd>();

            services.AddSingleton<INexioRepository, NexioRepository>();
            services.AddSingleton<INexioService, NexioService>();

            services.AddSingleton<IApiEndpoint, UpdateNexioSettingsEndpoint>();
            services.AddSingleton<IApiEndpoint, NexioCallbackEndpoint>();
            services.AddSingleton<IApiEndpoint, UpdatePendingNexioPayments>(); 
            services.AddSingleton<IApiEndpoint, NexioInitializeDbEndpoint>();
        }
    }
}
