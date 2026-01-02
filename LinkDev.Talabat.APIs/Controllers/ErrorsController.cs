using LinkDev.Talabat.APIs.Errors;
using Microsoft.AspNetCore.Mvc;

namespace LinkDev.Talabat.APIs.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            if(code == 401)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            else if(code == 404)
                return NotFound(new ApiErrorResponse(404, "Resource not found"));
            else if(code == 400)
                return BadRequest(new ApiErrorResponse(400, "Bad request"));
            else return StatusCode(code, new ApiErrorResponse(code, $"Error {code}"));
        }
    }
}
