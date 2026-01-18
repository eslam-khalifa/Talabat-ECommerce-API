using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.Core.Results
{
    public class LoginResult
    {
        public bool RequiresTwoFactor { get; set; }
        public string? TempToken { get; set; }
        public string? AccessToken { get; set; }
        public ApplicationUser? User { get; set; }

        public LoginResult(bool requiresTwoFactor, string? tempToken, string? accessToken, ApplicationUser? user)
        {
            RequiresTwoFactor = requiresTwoFactor;
            TempToken = tempToken;
            AccessToken = accessToken;
            User = user;
        }
    }
}
