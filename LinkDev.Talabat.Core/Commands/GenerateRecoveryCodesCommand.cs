using System.Security.Claims;

namespace LinkDev.Talabat.Core.Commands
{
    public class GenerateRecoveryCodesCommand
    {
        public ClaimsPrincipal User { get; }

        public GenerateRecoveryCodesCommand(ClaimsPrincipal user)
        {
            User = user;
        }
    }
}
