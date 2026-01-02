using LinkDev.Talabat.Core;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using LinkDev.Talabat.Core.Entities.Products;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Core.Specifications.OrderSpecs;

namespace LinkDev.Talabat.Application
{
    public class OrderService(
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService) : IOrderService
    {
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);

            var orderItems = new List<OrderItem>();
            if (basket?.Items?.Count > 0)
            {
                var productRepository = unitOfWork.Repository<Product>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepository.GetAsync(item.Id);
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                    orderItems.Add(orderItem);
                }
            }

            var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);

            // check for existing order with the same payment intent id
            var orderRepository = unitOfWork.Repository<Order>();
            var spec = new OrderWithPaymentIntentSpecifications(basket!.PaymentIntentId!);
            var existingOrder = await orderRepository.GetWithSpecAsync(spec);
            if(existingOrder is not null)
            {
                orderRepository.Delete(existingOrder);
                await paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal, basket?.PaymentIntentId ?? "");

            unitOfWork.Repository<Order>().Add(order);

            var result = await unitOfWork.CompleteAsync();
            if (result <= 0)
            {
                return null;
            }
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        }

        public Task<Order?> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            var orderRepository = unitOfWork.Repository<Order>();
            var spec = new OrderSpecificaitons(orderId, buyerEmail);
            var order = orderRepository.GetWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var ordersRepository = unitOfWork.Repository<Order>();
            var spec = new OrderSpecificaitons(buyerEmail);
            var orders = await ordersRepository.GetAllWithSpecAsync(spec);
            return orders;
        }
    }
}
