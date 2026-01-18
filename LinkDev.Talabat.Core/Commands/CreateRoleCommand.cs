using System.Collections.Generic;

namespace LinkDev.Talabat.Core.Commands
{
    public class CreateRoleCommand
    {
        public string RoleName { get; }
        public string Description { get; }
        public IList<string> ActorRoles { get; }

        public CreateRoleCommand(string roleName, string description, IList<string> actorRoles)
        {
            RoleName = roleName;
            Description = description;
            ActorRoles = actorRoles;
        }
    }
}
