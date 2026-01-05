using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public class LoginResult
    {
        public bool RequiresTwoFactor { get; set; }
        public string? TempToken { get; set; }
        public string? AccessToken { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
