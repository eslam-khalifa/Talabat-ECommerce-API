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
    public interface IOrderService
    {
        Task<OperationResult<Order>> CreateOrderAsync(CreateOrderCommand command);
        Task<OperationResult<IReadOnlyList<Order>>> GetOrdersForUserAsync(string buyerEmail);
        Task<OperationResult<Order>> GetOrderByIdForUserAsync(string buyerEmail, int orderId);
        Task<OperationResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethodsAsync();
    }
}
