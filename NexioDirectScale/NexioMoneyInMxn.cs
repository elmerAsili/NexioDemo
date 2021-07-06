using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInMxn : NexioMoneyIn
    {
        public NexioMoneyInMxn(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "MXN",
                    DisplayName = "OpenPay",
                    Id = 9903,
                    MerchantName = "OpenPay"
                })
        { }
    }
}
