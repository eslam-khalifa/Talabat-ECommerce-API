using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using LinkDev.Talabat.Core.Commands;

namespace LinkDev.Talabat.Application
{
    public class RoleService(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager) : IRoleService
    {
        public async Task<OperationResult<string>> AssignRoleAsync(AssignRoleCommand command)
        {
            if (!await roleManager.RoleExistsAsync(command.Role))
                return OperationResult<string>.Fail($"Role '{command.Role}' does not exist");

            var currentUserRoles = await userManager.GetRolesAsync(command.User);

            if (command.Role == "Admin" && !command.ActorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can assign Admin role");

            if ((command.Role == "Vendor" || command.Role == "Delivery") && command.ActorRoles.Contains("Admin") && command.ActorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only Admin or SuperAdmin can assign Vendor/Delivery roles");

            if (currentUserRoles.Contains(command.Role))
                return OperationResult<string>.Fail($"User already has role '{command.Role}'");

            var result = await userManager.AddToRoleAsync(command.User, command.Role);
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return OperationResult<string>.Success($"Role '{command.Role}' assigned to '{command.User}'");
        }

        public async Task<OperationResult<string>> RemoveRoleFromUserAsync(RemoveRoleFromUserCommand command)
        {
            var currentUserRoles = await userManager.GetRolesAsync(command.User);
            if (!currentUserRoles.Contains(command.Role))
                return OperationResult<string>.Fail($"User does not have role '{command.Role}'");

            if (command.Role == "Admin" && !command.ActorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can remove Admin role");

            var result = await userManager.RemoveFromRoleAsync(command.User, command.Role);
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return OperationResult<string>.Success($"Role '{command.Role}' removed from '{command.User}'");
        }

        public async Task<OperationResult<string>> CreateRoleAsync(CreateRoleCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.RoleName))
                return OperationResult<string>.Fail("Role name is required");

            if (await roleManager.RoleExistsAsync(command.RoleName))
                return OperationResult<string>.Fail($"Role '{command.RoleName}' already exists");

            if (command.RoleName == "Admin" && !command.ActorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can create Admin role");

            var result = await roleManager.CreateAsync(new ApplicationRole(command.RoleName, command.Description));
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return OperationResult<string>.Success($"Role '{command.RoleName}' created successfully");
        }

        public async Task<OperationResult<string>> DeleteRoleAsync(DeleteRoleCommand command)
        {
            if (!command.ActorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can delete roles");

            if (string.IsNullOrWhiteSpace(command.RoleName))
                return OperationResult<string>.Fail("Role name is required");

            var roleName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(command.RoleName.ToLower());
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
                return OperationResult<string>.Fail($"Role '{roleName}' does not exist");

            var usersInRole = await userManager.GetUsersInRoleAsync(roleName);
            if (usersInRole.Any())
                return OperationResult<string>.Fail($"Cannot delete role '{roleName}' as there are users assigned to it.");

            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return OperationResult<string>.Success($"Role '{roleName}' deleted successfully");
        }

        public async Task<OperationResult<string>> UpdateRoleAsync(UpdateRoleCommand command)
        {
            if (!command.ActorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can update roles");

            if (string.IsNullOrWhiteSpace(command.OldRoleName))
                return OperationResult<string>.Fail("Current role name is required");

            var roleToUpdate = await roleManager.FindByNameAsync(command.OldRoleName);
            if (roleToUpdate == null)
                return OperationResult<string>.Fail($"Role '{command.OldRoleName}' not found");

            bool nameChanged = !string.IsNullOrWhiteSpace(command.NewRoleName) && command.NewRoleName != command.OldRoleName;
            bool descriptionChanged = !string.IsNullOrWhiteSpace(command.NewDescription) && command.NewDescription != roleToUpdate.Description;

            if (!nameChanged && !descriptionChanged)
                return OperationResult<string>.Fail("No changes detected. Provide a new name or description.");

            if (nameChanged)
            {
                if (await roleManager.RoleExistsAsync(command.NewRoleName!))
                    return OperationResult<string>.Fail($"Role '{command.NewRoleName}' already exists");
                roleToUpdate.Name = command.NewRoleName;
                roleToUpdate.NormalizedName = roleManager.NormalizeKey(command.NewRoleName!);
            }

            if (descriptionChanged)
            {
                roleToUpdate.Description = command.NewDescription!;
            }

            // RoleManager.UpdateAsync is necessary to persist changes to the role in the underlying store.
            // Simply modifying properties of 'roleToUpdate' without calling UpdateAsync will not save changes.
            var result = await roleManager.UpdateAsync(roleToUpdate);
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            string successMessage = "";
            if (nameChanged && descriptionChanged)
            {
                successMessage = $"Role '{command.OldRoleName}' name updated to '{roleToUpdate.Name}' and description updated successfully.";
            }
            else if (nameChanged)
            {
                successMessage = $"Role '{command.OldRoleName}' name updated to '{roleToUpdate.Name}' successfully.";
            }
            else if (descriptionChanged)
            {
                successMessage = $"Role '{command.OldRoleName}' description updated successfully.";
            }

            return OperationResult<string>.Success(successMessage);
        }

        public async Task<OperationResult<IReadOnlyList<string>>> GetAllRolesAsync()
        {
            var roles = await Task.FromResult(roleManager.Roles.Select(r => r.Name).ToList());
            if (roles is null || roles.Count == 0) return OperationResult<IReadOnlyList<string>>.Fail("No roles found");
            return OperationResult<IReadOnlyList<string>>.Success(roles);
        }
    }
}
