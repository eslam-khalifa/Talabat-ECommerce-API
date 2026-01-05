namespace LinkDev.Talabat.APIs.DTOs
{
    public class LoginResponse
    {
        public bool RequiresTwoFactor { get; set; }
        public string? TempToken { get; set; } // short-lived token
        public string? AccessToken { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
    }
}
