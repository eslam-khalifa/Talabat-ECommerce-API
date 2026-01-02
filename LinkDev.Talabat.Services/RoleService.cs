using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace LinkDev.Talabat.Application
{
    public class RoleService(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager) : IRoleService
    {
        public async Task<OperationResult<string>> AssignRoleAsync(ApplicationUser applicationUser, string role, IList<string> actorRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                return OperationResult<string>.Fail($"Role '{role}' does not exist");

            var currentUserRoles = await userManager.GetRolesAsync(applicationUser);

            if (role == "Admin" && !actorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can assign Admin role");

            if ((role == "Vendor" || role == "Delivery") && actorRoles.Contains("Admin") && actorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only Admin or SuperAdmin can assign Vendor/Delivery roles");

            if (currentUserRoles.Contains(role))
                return OperationResult<string>.Fail($"User already has role '{role}'");

            var result = await userManager.AddToRoleAsync(applicationUser, role);
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return OperationResult<string>.Success($"Role '{role}' assigned to '{applicationUser}'");
        }

        public async Task<OperationResult<string>> RemoveRoleFromUserAsync(ApplicationUser applicationUser, string role, IList<string> actorRoles)
        {
            var currentUserRoles = await userManager.GetRolesAsync(applicationUser);
            if (!currentUserRoles.Contains(role))
                return OperationResult<string>.Fail($"User does not have role '{role}'");

            if (role == "Admin" && !actorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can remove Admin role");

            var result = await userManager.RemoveFromRoleAsync(applicationUser, role);
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return OperationResult<string>.Success($"Role '{role}' removed from '{applicationUser}'");
        }

        public async Task<OperationResult<string>> CreateRoleAsync(string roleName, string descritpion, IList<string> actorRoles)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return OperationResult<string>.Fail("Role name is required");

            if (await roleManager.RoleExistsAsync(roleName))
                return OperationResult<string>.Fail($"Role '{roleName}' already exists");

            if (roleName == "Admin" && !actorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can create Admin role");

            var result = await roleManager.CreateAsync(new ApplicationRole(roleName, descritpion));
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return OperationResult<string>.Success($"Role '{roleName}' created successfully");
        }

        public async Task<OperationResult<string>> DeleteRoleAsync(string roleName, IList<string> actorRoles)
        {
            if (!actorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can delete roles");

            if (string.IsNullOrWhiteSpace(roleName))
                return OperationResult<string>.Fail("Role name is required");

            roleName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(roleName.ToLower());
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

        public async Task<OperationResult<string>> UpdateRoleAsync(string currentRoleName, string newRoleName, string newDescription, IList<string> actorRoles)
        {
            if (!actorRoles.Contains("SuperAdmin"))
                return OperationResult<string>.Fail("Only SuperAdmin can update roles");

            if (string.IsNullOrWhiteSpace(currentRoleName))
                return OperationResult<string>.Fail("Current role name is required");

            var roleToUpdate = await roleManager.FindByNameAsync(currentRoleName);
            if (roleToUpdate == null)
                return OperationResult<string>.Fail($"Role '{currentRoleName}' not found");

            bool nameChanged = !string.IsNullOrWhiteSpace(newRoleName) && newRoleName != currentRoleName;
            bool descriptionChanged = !string.IsNullOrWhiteSpace(newDescription) && newDescription != roleToUpdate.Description;

            if (!nameChanged && !descriptionChanged)
                return OperationResult<string>.Fail("No changes detected. Provide a new name or description.");

            if (nameChanged)
            {
                if (await roleManager.RoleExistsAsync(newRoleName))
                    return OperationResult<string>.Fail($"Role '{newRoleName}' already exists");
                roleToUpdate.Name = newRoleName;
                roleToUpdate.NormalizedName = roleManager.NormalizeKey(newRoleName);
            }

            if (descriptionChanged)
            {
                roleToUpdate.Description = newDescription;
            }

            // RoleManager.UpdateAsync is necessary to persist changes to the role in the underlying store.
            // Simply modifying properties of 'roleToUpdate' without calling UpdateAsync will not save changes.
            var result = await roleManager.UpdateAsync(roleToUpdate);
            if (!result.Succeeded) return OperationResult<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            string successMessage = "";
            if (nameChanged && descriptionChanged)
            {
                successMessage = $"Role '{currentRoleName}' name updated to '{roleToUpdate.Name}' and description updated successfully.";
            }
            else if (nameChanged)
            {
                successMessage = $"Role '{currentRoleName}' name updated to '{roleToUpdate.Name}' successfully.";
            }
            else if (descriptionChanged)
            {
                successMessage = $"Role '{currentRoleName}' description updated successfully.";
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
