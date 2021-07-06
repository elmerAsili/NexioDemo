using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;

namespace Nexio
{
    public class NexioMoneyInKrw : NexioMoneyIn
    {
        public NexioMoneyInKrw(IAssociateService associateService, ILoggingService loggingService, INexioService nexioService, IOrderService orderService, ISettingsService settingsService) 
            : base(associateService, loggingService, nexioService, orderService, settingsService,
                new MerchantInfo
                {
                    Currency = "KRW",
                    DisplayName = "Nexio APM (KRW)",
                    Id = 9909,
                    MerchantName = "Nexio APM (KRW)"
                })
        { }
    }
}
