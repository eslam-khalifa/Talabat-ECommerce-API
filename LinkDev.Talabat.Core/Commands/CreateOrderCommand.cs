using LinkDev.Talabat.Core.Entities.Order_Aggregate;

namespace LinkDev.Talabat.Core.Commands
{
    public class CreateOrderCommand
    {
        public string BuyerEmail { get; }
        public string BasketId { get; }
        public int DeliveryMethodId { get; }
        public Address ShippingAddress { get; }

        public CreateOrderCommand(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            BuyerEmail = buyerEmail;
            BasketId = basketId;
            DeliveryMethodId = deliveryMethodId;
            ShippingAddress = shippingAddress;
        }
    }
}
