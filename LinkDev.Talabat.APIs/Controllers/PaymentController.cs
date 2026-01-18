using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.Core.Entities.Basket;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Controllers;
using LinkDev.Talabat.Core.Commands;

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
            var command = new CreateOrUpdatePaymentIntentCommand(basketId);
            var result = await paymentService.CreateOrUpdatePaymentIntent(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(result.Data);
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
                    var successCommand = new UpdateOrderStatusCommand(paymentIntent.Id, true);
                    await paymentService.UpdateOrderStatus(successCommand);
                    logger.LogInformation("handled webhook event: {EventId}", stripeEvent.Id);
                    break;
                case EventTypes.PaymentIntentPaymentFailed:
                    var failCommand = new UpdateOrderStatusCommand(paymentIntent.Id, false);
                    await paymentService.UpdateOrderStatus(failCommand);
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
