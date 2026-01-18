using LinkDev.Talabat.Core;
using LinkDev.Talabat.Core.Specifications.OrderSpecs;
using LinkDev.Talabat.Core.Commands;
using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Basket;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace LinkDev.Talabat.Application
{
    public class PaymentService(IConfiguration configuration,
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork) : IPaymentService
    {
        public async Task<OperationResult<CustomerBasket>> CreateOrUpdatePaymentIntent(CreateOrUpdatePaymentIntentCommand command)
        {
            StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];
            var basket = await basketRepository.GetBasketAsync(command.BasketId);
            if (basket is null) return OperationResult<CustomerBasket>.Fail("Basket not found");
            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethodRepository = unitOfWork.Repository<DeliveryMethod>();
                var deliveryMethod = await deliveryMethodRepository.GetAsync(basket.DeliveryMethodId.Value);
                if (deliveryMethod != null)
                {
                    shippingPrice = deliveryMethod.Cost;
                    basket.ShippingPrice = shippingPrice;
                }
            }
            if (basket.Items?.Count > 0)
            {
                var productRepository = unitOfWork.Repository<Core.Entities.Products.Product>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepository.GetAsync(item.Id);
                    if (product != null && item.Price != product.Price)
                        item.Price = product.Price;
                }
            }
            PaymentIntent paymentIntent;
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)((basket.Items.Sum(item => item.Quantity * item.Price) + shippingPrice) * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent = await paymentIntentService.CreateAsync(options); // integration with stripe
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)((basket.Items.Sum(item => item.Quantity * item.Price) + shippingPrice) * 100),
                };
                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);
            }
            await basketRepository.UpdateBasketAsync(basket);
            return OperationResult<CustomerBasket>.Success(basket);
        }

        public async Task<OperationResult<Order>> UpdateOrderStatus(UpdateOrderStatusCommand command)
        {
            var orderRepository = unitOfWork.Repository<Order>();
            var spec = new OrderWithPaymentIntentSpecifications(command.PaymentIntentId);
            var order = await orderRepository.GetWithSpecAsync(spec);
            if (order is null) return OperationResult<Order>.Fail("Order not found");
            if (command.IsPaid)
                order.Status = OrderStatus.PaymentReceived;
            else
                order.Status = OrderStatus.PaymentFailed;
            orderRepository.Update(order);
            await unitOfWork.CompleteAsync();
            return OperationResult<Order>.Success(order);
        }
    }
}
