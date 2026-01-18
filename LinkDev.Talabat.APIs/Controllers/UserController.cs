using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Controllers;
using LinkDev.Talabat.Core.Commands;
using AutoMapper;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.APIs.Helpers;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Core.Specifications.UserSpecs;
using Microsoft.AspNetCore.Identity;

namespace LinkDev.Talabat.APIs.Controllers
{
    public class UserController(IUserService userService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper) : BaseApiController
    {
        [HttpGet("{userId}")]
        public async Task<ActionResult<ApplicationUserToReturnDto>> GetUserAsync(string userId)
        {
            var result = await userService.GetUserAsync(userId);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            var userRoles = await userManager.GetRolesAsync(result.Data);
            var applicationUserToReturnDto = mapper.Map<ApplicationUserToReturnDto>(result.Data);
            applicationUserToReturnDto.Roles = userRoles;
            return result.IsSuccess
                ? Ok(applicationUserToReturnDto)
                : BadRequest(new ApiErrorResponse(400, result.Errors));
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ApplicationUserToReturnDto>>> GetUsersAsync([FromQuery] UserSpecParams userSpecParams)
        {
            var result = await userService.GetUsersAsync(userSpecParams);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            var applicationUserToReturnDtos = mapper.Map<IReadOnlyList<ApplicationUserToReturnDto>>(result.Data);
            foreach (var userDto in applicationUserToReturnDtos)
            {
                var user = result.Data.First(u => u.Id == userDto.Id);
                var roles = await userManager.GetRolesAsync(user);
                userDto.Roles = roles.ToList();
            }
            var countResult = await userService.CountUsersAsync(userSpecParams);
            if (!countResult.IsSuccess) return BadRequest(new ApiErrorResponse(400, countResult.Errors));
            return result.IsSuccess
                ? Ok(new Pagination<ApplicationUserToReturnDto>(userSpecParams.PageIndex, userSpecParams.PageSize, countResult.Data, applicationUserToReturnDtos))
                : BadRequest(new ApiErrorResponse(400, result.Errors));
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<bool>> DeleteUserAsync(string userId)
        {
            var command = new DeleteUserCommand(userId);
            var result = await userService.DeleteUserAsync(command);
            return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(new ApiErrorResponse(400, result.Errors));
        }
    }
}
