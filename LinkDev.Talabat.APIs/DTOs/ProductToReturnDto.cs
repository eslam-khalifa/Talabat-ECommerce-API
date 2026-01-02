using LinkDev.Talabat.Core.Entities.Products;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = null!;
        public int CategoryId { get; set; }
        public string Category { get; set; } = null!;
        public int BrandId { get; set; }
        public string Brand { get; set; } = null!;
        public string VendorId { get; set; } = null!;
    }
}
