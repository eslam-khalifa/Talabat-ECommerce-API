using LinkDev.Talabat.Core.Entities.Common;

namespace LinkDev.Talabat.Core.Entities.Identity
{
    public class EmailOtp
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty; // "EmailConfirmation" or "PasswordReset"
        public string Otp { get; set; } = string.Empty; // store hashed OTP
        public DateTime ExpiresAt { get; set; }
    }
}
