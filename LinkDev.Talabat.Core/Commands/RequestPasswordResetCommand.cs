namespace LinkDev.Talabat.Core.Commands
{
    public class RequestPasswordResetCommand
    {
        public string Email { get; }

        public RequestPasswordResetCommand(string email)
        {
            Email = email;
        }
    }
}
