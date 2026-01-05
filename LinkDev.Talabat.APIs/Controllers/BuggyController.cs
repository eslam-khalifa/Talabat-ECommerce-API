using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Controllers;

namespace LinkDev.Talabat.APIs.Controllers
{
    public class BuggyController(StoreDbContext storeDbContext) : BaseApiController
    {
        [HttpGet("notfound")]
        public ActionResult GetNotFound()
        {
            var product = storeDbContext.Products.Find(-1);
            if (product is null)
                return NotFound(new ApiErrorResponse(404, "Item not found."));
            return Ok(product);
        }

        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var product = storeDbContext.Products.Find(-1);
            var productToReturn = product.ToString(); // This will throw a NullReferenceException
            return Ok(productToReturn);
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiErrorResponse(400, "This is a bad request."));
        }

        [HttpGet("badrequest/{id}")]
        public ActionResult GetBadRequest(int id) // Validation Error
        {
            return Ok();
        }

        [HttpGet("unauthorized")]
        public ActionResult GetUnauthorized()
        {
            return Unauthorized(new ApiErrorResponse(401, "You are not authorized."));
        }
    }
}
