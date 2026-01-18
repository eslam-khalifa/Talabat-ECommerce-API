namespace LinkDev.Talabat.Core.Commands
{
    public class RegisterCustomerCommand
    {
        public string DisplayName { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public string Password { get; }

        public RegisterCustomerCommand(string displayName, string email, string phoneNumber, string password)
        {
            DisplayName = displayName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
        }
    }
}
