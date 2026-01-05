using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.Core.Specifications.EmailOtpSpecs
{
    public class HourlyOtpCountSpecification : BaseSpecifications<EmailOtp>
    {
        public HourlyOtpCountSpecification(string userId, string purpose)
            : base()
        {
            var now = DateTime.UtcNow;
            var startOfHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);

            Criteria = otp => otp.UserId == userId && 
                              otp.Purpose == purpose && 
                              otp.CreatedAt >= startOfHour;
        }
    }
}
