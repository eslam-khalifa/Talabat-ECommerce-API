using LinkDev.Talabat.Core.Entities.Basket;
using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class CustomerBasketDto
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        public required List<BasketItemDto> Items { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
