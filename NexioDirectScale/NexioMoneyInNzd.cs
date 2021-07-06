using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInNzd : NexioMoneyIn
    {
        public NexioMoneyInNzd(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "NZD",
                    DisplayName = "Nexio APM (NZD)",
                    Id = 9910,
                    MerchantName = "Nexio APM (NZD)"
                })
        { }
    }
}
