namespace LinkDev.Talabat.Core.Commands
{
    public class VerifyTwoFactorCommand
    {
        public string Otp { get; }
        public string TempToken { get; }

        public VerifyTwoFactorCommand(string otp, string tempToken)
        {
            Otp = otp;
            TempToken = tempToken;
        }
    }
}
