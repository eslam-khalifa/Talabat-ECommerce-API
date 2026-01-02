using LinkDev.Talabat.Core.Entities.Common;

namespace LinkDev.Talabat.Core.Entities.Order_Aggregate
{
    public class OrderItem : BaseEntity
    {
        // EF Core requires an empty constructor for mapping purposes
        private OrderItem()
        {
        }

        public OrderItem(ProductItemOrdered product, decimal price, int quantity)
        {
            Product = product;
            Price = price;
            Quantity = quantity;
        }

        public ProductItemOrdered Product {  get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
