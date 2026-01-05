using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class RegisterDto
    {
        [Required]
        public required string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,20}$",
            ErrorMessage = "Password must be 8-20 characters and include: uppercase letter, lowercase letter, number, and special character (@$!%*?&)")]
        public required string Password { get; set; }

        [Required]
        public required string Phone { get; set; }
    }
}
