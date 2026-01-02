using AutoMapper;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.Core.Entities.Basket;
using LinkDev.Talabat.Core.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Controllers;

namespace LinkDev.Talabat.APIs.Controllers
{
    public class BasketController(
        IBasketRepository basketRepository,
        IMapper mapper) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await basketRepository.GetBasketAsync(id);
            if (basket is null) return new CustomerBasket(id);
            else return basket;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto customerBasketDto)
        {
            var mappedBasket = mapper.Map<CustomerBasket>(customerBasketDto);
            var createdOrUpdatedBasket = await basketRepository.UpdateBasketAsync(mappedBasket);
            if (createdOrUpdatedBasket is null) return BadRequest(new ApiErrorResponse(400, "Failed to update basket"));
            return Ok(createdOrUpdatedBasket);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            return await basketRepository.DeleteBasketAsync(id);
        }
    }
}
