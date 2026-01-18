namespace LinkDev.Talabat.Core.Commands
{
    public class CreateOrUpdatePaymentIntentCommand
    {
        public string BasketId { get; }

        public CreateOrUpdatePaymentIntentCommand(string basketId)
        {
            BasketId = basketId;
        }
    }
}
