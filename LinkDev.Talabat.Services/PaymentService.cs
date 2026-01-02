using LinkDev.Talabat.Core;
using LinkDev.Talabat.Core.Entities.Basket;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Core.Specifications.OrderSpecs;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace LinkDev.Talabat.Application
{
    public class PaymentService(IConfiguration configuration,
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork) : IPaymentService
    {
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];
            var basket = await basketRepository.GetBasketAsync(basketId);
            if (basket is null) return null;
            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethodRepository = unitOfWork.Repository<DeliveryMethod>();
                var deliveryMethod = await deliveryMethodRepository.GetAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
                basket.ShippingPrice = shippingPrice;
            }
            if (basket.Items?.Count > 0)
            {
                var productRepository = unitOfWork.Repository<Core.Entities.Products.Product>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepository.GetAsync(item.Id);
                    if (item.Price != product.Price)
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
            return basket;
        }

        public async Task<Order?> UpdateOrderStatus(string paymentIntentId, bool isPaid)
        {
            var orderRepository = unitOfWork.Repository<Order>();
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);
            var order = await orderRepository.GetWithSpecAsync(spec);
            if (order is null) return null;
            if (isPaid)
                order.Status = OrderStatus.PaymentReceived;
            else
                order.Status = OrderStatus.PaymentFailed;
            orderRepository.Update(order);
            await unitOfWork.CompleteAsync();
            return order;
        }
    }
}
