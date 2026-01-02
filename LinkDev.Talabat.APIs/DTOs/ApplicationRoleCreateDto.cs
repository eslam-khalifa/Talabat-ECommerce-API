using System.ComponentModel.DataAnnotations;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class ApplicationRoleCreateDto
    {
        [Required]
        public string? Name;
        [Required]
        public string? Description;
    }
}
