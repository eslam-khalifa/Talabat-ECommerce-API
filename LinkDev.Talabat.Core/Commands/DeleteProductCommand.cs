using System.Collections.Generic;

namespace LinkDev.Talabat.Core.Commands
{
    public class DeleteProductCommand
    {
        public int Id { get; }
        public string CurrentUserId { get; }
        public IList<string> CurrentUserRoles { get; }

        public DeleteProductCommand(int id, string currentUserId, IList<string> currentUserRoles)
        {
            Id = id;
            CurrentUserId = currentUserId;
            CurrentUserRoles = currentUserRoles;
        }
    }
}
