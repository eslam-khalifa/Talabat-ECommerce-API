using LinkDev.Talabat.Core.Entities.Common;

namespace LinkDev.Talabat.Core.Entities.Order_Aggregate
{
    public class DeliveryMethod : BaseEntity
    {
        public required string ShortName { get; set; }
        public required string Description { get; set; }
        public required decimal Cost { get; set; }
        public required string DeliveryTime { get; set; }
    }
}
