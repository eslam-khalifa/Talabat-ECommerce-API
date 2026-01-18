using LinkDev.Talabat.Core.Entities.Basket;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Commands;
using LinkDev.Talabat.Core.Entities.Common;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IPaymentService
    {
        Task<OperationResult<CustomerBasket>> CreateOrUpdatePaymentIntent(CreateOrUpdatePaymentIntentCommand command);
        Task<OperationResult<Order>> UpdateOrderStatus(UpdateOrderStatusCommand command);
    }
}
