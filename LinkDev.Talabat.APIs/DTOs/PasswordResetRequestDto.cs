using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class PasswordResetRequestDto
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}
