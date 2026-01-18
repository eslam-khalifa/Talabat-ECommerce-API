using Talabat.APIs.Controllers;
using LinkDev.Talabat.Core.Commands;
using AutoMapper;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkDev.Talabat.APIs.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(
            IOrderService orderService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize]
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
        {
            var address = _mapper.Map<AddressDto, Core.Entities.Order_Aggregate.Address>(orderDto.ShippingAddress);
            var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var command = new CreateOrderCommand(email, orderDto.BasketId, orderDto.DeliveryMethodId, address);
            var result = await _orderService.CreateOrderAsync(command);
            if(!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(_mapper.Map<Order, OrderToReturnDto>(result.Data));
        }

        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var result = await _orderService.GetOrdersForUserAsync(email);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(result.Data));
        }

        [Authorize]
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var result = await _orderService.GetOrderByIdForUserAsync(email, id);
            if (!result.IsSuccess) return NotFound(new ApiErrorResponse(404, result.Errors));
            return Ok(_mapper.Map<Order, OrderToReturnDto>(result.Data));
        }

        [ProducesResponseType(typeof(IReadOnlyList<DeliveryMethod>), StatusCodes.Status200OK)]
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var result = await _orderService.GetDeliveryMethodsAsync();
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(result.Data);
        }
    }
}
