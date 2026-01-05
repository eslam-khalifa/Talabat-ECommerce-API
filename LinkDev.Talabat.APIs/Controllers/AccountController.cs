using AutoMapper;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Controllers;

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
            var result = await authService.ConfirmEmailWithOtpAsync(dto.Email, dto.Otp, cancellationToken);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(new ApiSuccessResponse(200, "Email confirmed successfully"));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model, CancellationToken cancellationToken)
        {
            var userResult = await authService.LoginAsync(model.Email, model.Password);
            if (!userResult.IsSuccess) return Unauthorized(new ApiErrorResponse(401, userResult.Errors));
            if (userResult.Data is null) return BadRequest(new ApiErrorResponse(400, "User not found!"));
            if (!userResult.Data.EmailConfirmed) return BadRequest(new ApiErrorResponse(400, "Please confirm your email before logging in."));
            var token = await authService.CreateTokenAsync(userResult.Data);
            if (!token.IsSuccess) return Unauthorized(new ApiErrorResponse(401, token.Errors));
            await emailService.SendEmailAsync(model.Email, "Welcome Back", "Welcome Back To Talabat Website! You've signed in successfully.", cancellationToken);
            var userDto = new UserDto()
            {
                DisplayName = userResult.Data.DisplayName,
                Email = userResult.Data.Email,
                Token = token.Data
            };
            return Ok(userDto);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterCustomer(RegisterDto model)
        {
            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.Phone
            };
            var result = await authService.RegisterCustomerAsync(user, model.Password);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            var token = await authService.CreateTokenAsync(result.Data);
            if (!token.IsSuccess) return BadRequest(new ApiErrorResponse(400, token.Errors));
            return Ok(new UserDto()
            {
                DisplayName = result.Data.DisplayName,
                Email = result.Data.Email,
                Token = token.Data
            });
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost("register/{role}")]
        public async Task<ActionResult<RegisterUserWithRoleDto>> RegisterUserWithRole(RegisterDto model, string role)
        {
            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.Phone
            };
            var result = await authService.RegisterUserWithRoleAsync(user, model.Password, role, User.FindAll(ClaimTypes.Role).Select(role => role.Value).ToList());
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(new RegisterUserWithRoleDto()
            {
                DisplayName = result.Data.DisplayName,
                Email = result.Data.Email
            });
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto dto, CancellationToken cancellationToken)
        {
            var operationResult = await authService.RequestPasswordResetWithOtpAsync(dto.Email, cancellationToken);
            if (!operationResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, operationResult.Errors));
            else return Ok(new ApiSuccessResponse(200, "A reset code was sent to your email."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request, CancellationToken cancellationToken)
        {
            var operationResult = await authService.ResetPasswordWithOtpAsync(request.NewPassword, request.Email, request.Otp, cancellationToken);
            if (!operationResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, operationResult.Errors));
            else return Ok(new ApiSuccessResponse(200, "Password reset successfully."));
        }

        [HttpPost("email/send-otp")]
        public async Task<IActionResult> SendEmailOtp([FromBody] SendEmailConfirmationOtpRequestDto dto, CancellationToken cancellationToken)
        {
            var operationResult = await authService.SendEmailConfirmationOtpAsync(dto.Email, cancellationToken);
            if (!operationResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, operationResult.Errors));
            else return Ok(new ApiSuccessResponse(200, "OTP sent"));
        }

        [Authorize]
        [HttpPost("2fa/toggle")]
        public async Task<IActionResult> ToggleTwoFactorAuthentication([FromBody] ToggleTwoFactorDto dto, CancellationToken cancellationToken)
        {
            var operationResult = await authService.ToggleTwoFactorAsync(User, dto.Enable);
            if (!operationResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, operationResult.Errors));
            return Ok(new ApiSuccessResponse(200, dto.Enable? "2FA toggled on successfully." : "2FA toggled off successfully.", operationResult.Data));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var result = await authService.GetCurrentUserAsync(User);
            if (!result.IsSuccess) return Unauthorized(new ApiErrorResponse(401, result.Errors));
            var token = await authService.CreateTokenAsync(result.Data);
            if (!token.IsSuccess) return BadRequest(new ApiErrorResponse(400, token.Errors));
            return Ok(new UserDto()
            {
                DisplayName = result.Data?.DisplayName ?? string.Empty,
                Email = result.Data?.Email ?? string.Empty,
                Token = token.Data
            });
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var result = await authService.GetUserAddressAsync(User);
            if (!result.IsSuccess) return Unauthorized(new ApiErrorResponse(401, result.Errors));
            return Ok(mapper.Map<AddressDto>(result.Data));
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<Address>> UpdateUserAddress(AddressDto address)
        {
            var updatedAddress = mapper.Map<Address>(address);
            var userResult = await authService.UpdateUserAddressAsync(User, updatedAddress);
            if (!userResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, userResult.Errors));
            return Ok(address);
        }

        [Authorize]
        [HttpGet("roles")]
        public async Task<IActionResult> GetUserRoles()
        {
            var result = await authService.GetUserRolesAsync(User);
            if (!result.IsSuccess) return NotFound(new ApiErrorResponse(404, result.Errors));
            return Ok(result.Data);
        }
    }
}
