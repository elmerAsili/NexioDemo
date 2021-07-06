using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInAud : NexioMoneyIn
    {
        public NexioMoneyInAud(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "AUD",
                    DisplayName = "Nexio APM (AUD)",
                    Id = 9904,
                    MerchantName = "Nexio APM (AUD)"
                })
        { }
    }
}
