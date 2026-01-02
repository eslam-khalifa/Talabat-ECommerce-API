using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.Core.Entities.Products
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = null!;
        public int CategoryId { get; set; }
        public ProductCategory? Category { get; set; }
        public int BrandId { get; set; }
        public ProductBrand? Brand { get; set; }
        public string VendorId { get; set; } = null!;
    }
}
