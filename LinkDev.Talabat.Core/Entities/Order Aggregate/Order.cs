using LinkDev.Talabat.Core.Entities.Common;

namespace LinkDev.Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {
        // EF Core requires an empty constructor for mapping purposes
        private Order()
        {
        }

        public Order(string buyerEmail, Address shippingAddress, DeliveryMethod? deliveryMethod, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; } = string.Empty;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; } = null!;
        public DeliveryMethod? DeliveryMethod { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
        public decimal SubTotal { get; set; }
        public string PaymentIntentId { get; set; }

        public decimal GetTotal() => SubTotal + DeliveryMethod.Cost;
    }
}
