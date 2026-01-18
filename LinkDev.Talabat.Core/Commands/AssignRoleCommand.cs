using LinkDev.Talabat.Core.Entities.Identity;
using System.Collections.Generic;

namespace LinkDev.Talabat.Core.Commands
{
    public class AssignRoleCommand
    {
        public ApplicationUser User { get; }
        public string Role { get; }
        public IList<string> ActorRoles { get; }

        public AssignRoleCommand(ApplicationUser user, string role, IList<string> actorRoles)
        {
            User = user;
            Role = role;
            ActorRoles = actorRoles;
        }
    }
}
