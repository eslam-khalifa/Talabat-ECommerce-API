using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class OrderDto
    {
        [Required]
        public string BasketId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DeliveryMethodId { get; set; }

        [Required]
        public AddressDto ShippingAddress { get; set; }
    }
}
