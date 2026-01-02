using AutoMapper;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.Core.Entities.Products;

namespace LinkDev.Talabat.APIs.Helpers
{
    public class ProductPictureUrlResolver(IConfiguration configuration) : IValueResolver<Product, ProductToReturnDto, string>
    {
        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                return $"{configuration["ApiBaseUrl"]}/{source.PictureUrl}";
            }
            return string.Empty;
        }
    }
}
