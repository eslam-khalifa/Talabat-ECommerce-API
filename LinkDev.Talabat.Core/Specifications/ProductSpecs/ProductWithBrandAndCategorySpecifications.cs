using LinkDev.Talabat.Core.Entities.Products;

namespace LinkDev.Talabat.Core.Specifications.ProductSpecs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams productSpecParams)
            : base(p =>
                (string.IsNullOrEmpty(productSpecParams.Search) || p.Name.ToLower().Contains(productSpecParams.Search)) &&
                (!productSpecParams.BrandId.HasValue || p.BrandId == productSpecParams.BrandId.Value) &&
                (!productSpecParams.CategoryId.HasValue || p.CategoryId == productSpecParams.CategoryId.Value)
            )
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);

            if(!string.IsNullOrEmpty(productSpecParams.Sort))
            {
                switch(productSpecParams.Sort.ToLower())
                {
                    case "priceasc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "pricedesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                AddOrderBy(p => p.Name);
            }

            ApplyPaging((productSpecParams.PageIndex - 1) * productSpecParams.PageSize, productSpecParams.PageSize);
        }

        public ProductWithBrandAndCategorySpecifications(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }
    }
}
