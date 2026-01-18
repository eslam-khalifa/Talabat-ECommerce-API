using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Specifications;
using LinkDev.Talabat.Core.Specifications.UserSpecs;
using LinkDev.Talabat.Core.Commands;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IUserService
    {
        Task<OperationResult<ApplicationUser>> GetUserAsync(string userId);
        Task<OperationResult<IReadOnlyList<ApplicationUser>>> GetUsersAsync(UserSpecParams userSpecParams);
        Task<OperationResult<bool>> DeleteUserAsync(DeleteUserCommand command);
        Task<OperationResult<int>> CountUsersAsync(UserSpecParams userSpecParams);

    }
}
