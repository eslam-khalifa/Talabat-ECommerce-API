using LinkDev.Talabat.Core;
using LinkDev.Talabat.Core.Specifications.OrderSpecs;
using LinkDev.Talabat.Core.Commands;
using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using LinkDev.Talabat.Core.Entities.Products;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Services.Contracts;

namespace LinkDev.Talabat.Application
{
    public class OrderService(
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService) : IOrderService
    {
        public async Task<OperationResult<Order>> CreateOrderAsync(CreateOrderCommand command)
        {
            var basket = await basketRepository.GetBasketAsync(command.BasketId);
            if (basket == null) return OperationResult<Order>.Fail("Basket not found");

            var orderItems = new List<OrderItem>();
            if (basket.Items?.Count > 0)
            {
                var productRepository = unitOfWork.Repository<Product>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepository.GetAsync(item.Id);
                    if (product == null) return OperationResult<Order>.Fail($"Product with ID {item.Id} not found");
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                    orderItems.Add(orderItem);
                }
            }

            var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetAsync(command.DeliveryMethodId);
            if (deliveryMethod == null) return OperationResult<Order>.Fail("Delivery method not found");

            // check for existing order with the same payment intent id
            var orderRepository = unitOfWork.Repository<Order>();
            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId!);
            var existingOrder = await orderRepository.GetWithSpecAsync(spec);
            if(existingOrder is not null)
            {
                orderRepository.Delete(existingOrder);
                var paymentCommand = new CreateOrUpdatePaymentIntentCommand(command.BasketId);
                await paymentService.CreateOrUpdatePaymentIntent(paymentCommand);
            }

            var order = new Order(command.BuyerEmail, command.ShippingAddress, deliveryMethod, orderItems, subTotal, basket.PaymentIntentId ?? "");

            unitOfWork.Repository<Order>().Add(order);

            var result = await unitOfWork.CompleteAsync();
            if (result <= 0)
            {
                return OperationResult<Order>.Fail("Failed to create order");
            }
            return OperationResult<Order>.Success(order);
        }

        public async Task<OperationResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethodsAsync()
        {
            var methods = await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return OperationResult<IReadOnlyList<DeliveryMethod>>.Success(methods);
        }

        public async Task<OperationResult<Order>> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            var orderRepository = unitOfWork.Repository<Order>();
            var spec = new OrderSpecificaitons(orderId, buyerEmail);
            var order = await orderRepository.GetWithSpecAsync(spec);
            if (order == null) return OperationResult<Order>.Fail("Order not found");
            return OperationResult<Order>.Success(order);
        }

        public async Task<OperationResult<IReadOnlyList<Order>>> GetOrdersForUserAsync(string buyerEmail)
        {
            var ordersRepository = unitOfWork.Repository<Order>();
            var spec = new OrderSpecificaitons(buyerEmail);
            var orders = await ordersRepository.GetAllWithSpecAsync(spec);
            return OperationResult<IReadOnlyList<Order>>.Success(orders);
        }
    }
}
