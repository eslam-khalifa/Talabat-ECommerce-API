using LinkDev.Talabat.Core.Entities.Identity;
using System.Security.Claims;

namespace LinkDev.Talabat.Core.Commands
{
    public class UpdateUserAddressCommand
    {
        public ClaimsPrincipal User { get; }
        public Address Address { get; }

        public UpdateUserAddressCommand(ClaimsPrincipal user, Address address)
        {
            User = user;
            Address = address;
        }
    }
}
