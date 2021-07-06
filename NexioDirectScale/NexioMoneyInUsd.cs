using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInUsd : NexioMoneyIn
    {
        public NexioMoneyInUsd(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "USD",
                    DisplayName = "Nexio APM (USD)",
                    Id = 9902,
                    MerchantName = "Nexio APM (USD)"
                })
        { }
    }
}
