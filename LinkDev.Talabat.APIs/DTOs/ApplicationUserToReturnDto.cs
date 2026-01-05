using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.APIs.DTOs
{
    public class ApplicationUserToReturnDto
    {
        public string DisplayName { get; set; } = null!;
        public Address? Address { get; set; }
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; } = null!;
        public IList<string>? Roles;
    }
}