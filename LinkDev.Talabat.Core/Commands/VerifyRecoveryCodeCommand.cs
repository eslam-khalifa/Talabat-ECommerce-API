namespace LinkDev.Talabat.Core.Commands
{
    public class VerifyRecoveryCodeCommand
    {
        public string RecoveryCode { get; }
        public string TempToken { get; }

        public VerifyRecoveryCodeCommand(string recoveryCode, string tempToken)
        {
            RecoveryCode = recoveryCode;
            TempToken = tempToken;
        }
    }
}
