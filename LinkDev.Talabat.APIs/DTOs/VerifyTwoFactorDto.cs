using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class VerifyTwoFactorDto
    {
        [Required]
        public string Otp { get; set; } = null!;
        [Required]
        public string TempToken { get; set; } = null!;
    }
}
