using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class VerifyRecoveryCodeDto
    {
        [Required]
        public required string RecoveryCode { get; set; }
        [Required]
        public required string TempToken { get; set; }
    }
}
