using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Specifications.OrderSpecs
{
    public class OrderSpecificaitons : BaseSpecifications<Order>
    {
        public OrderSpecificaitons(string buyerEmail)
            : base(order => order.BuyerEmail == buyerEmail)
        {
            Includes.Add(order => order.DeliveryMethod);
            Includes.Add(order => order.Items);
            AddOrderByDescending(order => order.OrderDate);
        }

        public OrderSpecificaitons(int orderId, string buyerEmail)
            : base(order => order.BuyerEmail == buyerEmail && order.Id == orderId)
        {
            Includes.Add(order => order.DeliveryMethod);
            Includes.Add(order => order.Items);
        }
    }
}
