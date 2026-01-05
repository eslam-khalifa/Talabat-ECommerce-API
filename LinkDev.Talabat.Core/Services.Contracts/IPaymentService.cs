using LinkDev.Talabat.Core.Entities.Basket;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IPaymentService
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
        Task<Order?> UpdateOrderStatus(string paymentIntentId, bool isPaid);
    }
}
