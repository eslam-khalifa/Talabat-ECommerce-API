using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;
using System.Security.Claims;
using LinkDev.Talabat.Core.Results;
using LinkDev.Talabat.Core.Commands;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IAuthService
    {
        Task<OperationResult<bool>> ConfirmEmailWithOtpAsync(ConfirmEmailCommand command, CancellationToken cancellationToken);
        Task<OperationResult<string>> CreateTokenAsync(ApplicationUser applicationUser);
        Task<OperationResult<ApplicationUser>> GetCurrentUserAsync(ClaimsPrincipal userClaims);
        Task<OperationResult<Address>> GetUserAddressAsync(ClaimsPrincipal userClaims);
        Task<OperationResult<IList<string>>> GetUserRolesAsync(ClaimsPrincipal userClaims);
        Task<OperationResult<LoginResult>> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);
        Task<OperationResult<ApplicationUser>> RegisterCustomerAsync(RegisterCustomerCommand command);
        Task<OperationResult<ApplicationUser>> RegisterUserWithRoleAsync(RegisterUserWithRoleCommand command);
        Task<OperationResult<EmailOtp>> RequestPasswordResetWithOtpAsync(RequestPasswordResetCommand command, CancellationToken cancellationToken);
        Task<OperationResult<bool>> ResetPasswordWithOtpAsync(ResetPasswordCommand command, CancellationToken cancellationToken);
        Task<OperationResult<EmailOtp>> SendEmailConfirmationOtpAsync(SendEmailOTPCommand command, CancellationToken cancellationToken);
        Task<OperationResult<bool>> ToggleTwoFactorAsync(ToggleTwoFactorCommand command, CancellationToken cancellationToken = default);
        Task<OperationResult<Address>> UpdateUserAddressAsync(UpdateUserAddressCommand command, CancellationToken cancellationToken = default);
        Task<OperationResult<LoginResult>> VerifyTwoFactorAsync(VerifyTwoFactorCommand command, CancellationToken cancellationToken = default);
        Task<OperationResult<IEnumerable<string>>> GenerateRecoveryCodesAsync(GenerateRecoveryCodesCommand command);
        Task<OperationResult<LoginResult>> VerifyRecoveryCodeAsync(VerifyRecoveryCodeCommand command, CancellationToken cancellationToken = default);
    }
}
