using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Specifications.EmailOtpSpecs;
using Microsoft.Extensions.Logging;

namespace LinkDev.Talabat.Application
{
    public class OtpCleanupService(
        IIdentityGenericRepository<EmailOtp> identityGenericRepository,
        ILogger<OtpCleanupService> logger)
    {
        public async Task RemoveExpiredOtpsAsync(CancellationToken cancellationToken = default)
        {
            var spec = new ExpiredEmailOtpSpecification();
            
            // Bulk delete using specification pattern - executes single DELETE SQL statement
            // Returns the count of deleted records
            // no need to use savechanges because DeleteRangeAsync already does it
            var deletedCount = await identityGenericRepository.DeleteRangeAsync(spec, cancellationToken);

            logger.LogInformation("Deleted {Count} expired OTPs at {Time}", deletedCount, DateTime.UtcNow);
        }
    }
}
