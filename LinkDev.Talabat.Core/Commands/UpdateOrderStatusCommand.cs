namespace LinkDev.Talabat.Core.Commands
{
    public class UpdateOrderStatusCommand
    {
        public string PaymentIntentId { get; }
        public bool IsPaid { get; }

        public UpdateOrderStatusCommand(string paymentIntentId, bool isPaid)
        {
            PaymentIntentId = paymentIntentId;
            IsPaid = isPaid;
        }
    }
}
