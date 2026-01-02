using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class AssignRoleDto
    {
        [Required]
        public required string UserEmail { get; set; }
        [Required]
        public required string Role { get; set; }
    }
}
