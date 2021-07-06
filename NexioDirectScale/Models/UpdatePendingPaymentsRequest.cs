using System;

namespace Nexio.Models
{
    public class UpdatePendingPaymentsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
