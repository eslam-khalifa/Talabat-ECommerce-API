using LinkDev.Talabat.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace LinkDev.Talabat.Infrastructure.Identity
{
    public static class ApplicationIdentityDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            var applicationRoles = new[]
            {
                new ApplicationRole("SuperAdmin", "System super administrator with full access"),
                new ApplicationRole("Admin", "System administrator with full access"),
                new ApplicationRole("Vendor", "Store or restaurant owner who manages products and orders"),
                new ApplicationRole("Customer", "Customer who can browse and place orders"),
                new ApplicationRole("Delivery", "Delivery agent responsible for delivering orders")
            };

            if (!roleManager.Roles.Any())
            {
                foreach(var applicationRole in applicationRoles)
                {
                    await roleManager.CreateAsync(applicationRole);
                }
            }

            if (!userManager.Users.Any())
            {
                var applicationUsers = new List<(string DisplayName, string Email, string UserName, string Phone, string Role)>
                {
                    ("Eslam Khalifa", "khalifaeslam754@gmail.com", "eslamKhalifa", "01014121770", "SuperAdmin"),
                    ("Ahmed Ali", "ahmed.admin1.overreact919@aleeas.com", "ahmedAdmin1", "01010000001", "Admin"),
                    ("Sara Hassan", "sara.admin2.nimbly139@aleeas.com", "saraAdmin2", "01010000002", "Admin"),
                    ("Omar Said", "omar.delivery1.conductor929@aleeas.com", "omarDelivery1", "01010000003", "Delivery"),
                    ("Nour El-Sayed", "nour.delivery2.portly169@aleeas.com", "nourDelivery2", "01010000004", "Delivery"),
                    ("Mona Adel", "mona.customer1.stunner671@aleeas.com", "monaCustomer1", "01010000005", "Customer"),
                    ("Hany Mohamed", "hany.customer2.those086@aleeas.com", "hanyCustomer2", "01010000006", "Customer"),
                    ("Khaled Samir", "khaled.vendor3.maker886@aleeas.com", "khaledVendor3", "01010000009", "Customer"),
                    ("Tamer Nabil", "tamer.vendor1.unlined694@aleeas.com", "tamerVendor1", "01010000007", "Vendor"),
                    ("Laila Gamal", "laila.vendor2.hula998@aleeas.com", "lailaVendor2", "01010000008", "Vendor")
                };

                foreach (var applicationUser in applicationUsers)
                {
                    var user = new ApplicationUser
                    {
                        DisplayName = applicationUser.DisplayName,
                        Email = applicationUser.Email,
                        UserName = applicationUser.UserName,
                        PhoneNumber = applicationUser.Phone
                    };

                    var res = await userManager.CreateAsync(user, "Pa$$w0rd");
                    if (res.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, applicationUser.Role);
                    }
                }
            }
        }
    }
}
