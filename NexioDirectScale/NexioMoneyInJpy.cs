using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInJpy : NexioMoneyIn
    {
        public NexioMoneyInJpy(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "JPY",
                    DisplayName = "Nexio APM (JPY)",
                    Id = 9908,
                    MerchantName = "Nexio APM (JPY)"
                })
        { }
    }
}
