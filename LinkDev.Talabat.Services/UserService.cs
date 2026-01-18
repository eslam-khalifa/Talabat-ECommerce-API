using LinkDev.Talabat.Core.Specifications.UserSpecs;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using LinkDev.Talabat.Core.Commands;
using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;

namespace LinkDev.Talabat.Application
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        public async Task<OperationResult<ApplicationUser>> GetUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return OperationResult<ApplicationUser>.Fail("User not found.");
            }
            return OperationResult<ApplicationUser>.Success(user);
        }

        public async Task<OperationResult<IReadOnlyList<ApplicationUser>>> GetUsersAsync(UserSpecParams userSpecParams)
        {
            var usersQuery = userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(userSpecParams.DisplayName))
            {
                usersQuery = usersQuery.Where(u =>
                    u.DisplayName.ToLower().Contains(userSpecParams.DisplayName.ToLower())
                );
            }

            if (!string.IsNullOrEmpty(userSpecParams.Sort))
            {
                switch (userSpecParams.Sort.ToLower())
                {
                    case "displaynamedesc":
                        usersQuery = usersQuery.OrderByDescending(u => u.DisplayName);
                        break;
                    default:
                        usersQuery = usersQuery.OrderBy(u => u.DisplayName);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(userSpecParams.Role))
            {
                // to convert the entered input to lower case then to pascal case as it is stored in the pascal case form in the database
                var roleName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(userSpecParams.Role.ToLower());

                var usersInRole = await userManager.GetUsersInRoleAsync(roleName);
                var userIdsInRole = usersInRole.Select(u => u.Id).ToList();

                usersQuery = usersQuery.Where(u => userIdsInRole.Contains(u.Id));
            }

            // let pagination be the last step
            var skip = (userSpecParams.PageIndex - 1) * userSpecParams.PageSize;
            var baseUsers = usersQuery.Skip(skip).Take(userSpecParams.PageSize).ToList();

            return OperationResult<IReadOnlyList<ApplicationUser>>.Success(baseUsers);
        }

        public async Task<OperationResult<bool>> DeleteUserAsync(DeleteUserCommand command)
        {
            var user = await userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return OperationResult<bool>.Fail("User not found.");
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return OperationResult<bool>.Fail(result.Errors.Select(e => e.Description).ToList());
            }

            return OperationResult<bool>.Success(true);
        }

        public async Task<OperationResult<int>> CountUsersAsync(UserSpecParams userSpecParams)
        {
            var usersQuery = userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(userSpecParams.DisplayName))
            {
                usersQuery = usersQuery.Where(u =>
                    u.DisplayName.ToLower().Contains(userSpecParams.DisplayName.ToLower())
                );
            }

            if (!string.IsNullOrEmpty(userSpecParams.Role))
            {
                var roleName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(userSpecParams.Role.ToLower());

                var usersInRole = await userManager.GetUsersInRoleAsync(roleName);
                var userIdsInRole = usersInRole.Select(u => u.Id).ToList();

                usersQuery = usersQuery.Where(u => userIdsInRole.Contains(u.Id));
            }

            return OperationResult<int>.Success(usersQuery.Count());
        }
    }
}
