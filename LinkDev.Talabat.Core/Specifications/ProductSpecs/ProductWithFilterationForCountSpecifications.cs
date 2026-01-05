using LinkDev.Talabat.Core.Entities.Products;

namespace LinkDev.Talabat.Core.Specifications.ProductSpecs
{
    public class ProductWithFilterationForCountSpecifications : BaseSpecifications<Product>
    {
        public ProductWithFilterationForCountSpecifications(ProductSpecParams productSpecParams)
            : base(p =>
                (string.IsNullOrEmpty(productSpecParams.Search) || p.Name.ToLower().Contains(productSpecParams.Search)) &&
                (!productSpecParams.BrandId.HasValue || p.BrandId == productSpecParams.BrandId) &&
                (!productSpecParams.CategoryId.HasValue || p.CategoryId == productSpecParams.CategoryId)
            )
        {
        }
    }
}
