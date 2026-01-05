using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;
using System.Security.Claims;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IRoleService
    {
        Task<OperationResult<string>> AssignRoleAsync(ApplicationUser applicationUser, string role, IList<string> actorRoles);
        Task<OperationResult<string>> RemoveRoleFromUserAsync(ApplicationUser applicationUser, string role, IList<string> actorRoles);
        Task<OperationResult<string>> CreateRoleAsync(string roleName, string description, IList<string> actorRoles);
        Task<OperationResult<string>> UpdateRoleAsync(string oldRoleName, string? newRoleName, string? newDescription, IList<string> actorRoles);
        Task<OperationResult<string>> DeleteRoleAsync(string roleName, IList<string> actorRoles);
        Task<OperationResult<IReadOnlyList<string>>> GetAllRolesAsync();
    }
}
