using Microsoft.AspNetCore.Identity;

namespace LinkDev.Talabat.Core.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public Address? Address { get; set; }
    }
}
