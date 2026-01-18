using System.Collections.Generic;

namespace LinkDev.Talabat.Core.Commands
{
    public class UpdateRoleCommand
    {
        public string OldRoleName { get; }
        public string? NewRoleName { get; }
        public string? NewDescription { get; }
        public IList<string> ActorRoles { get; }

        public UpdateRoleCommand(string oldRoleName, string? newRoleName, string? newDescription, IList<string> actorRoles)
        {
            OldRoleName = oldRoleName;
            NewRoleName = newRoleName;
            NewDescription = newDescription;
            ActorRoles = actorRoles;
        }
    }
}
