using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInGbp : NexioMoneyIn
    {
        public NexioMoneyInGbp(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "GBP",
                    DisplayName = "Nexio APM (GBP)",
                    Id = 9907,
                    MerchantName = "Nexio APM (GBP)"
                })
        { }
    }
}
