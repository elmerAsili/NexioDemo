using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInTwd : NexioMoneyIn
    {
        public NexioMoneyInTwd(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "TWD",
                    DisplayName = "Nexio APM (TWD)",
                    Id = 9911,
                    MerchantName = "Nexio APM (TWD)"
                })
        { }
    }
}
