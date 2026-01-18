using AutoMapper;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Controllers;
using LinkDev.Talabat.Core.Commands;

namespace LinkDev.Talabat.APIs.Controllers
{
    // current user actions
    public class AccountController(IAuthService authService,
        IEmailService emailService,
        IMapper mapper) : BaseApiController
    {
        [HttpPost("email/confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto dto, CancellationToken cancellationToken)
        {
            var command = new ConfirmEmailCommand(dto.Email, dto.Otp);
            var result = await authService.ConfirmEmailWithOtpAsync(command, cancellationToken);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(new ApiSuccessResponse(200, "Email confirmed successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(model.Email, model.Password);
            var result = await authService.LoginAsync(command, cancellationToken);
            if (!result.IsSuccess) return Unauthorized(new ApiErrorResponse(401, result.Errors));
            var response = new LoginResponse
            {
                RequiresTwoFactor = result.Data.RequiresTwoFactor,
                TempToken = result.Data.TempToken,
                AccessToken = result.Data.AccessToken,
                DisplayName = result.Data.User?.DisplayName,
                Email = result.Data.User?.Email
            };
            return Ok(new ApiSuccessResponse(200, response));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterCustomer(RegisterDto model, CancellationToken cancellationToken)
        {
            var command = new RegisterCustomerCommand(model.DisplayName, model.Email, model.Phone, model.Password);
            var result = await authService.RegisterCustomerAsync(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            var token = await authService.CreateTokenAsync(result.Data);
            if (!token.IsSuccess) return BadRequest(new ApiErrorResponse(400, token.Errors));
            return Ok(new ApiSuccessResponse(200, new UserDto()
            {
                DisplayName = result.Data.DisplayName,
                Email = result.Data.Email,
                Token = token.Data
            }));
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost("register/{role}")]
        public async Task<IActionResult> RegisterUserWithRole(RegisterDto model, string role, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.Phone
            };
            var command = new RegisterUserWithRoleCommand(user, model.Password, role, User.FindAll(ClaimTypes.Role).Select(role => role.Value).ToList());
            var result = await authService.RegisterUserWithRoleAsync(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(new ApiSuccessResponse(200, new RegisterUserWithRoleDto()
            {
                DisplayName = result.Data.DisplayName,
                Email = result.Data.Email
            }));
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto dto, CancellationToken cancellationToken)
        {
            var command = new RequestPasswordResetCommand(dto.Email);
            var operationResult = await authService.RequestPasswordResetWithOtpAsync(command, cancellationToken);
            if (!operationResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, operationResult.Errors));
            else return Ok(new ApiSuccessResponse(200, "A reset code was sent to your email."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request, CancellationToken cancellationToken)
        {
            var command = new ResetPasswordCommand(request.Email, request.NewPassword, request.Otp);
            var operationResult = await authService.ResetPasswordWithOtpAsync(command, cancellationToken);
            if (!operationResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, operationResult.Errors));
            else return Ok(new ApiSuccessResponse(200, "Password reset successfully."));
        }

        [HttpPost("email/send-otp")]
        public async Task<IActionResult> SendEmailOtp([FromBody] SendEmailConfirmationOtpRequestDto dto, CancellationToken cancellationToken)
        {
            var command = new SendEmailOTPCommand(dto.Email);
            var operationResult = await authService.SendEmailConfirmationOtpAsync(command, cancellationToken);
            if (!operationResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, operationResult.Errors));
            else return Ok(new ApiSuccessResponse(200, "OTP sent"));
        }

        [HttpPost("2fa/toggle")]
        public async Task<IActionResult> ToggleTwoFactorAuthentication([FromBody] ToggleTwoFactorDto dto, CancellationToken cancellationToken)
        {
            var command = new ToggleTwoFactorCommand(User, dto.Enable);
            var operationResult = await authService.ToggleTwoFactorAsync(command, cancellationToken);
            if (!operationResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, operationResult.Errors));
            return Ok(new ApiSuccessResponse(200, dto.Enable? "2FA toggled on successfully." : "2FA toggled off successfully.", operationResult.Data));
        }

        [HttpPost("2fa/verify")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorDto dto, CancellationToken cancellationToken)
        {
            var command = new VerifyTwoFactorCommand(dto.Otp, dto.TempToken);
            var result = await authService.VerifyTwoFactorAsync(command, cancellationToken);
            if (!result.IsSuccess) return Unauthorized(new ApiErrorResponse(401, result.Errors));

            var response = new LoginResponse
            {
                RequiresTwoFactor = false,
                AccessToken = result.Data.AccessToken,
                DisplayName = result.Data.User?.DisplayName,
                Email = result.Data.User?.Email
            };

            return Ok(new ApiSuccessResponse(200, response));
        }

        [Authorize]
        [HttpPost("2fa/recovery-codes")]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var command = new GenerateRecoveryCodesCommand(User);
            var result = await authService.GenerateRecoveryCodesAsync(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(new ApiSuccessResponse(200, "Recovery codes generated successfully.", result.Data));
        }

        [HttpPost("2fa/recovery/verify")]
        public async Task<IActionResult> VerifyRecoveryCode([FromBody] VerifyRecoveryCodeDto dto, CancellationToken cancellationToken)
        {
            var command = new VerifyRecoveryCodeCommand(dto.RecoveryCode, dto.TempToken);
            var result = await authService.VerifyRecoveryCodeAsync(command, cancellationToken);
            if (!result.IsSuccess) return Unauthorized(new ApiErrorResponse(401, result.Errors));

            var response = new LoginResponse
            {
                RequiresTwoFactor = false,
                AccessToken = result.Data.AccessToken,
                DisplayName = result.Data.User?.DisplayName,
                Email = result.Data.User?.Email
            };

            return Ok(new ApiSuccessResponse(200, response));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await authService.GetCurrentUserAsync(User);
            if (!result.IsSuccess) return Unauthorized(new ApiErrorResponse(401, result.Errors));
            var token = await authService.CreateTokenAsync(result.Data);
            if (!token.IsSuccess) return BadRequest(new ApiErrorResponse(400, token.Errors));
            return Ok(new ApiSuccessResponse(200, new UserDto()
            {
                DisplayName = result.Data?.DisplayName ?? string.Empty,
                Email = result.Data?.Email ?? string.Empty,
                Token = token.Data
            }));
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<IActionResult> GetUserAddress(CancellationToken cancellationToken)
        {
            var result = await authService.GetUserAddressAsync(User);
            if (!result.IsSuccess) return Unauthorized(new ApiErrorResponse(401, result.Errors));
            return Ok(new ApiSuccessResponse(200, mapper.Map<AddressDto>(result.Data)));
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<IActionResult> UpdateUserAddress(AddressDto address, CancellationToken cancellationToken)
        {
            var updatedAddress = mapper.Map<Address>(address);
            var command = new UpdateUserAddressCommand(User, updatedAddress);
            var userResult = await authService.UpdateUserAddressAsync(command, cancellationToken);
            if (!userResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, userResult.Errors));
            return Ok(new ApiSuccessResponse(200, address));
        }

        [Authorize]
        [HttpGet("roles")]
        public async Task<IActionResult> GetUserRoles(CancellationToken cancellationToken)
        {
            var result = await authService.GetUserRolesAsync(User);
            if (!result.IsSuccess) return NotFound(new ApiErrorResponse(404, result.Errors));
            return Ok(new ApiSuccessResponse(200, result.Data));
        }
    }
}
