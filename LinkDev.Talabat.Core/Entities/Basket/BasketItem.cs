namespace LinkDev.Talabat.Core.Entities.Basket
{
    public class BasketItem
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public required string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public required string Category { get; set; }
        public required string Brand { get; set; }
    }
}