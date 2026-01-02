using AutoMapper;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;

namespace LinkDev.Talabat.APIs.Helpers
{
    public class OrderItemPictureUrlResolver(IConfiguration configuration) : IValueResolver<OrderItem, OrderItemDto, string>
    {
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.PictureUrl))
            {
                return $"{configuration["ApiBaseUrl"]}/{source.Product.PictureUrl}";
            }
            return string.Empty;
        }
    }
}
