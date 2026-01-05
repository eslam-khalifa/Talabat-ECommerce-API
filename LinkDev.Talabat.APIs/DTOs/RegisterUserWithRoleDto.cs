using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class RegisterUserWithRoleDto
    {
        [Required]
        public string? DisplayName { get; set; }
        [Required]
        public string? Email { get; set; }
    }
}
