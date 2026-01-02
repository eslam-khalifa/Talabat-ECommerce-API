using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.Core.Specifications.EmailOtpSpecs
{
    public class EmailOtpByUserAndPurposeSpecification : BaseSpecifications<EmailOtp>
    {
        public EmailOtpByUserAndPurposeSpecification(string userId, string purpose, string otpHash)
            : base(e => e.UserId == userId && e.Purpose == purpose && e.Otp == otpHash)
        {
        }
    }
}
