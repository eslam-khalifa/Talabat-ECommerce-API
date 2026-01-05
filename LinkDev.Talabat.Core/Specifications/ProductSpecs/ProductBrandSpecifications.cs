using LinkDev.Talabat.Core.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Specifications.ProductSpecs
{
    public class ProductBrandSpecifications : BaseSpecifications<ProductBrand>
    {
        public ProductBrandSpecifications(int id) : base(productBrand => productBrand.Id == id)
        {
        }
    }
}
