namespace LinkDev.Talabat.Core.Commands
{
    public class DeleteUserCommand
    {
        public string UserId { get; }

        public DeleteUserCommand(string userId)
        {
            UserId = userId;
        }
    }
}
