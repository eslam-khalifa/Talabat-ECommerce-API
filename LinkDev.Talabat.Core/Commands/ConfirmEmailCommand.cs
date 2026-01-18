namespace LinkDev.Talabat.Core.Commands
{
    public class ConfirmEmailCommand
    {
        public string Email { get; }
        public string Otp { get; }

        public ConfirmEmailCommand(string email, string otp)
        {
            Email = email;
            Otp = otp;
        }
    }
}
