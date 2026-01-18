using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;
using System.Security.Claims;
using LinkDev.Talabat.Core.Commands;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IRoleService
    {
        Task<OperationResult<string>> AssignRoleAsync(AssignRoleCommand command);
        Task<OperationResult<string>> RemoveRoleFromUserAsync(RemoveRoleFromUserCommand command);
        Task<OperationResult<string>> CreateRoleAsync(CreateRoleCommand command);
        Task<OperationResult<string>> UpdateRoleAsync(UpdateRoleCommand command);
        Task<OperationResult<string>> DeleteRoleAsync(DeleteRoleCommand command);
        Task<OperationResult<IReadOnlyList<string>>> GetAllRolesAsync();
    }
}
