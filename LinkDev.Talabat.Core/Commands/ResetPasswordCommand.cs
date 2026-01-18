namespace LinkDev.Talabat.Core.Commands
{
    public class ResetPasswordCommand
    {
        public string Email { get; }
        public string NewPassword { get; }
        public string Otp { get; }

        public ResetPasswordCommand(string email, string newPassword, string otp)
        {
            Email = email;
            NewPassword = newPassword;
            Otp = otp;
        }
    }
}
