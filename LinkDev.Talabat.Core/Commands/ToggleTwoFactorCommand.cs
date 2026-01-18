using System.Security.Claims;

namespace LinkDev.Talabat.Core.Commands
{
    public class ToggleTwoFactorCommand
    {
        public ClaimsPrincipal User { get; set; }
        public bool Enable { get; set; }

        public ToggleTwoFactorCommand(ClaimsPrincipal user, bool enable)
        {
            User = user;
            Enable = enable;
        }
    }
}
