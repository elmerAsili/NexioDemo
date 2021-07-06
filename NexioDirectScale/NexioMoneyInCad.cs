using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInCad : NexioMoneyIn
    {
        public NexioMoneyInCad(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "CAD",
                    DisplayName = "Nexio APM (CAD)",
                    Id = 9905,
                    MerchantName = "Nexio APM (CAD)"
                })
        { }
    }
}
