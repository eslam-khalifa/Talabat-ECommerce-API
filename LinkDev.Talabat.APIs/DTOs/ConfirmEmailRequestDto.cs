using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class ConfirmEmailRequestDto
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Otp { get; set; } = null!;
    }
}
