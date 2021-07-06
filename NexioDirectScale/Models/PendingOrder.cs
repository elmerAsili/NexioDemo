namespace Nexio.Models
{
    public class PendingOrder
    {
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public string TransactionNumber { get; set; }
    }
}
