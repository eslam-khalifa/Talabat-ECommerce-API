using AutoMapper;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.APIs.Extensions;
using LinkDev.Talabat.APIs.Helpers;
using LinkDev.Talabat.Core.Entities.Products;
using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Core.Specifications.ProductSpecs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Talabat.APIs.Controllers
{
    public class ProductsController(
        IProductService productService,
        IAuthService authService,
        IMapper mapper) : BaseApiController
    {
        // controller checks if the input data is null or not
        [Authorize(Roles = "SuperAdmin, Admin, Vendor")]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ProductToReturnDto), 200)]
        [HttpPost]
        public async Task<ActionResult<ProductToReturnDto>> CreateProduct([FromForm]ProductDto productDto)
        {
            if (productDto.Picture is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "Picture is required"));
            var fileData = await productDto.Picture.ToByteArrayAsync();
            var product = mapper.Map<Product>(productDto);
            var currentUserId = (await authService.GetCurrentUserAsync(User)).Data.Id;
            var result = await productService.CreateProductAsync(currentUserId, product, fileData.Value.fileBytes, fileData.Value.fileName);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, result.Errors));
            else return Ok(mapper.Map<ProductToReturnDto>(result.Data));
        }

        [AllowAnonymous]
        [Cached(600)]
        [ProducesResponseType(typeof(Pagination<ProductToReturnDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productSpecParams)
        {
            var result = await productService.GetProductsAsync(productSpecParams);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, result.Errors));
            var productToReturnDtos = mapper.Map<IReadOnlyList<ProductToReturnDto>>(result.Data);
            var resultCount = await productService.GetCountAsync(productSpecParams);
            return Ok(new Pagination<ProductToReturnDto>(productSpecParams.PageIndex, productSpecParams.PageSize, resultCount.Data, productToReturnDtos));
        }

        [AllowAnonymous]
        [Cached(600)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ProductToReturnDto), 200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var result = await productService.GetProductAsync(id);
            if (!result.IsSuccess) return NotFound(new ApiErrorResponse(404, result.Errors));
            return Ok(mapper.Map<ProductToReturnDto>(result.Data));
        }

        [AllowAnonymous]
        [Cached(600)]
        [ProducesResponseType(typeof(IReadOnlyList<ProductBrand>), 200)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var result = await productService.GetProductBrandsAsync();
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, result.Errors));
            return Ok(result.Data);
        }

        [AllowAnonymous]
        [Cached(600)]
        [ProducesResponseType(typeof(IReadOnlyList<ProductCategory>), 200)]
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var result = await productService.GetProductCategoriesAsync();
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, result.Errors));
            return Ok(result.Data);
        }

        // if the client sends a null id, the framework will return a 400 bad request automatically.
        [Authorize(Roles = "SuperAdmin, Admin, Vendor")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ProductToReturnDto), 200)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> UpdateProductAsync(int id, [FromForm]ProductDto productDto)
        {
            if (productDto.Picture is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "Picture is required"));
            var fileData = await productDto.Picture.ToByteArrayAsync();
            var product = mapper.Map<Product>(productDto);
            var currentUserId = await authService.GetCurrentUserAsync(User);
            if (!currentUserId.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, currentUserId.Errors));
            var currentUserRoles = await authService.GetUserRolesAsync(User);
            if (!currentUserRoles.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, currentUserRoles.Errors));
            var result = await productService.UpdateProductAsync(id, product, fileData.Value.fileBytes, fileData.Value.fileName, currentUserId.Data.Id, currentUserRoles.Data);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, result.Errors));
            else return Ok(mapper.Map<ProductToReturnDto>(result.Data));
        }

        [Authorize(Roles = "SuperAdmin, Admin, Vendor")]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(bool), 200)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteProductAsync(int id)
        {
            var currentUserId = await authService.GetCurrentUserAsync(User);
            if (!currentUserId.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, currentUserId.Errors));
            var currentUserRoles = await authService.GetUserRolesAsync(User);
            if (!currentUserRoles.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, currentUserRoles.Errors));
            var result = await productService.DeleteProductAsync(id, currentUserId.Data.Id, currentUserRoles.Data);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, result.Errors));
            return Ok(result.Data);
        }
    }
}
