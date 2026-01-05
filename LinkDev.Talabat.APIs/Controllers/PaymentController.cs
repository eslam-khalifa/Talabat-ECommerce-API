using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.Core.Entities.Basket;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Controllers;

namespace LinkDev.Talabat.APIs.Controllers
{
    public class PaymentController(IPaymentService paymentService,
        ILogger<PaymentController> logger) : BaseApiController
    {
        private const string whSecret = "whsec_a7da649fadef2d7b153ae5b44c7edebe747c799f440c72fee54d46e3fbe33de2";

        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket is null) return BadRequest(new ApiErrorResponse(400, "An Error with your Basket."));
            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, whSecret, 300, false);

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    await paymentService.UpdateOrderStatus(paymentIntent.Id, true);
                    logger.LogInformation("handled webhook event: {EventId}", stripeEvent.Id);
                    break;
                case EventTypes.PaymentIntentPaymentFailed:
                    await paymentService.UpdateOrderStatus(paymentIntent.Id, false);
                    logger.LogInformation("handled webhook event: {EventId}", stripeEvent.Id);
                    break;
                default:
                    Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                    break;
            }
                
            return Ok();
        }
    }
}
