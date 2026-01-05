using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;
using System.Security.Claims;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IAuthService
    {
        Task<OperationResult<bool>> ConfirmEmailWithOtpAsync(string email, string otp, CancellationToken cancellationToken);
        Task<OperationResult<string>> CreateTokenAsync(ApplicationUser applicationUser);
        Task<OperationResult<ApplicationUser>> GetCurrentUserAsync(ClaimsPrincipal userClaims);
        Task<OperationResult<Address>> GetUserAddressAsync(ClaimsPrincipal userClaims);
        Task<OperationResult<IList<string>>> GetUserRolesAsync(ClaimsPrincipal userClaims);
        Task<OperationResult<LoginResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<OperationResult<ApplicationUser>> RegisterCustomerAsync(ApplicationUser applicationUser, string password);
        Task<OperationResult<ApplicationUser>> RegisterUserWithRoleAsync(ApplicationUser applicationUser, string password, string role, IList<string> creatorRoles);
        Task<OperationResult<EmailOtp>> RequestPasswordResetWithOtpAsync(string email, CancellationToken cancellationToken);
        Task<OperationResult<bool>> ResetPasswordWithOtpAsync(string NewPassword, string Email, string otp, CancellationToken cancellationToken);
        Task<OperationResult<EmailOtp>> SendEmailConfirmationOtpAsync(string email, CancellationToken cancellationToken);
        Task<OperationResult<bool>> ToggleTwoFactorAsync(ClaimsPrincipal userClaims, bool enable, CancellationToken cancellationToken = default);
        Task<OperationResult<Address>> UpdateUserAddressAsync(ClaimsPrincipal userClaims, Address address, CancellationToken cancellationToken = default);
    }
}
