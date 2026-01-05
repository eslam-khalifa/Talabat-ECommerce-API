using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class SendEmailConfirmationOtpRequestDto
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}
