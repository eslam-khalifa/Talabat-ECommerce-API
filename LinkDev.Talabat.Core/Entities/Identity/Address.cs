using LinkDev.Talabat.Core.Entities.Common;

namespace LinkDev.Talabat.Core.Entities.Identity
{
    public class Address : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required string ApplicationUserId { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }
    }
}