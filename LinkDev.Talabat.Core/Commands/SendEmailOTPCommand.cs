namespace LinkDev.Talabat.Core.Commands
{
    public class SendEmailOTPCommand
    {
        public string Email { get; }

        public SendEmailOTPCommand(string email)
        {
            Email = email;
        }
    }
}
