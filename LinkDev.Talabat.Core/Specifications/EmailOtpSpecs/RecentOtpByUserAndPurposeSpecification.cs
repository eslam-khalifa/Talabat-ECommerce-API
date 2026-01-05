using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.Core.Specifications.EmailOtpSpecs
{
    public class RecentOtpByUserAndPurposeSpecification : BaseSpecifications<EmailOtp>
    {
        public RecentOtpByUserAndPurposeSpecification(string userId, string purpose, int cooldownSeconds)
            : base(otp => otp.UserId == userId && otp.Purpose == purpose && otp.CreatedAt > DateTime.UtcNow.AddSeconds(-cooldownSeconds))
        {
        }
    }
}
