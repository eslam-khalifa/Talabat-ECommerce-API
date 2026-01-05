using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Helpers;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Core.Specifications.EmailOtpSpecs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LinkDev.Talabat.Application
{
    public class AuthService(IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IIdentityGenericRepository<EmailOtp> identityGenericRepository,
        IEmailService emailService) : IAuthService
    {
        // header & payload => encoding using Base64UrlEncoding
        // signature (header + payload + secret key) => hashing
        public async Task<OperationResult<string>> CreateTokenAsync(ApplicationUser applicationUser)
        {
            // Private Claims (User-Defined)
            var authClaims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id), // to include the user ID in the token
                new Claim(JwtRegisteredClaimNames.Name, applicationUser.DisplayName),
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email?? "")
            };

            var userRoles = await userManager.GetRolesAsync(applicationUser);
            foreach(var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AuthKey"] ?? string.Empty));

            var token = new JwtSecurityToken(
                audience: configuration["JWT:ValidAudience"],
                issuer: configuration["JWT:ValidIssuer"],
                expires: DateTime.Now.AddMinutes(double.Parse(configuration["JWT:DurationInMinutes"] ?? "0")),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
            );

            return OperationResult<string>.Success(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<OperationResult<ApplicationUser>> GetCurrentUserAsync(ClaimsPrincipal userClaims)
        {
            var id = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id is null) return OperationResult<ApplicationUser>.Fail("User Id is not found.");
            var applicationUser = await userManager.FindByIdAsync(id);
            if (applicationUser is null) return OperationResult<ApplicationUser>.Fail("User is not found.");
            return OperationResult<ApplicationUser>.Success(applicationUser);
        }

        public async Task<OperationResult<Address>> GetUserAddressAsync(ClaimsPrincipal userClaims)
        {
            var applicationUser = await GetCurrentUserAsync(userClaims);
            if (!applicationUser.IsSuccess) return OperationResult<Address>.Fail(applicationUser.Errors);
            if (applicationUser.Data.Address is null) return OperationResult<Address>.Fail("User address is not found.");
            return OperationResult<Address>.Success(applicationUser.Data.Address);
        }

        public async Task<OperationResult<IList<string>>> GetUserRolesAsync(ClaimsPrincipal userClaims)
        {
            var applicationUser = await GetCurrentUserAsync(userClaims);
            if (!applicationUser.IsSuccess) return OperationResult<IList<string>>.Fail(applicationUser.Errors);
            var userRoles = await userManager.GetRolesAsync(applicationUser.Data);
            return OperationResult<IList<string>>.Success(userRoles);
        }

        public async Task<OperationResult<ApplicationUser>> LoginAsync(string email, string Password)
        {
            var applicationUser = await userManager.FindByEmailAsync(email);
            if (applicationUser is null) return OperationResult<ApplicationUser>.Fail("User not found.");
            var result = await userManager.CheckPasswordAsync(applicationUser, Password);
            if (!result) return OperationResult<ApplicationUser>.Fail("User not found.");
            return OperationResult<ApplicationUser>.Success(applicationUser);
        }

        public async Task<OperationResult<ApplicationUser>> RegisterCustomerAsync(ApplicationUser applicationUser, string password)
        {
            var result = await userManager.CreateAsync(applicationUser, password);
            if (!result.Succeeded) return OperationResult<ApplicationUser>.Fail("User not created.");
            var roleResult = await userManager.AddToRoleAsync(applicationUser, "customer");
            if (!roleResult.Succeeded) return OperationResult<ApplicationUser>.Fail("Failed to assign customer role.");
            return OperationResult<ApplicationUser>.Success(applicationUser);
        }

        public async Task<OperationResult<ApplicationUser>> RegisterUserWithRoleAsync(ApplicationUser applicationUser, string password, string role, IList<string> creatorRoles)
        {
            if (role == "SuperAdmin")
            {
                return OperationResult<ApplicationUser>.Fail("SuperAdmin role can only be created during seeding and cannot be assigned via this method.");
            }
            else if (role == "Admin" && !creatorRoles.Contains("SuperAdmin"))
            {
                return OperationResult<ApplicationUser>.Fail("Only SuperAdmin can create Admin users.");
            }
            else if ((role == "Vendor" || role == "Delivery") && !creatorRoles.Contains("SuperAdmin") && !creatorRoles.Contains("Admin"))
            {
                return OperationResult<ApplicationUser>.Fail("Only SuperAdmin or Admin can create Vendor or Delivery users.");
            }

            var result = await userManager.CreateAsync(applicationUser, password);
            if (!result.Succeeded)
            {
                return OperationResult<ApplicationUser>.Fail("User creation failed.");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                var deleteResult = await userManager.DeleteAsync(applicationUser); // Rollback user creation
                if (!deleteResult.Succeeded) return OperationResult<ApplicationUser>.Fail("Failed to rollback user creation after role check.");
                return OperationResult<ApplicationUser>.Fail($"Role '{role}' does not exist.");
            }

            var roleResult = await userManager.AddToRoleAsync(applicationUser, role);
            if (!roleResult.Succeeded)
            {
                var deleteResult = await userManager.DeleteAsync(applicationUser);
                if (!deleteResult.Succeeded) return OperationResult<ApplicationUser>.Fail("Failed to assign role and rollback user creation.");
                return OperationResult<ApplicationUser>.Fail($"Failed to assign role '{role}'.");
            }

            return OperationResult<ApplicationUser>.Success(applicationUser);
        }

        public async Task<OperationResult<EmailOtp>> RequestPasswordResetWithOtpAsync(string email, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return OperationResult<EmailOtp>.Fail("User is not found.");
            if (user.Email is null) return OperationResult<EmailOtp>.Fail("Email isn't found");

            // Rate limiting: Check if a recent OTP was sent (within last 60 seconds)
            var recentOtpSpec = new RecentOtpByUserAndPurposeSpecification(user.Id, OtpPurpose.PasswordReset.ToString(), Convert.ToInt16(configuration["OTPSettings:CooldownSeconds"]));
            var recentOtp = await identityGenericRepository.GetWithSpecAsync(recentOtpSpec, cancellationToken: cancellationToken);
            if (recentOtp is not null)
            {
                var remainingSeconds = (int)(60 - (DateTime.UtcNow - recentOtp.CreatedAt).TotalSeconds);
                return OperationResult<EmailOtp>.Fail($"Please wait {remainingSeconds} seconds before requesting another OTP.");
            }

            // Hourly Limit Check
            var hourlyCountSpec = new HourlyOtpCountSpecification(user.Id, OtpPurpose.PasswordReset.ToString());
            var hourlyCount = await identityGenericRepository.GetCountAsync(hourlyCountSpec, cancellationToken);
            int hourlyLimit = int.TryParse(configuration["OTP:HourlyLimit"], out int limit) ? limit : 5;
            
            if (hourlyCount >= hourlyLimit)
            {
                return OperationResult<EmailOtp>.Fail($"You have reached the hourly limit of {hourlyLimit} OTP requests for password reset. Please try again later.");
            }

            string otp = OtpHelper.GenerateOtp();
            string hashedOtp = OtpHelper.HashOtp(otp);

            var otpEntry = new EmailOtp
            {
                UserId = user.Id,
                Otp = hashedOtp,
                Purpose = OtpPurpose.PasswordReset.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            identityGenericRepository.Add(otpEntry);
            var result = await identityGenericRepository.SaveChangesAsync(cancellationToken);
            if (result == 0) return OperationResult<EmailOtp>.Fail("OTP wasn't added.");

            await emailService.SendEmailAsync(
                user.Email,
                "Password Reset OTP",
                $"Your password reset code is: {otp}",
                cancellationToken
            );

            return OperationResult<EmailOtp>.Success();
        }

        public async Task<OperationResult<bool>> ResetPasswordWithOtpAsync(string NewPassword, string Email, string otp, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user is null) return OperationResult<bool>.Fail("Invalid email");

            var otpHash = OtpHelper.HashOtp(otp);

            var spec = new EmailOtpByUserAndPurposeSpecification(user.Id, OtpPurpose.PasswordReset.ToString(), otpHash);
            var otpEntry = await identityGenericRepository.GetWithSpecAsync(spec);
            if (otpEntry is null) return OperationResult<bool>.Fail("OTP is not correct.");
            if (otpEntry.ExpiresAt < DateTime.UtcNow) return OperationResult<bool>.Fail("OTP expired.");

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetPasswordResult = await userManager.ResetPasswordAsync(user, resetToken, NewPassword);
            if (!resetPasswordResult.Succeeded) return OperationResult<bool>.Fail(resetPasswordResult.Errors.Select(e => e.Description).ToList());

            identityGenericRepository.Delete(otpEntry);
            var result = await identityGenericRepository.SaveChangesAsync(cancellationToken);
            if (result == 0) return OperationResult<bool>.Fail("OTP wasn't deleted from the EmailOtp table.");

            return OperationResult<bool>.Success();
        }

        public async Task<OperationResult<EmailOtp>> SendEmailConfirmationOtpAsync(string email, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return OperationResult<EmailOtp>.Fail("User not found.");

            // Rate limiting: Check if a recent OTP was sent (within last 60 seconds)
            var recentOtpSpec = new RecentOtpByUserAndPurposeSpecification(user.Id, OtpPurpose.EmailConfirmation.ToString(), Convert.ToInt16(configuration["OTPSettings:CooldownSeconds"]));
            var recentOtp = await identityGenericRepository.GetWithSpecAsync(recentOtpSpec, cancellationToken: cancellationToken);
            if (recentOtp is not null)
            {
                var remainingSeconds = (int)(60 - (DateTime.UtcNow - recentOtp.CreatedAt).TotalSeconds);
                return OperationResult<EmailOtp>.Fail($"Please wait {remainingSeconds} seconds before requesting another OTP.");
            }

            // Rate limiting: Hourly Limit Check
            var hourlyCountSpec = new HourlyOtpCountSpecification(user.Id, OtpPurpose.EmailConfirmation.ToString());
            var hourlyCount = await identityGenericRepository.GetCountAsync(hourlyCountSpec, cancellationToken);
            int hourlyLimit = int.TryParse(configuration["OTPSettings:HourlyLimit"], out int limit) ? limit : 5;

            if (hourlyCount >= hourlyLimit)
            {
                return OperationResult<EmailOtp>.Fail($"You have reached the hourly limit of {hourlyLimit} OTP requests for email confirmation. Please try again later.");
            }

            var otpCode = OtpHelper.GenerateOtp();
            var hashedOtp = OtpHelper.HashOtp(otpCode);

            var otpEntry = new EmailOtp
            {
                UserId = user.Id,
                Otp = hashedOtp,
                Purpose = OtpPurpose.EmailConfirmation.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            identityGenericRepository.Add(otpEntry);
            var result = await identityGenericRepository.SaveChangesAsync();
            if (result == 0) return OperationResult<EmailOtp>.Fail("OTP can't be added.");

            await emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"Your confirmation code is: {otpCode}", cancellationToken);

            return OperationResult<EmailOtp>.Success();
        }

        public async Task<OperationResult<bool>> ConfirmEmailWithOtpAsync(string email, string otpCode, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return OperationResult<bool>.Fail("User not found.");

            var otpCodeHash = OtpHelper.HashOtp(otpCode);

            var spec = new EmailOtpByUserAndPurposeSpecification(user.Id, OtpPurpose.EmailConfirmation.ToString(), otpCodeHash);
            var otpEntry = await identityGenericRepository.GetWithSpecAsync(spec);
            if (otpEntry is null) return OperationResult<bool>.Fail("OTP is not correct.");
            if (otpEntry.ExpiresAt < DateTime.UtcNow) return OperationResult<bool>.Fail("OTP expired.");

            user.EmailConfirmed = true;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return OperationResult<bool>.Fail("Failed to confirm email.");

            return OperationResult<bool>.Success();
        }

        public async Task<OperationResult<Address>> UpdateUserAddressAsync(ClaimsPrincipal userClaims, Address address)
        {
            var applicationUser = await GetCurrentUserAsync(userClaims);
            if (!applicationUser.IsSuccess) return OperationResult<Address>.Fail(applicationUser.Errors);
            applicationUser.Data.Address = address;
            var result = await userManager.UpdateAsync(applicationUser.Data);
            if (!result.Succeeded) return OperationResult<Address>.Fail("Failed to update user address.");
            return OperationResult<Address>.Success(address);
        }
    }
}
