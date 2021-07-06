using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInEur : NexioMoneyIn
    {
        public NexioMoneyInEur(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "EUR",
                    DisplayName = "Nexio APM (EUR)",
                    Id = 9906,
                    MerchantName = "Nexio APM (EUR)"
                })
        { }
    }
}
