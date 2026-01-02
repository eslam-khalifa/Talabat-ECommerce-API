using System.Security.Cryptography;
using System.Text;

namespace LinkDev.Talabat.Core.Helpers
{
    public class OtpHelper
    {
        public static string GenerateOtp(int length = 6)
        {
            var random = new Random();
            return random.Next(0, (int)Math.Pow(10, length))
                         .ToString($"D{length}"); // pads with zeros
        }

        public static string HashOtp(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otp);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyOtp(string otp, string hashedOtp)
        {
            return HashOtp(otp) == hashedOtp;
        }
    }
}
