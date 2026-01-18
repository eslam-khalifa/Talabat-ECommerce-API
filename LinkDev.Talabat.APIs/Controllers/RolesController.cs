using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Controllers;
using LinkDev.Talabat.Core.Commands;
using LinkDev.Talabat.APIs.DTOs;
using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LinkDev.Talabat.APIs.Controllers
{
    // role management
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class RolesController(UserManager<ApplicationUser> userManager,
        IRoleService roleService) : BaseApiController
    {
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var actor = await userManager.GetUserAsync(User);
            if (actor is null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            var actorRoles = (await userManager.GetRolesAsync(actor));

            var userToAssign = await userManager.FindByEmailAsync(dto.UserEmail);
            if (userToAssign is null) return BadRequest(new ApiErrorResponse(400, "User to assign not found"));

            var command = new AssignRoleCommand(userToAssign, dto.Role, actorRoles);
            var result = await roleService.AssignRoleAsync(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(result.Data);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] AssignRoleDto dto)
        {
            var actor = await userManager.GetUserAsync(User);
            if (actor is null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            var actorRoles = (await userManager.GetRolesAsync(actor));

            var userToRemove = await userManager.FindByEmailAsync(dto.UserEmail);
            if (userToRemove is null) return BadRequest(new ApiErrorResponse(400, "User not found"));

            var command = new RemoveRoleFromUserCommand(userToRemove, dto.Role, actorRoles);
            var result = await roleService.RemoveRoleFromUserAsync(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(result.Data);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] ApplicationRoleCreateDto applicationRoleCreateDto)
        {
            var actor = await userManager.GetUserAsync(User);
            if (actor is null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            var actorRoles = (await userManager.GetRolesAsync(actor));

            var command = new CreateRoleCommand(applicationRoleCreateDto.Name, applicationRoleCreateDto.Description, actorRoles);
            var result = await roleService.CreateRoleAsync(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(result.Data);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<string>>> GetAllRoles()
        {
            var result = await roleService.GetAllRolesAsync();
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(result.Data);
        }

        [HttpDelete("{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var actor = await userManager.GetUserAsync(User);
            if (actor is null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            var actorRoles = (await userManager.GetRolesAsync(actor));

            var command = new DeleteRoleCommand(roleName, actorRoles);
            var result = await roleService.DeleteRoleAsync(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(result.Data);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRole([FromBody] ApplicationRoleUpdateDto applicationRoleUpdateDto)
        {
            var actor = await userManager.GetUserAsync(User);
            if (actor is null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            var actorRoles = (await userManager.GetRolesAsync(actor));
            if (actorRoles is null) return BadRequest(new ApiErrorResponse(400, "Actor roles not found"));

            var command = new UpdateRoleCommand(applicationRoleUpdateDto.OldName, applicationRoleUpdateDto.NewName, applicationRoleUpdateDto.NewDescription, actorRoles);
            var result = await roleService.UpdateRoleAsync(command);
            if (!result.IsSuccess) return BadRequest(new ApiErrorResponse(400, result.Errors));
            return Ok(result.Data);
        }
    }
}
