using System.Collections.Generic;

namespace LinkDev.Talabat.Core.Commands
{
    public class DeleteRoleCommand
    {
        public string RoleName { get; }
        public IList<string> ActorRoles { get; }

        public DeleteRoleCommand(string roleName, IList<string> actorRoles)
        {
            RoleName = roleName;
            ActorRoles = actorRoles;
        }
    }
}
