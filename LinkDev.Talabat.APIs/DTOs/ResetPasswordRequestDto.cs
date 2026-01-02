using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Otp { get; set; } = string.Empty;
    }
}
