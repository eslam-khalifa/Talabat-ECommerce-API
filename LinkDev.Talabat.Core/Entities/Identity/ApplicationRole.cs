using Microsoft.AspNetCore.Identity;

namespace LinkDev.Talabat.Core.Entities.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole(string name, string description) : base(name)
        {
            Description = description;
        }

        public string Description { get; set; } = null!;
    }
}
