using LinkDev.Talabat.Core.Entities.Products;
using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    // everything is required
    public class ProductDto
    {
        [Required] // check that the value is not null
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        public decimal Price { get; set; }
        [Required]
        public required IFormFile Picture { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
