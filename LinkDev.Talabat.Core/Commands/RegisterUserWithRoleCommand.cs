using LinkDev.Talabat.Core.Entities.Identity;
using System.Collections.Generic;

namespace LinkDev.Talabat.Core.Commands
{
    public class RegisterUserWithRoleCommand
    {
        public ApplicationUser User { get; }
        public string Password { get; }
        public string Role { get; }
        public IList<string> CreatorRoles { get; }

        public RegisterUserWithRoleCommand(ApplicationUser user, string password, string role, IList<string> creatorRoles)
        {
            User = user;
            Password = password;
            Role = role;
            CreatorRoles = creatorRoles;
        }
    }
}
