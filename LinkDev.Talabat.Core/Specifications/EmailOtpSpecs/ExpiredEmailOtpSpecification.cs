using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.Core.Specifications.EmailOtpSpecs
{
    public class ExpiredEmailOtpSpecification : BaseSpecifications<EmailOtp>
    {
        public ExpiredEmailOtpSpecification()
            : base(e => e.ExpiresAt <= DateTime.UtcNow)
        {
        }
    }
}
