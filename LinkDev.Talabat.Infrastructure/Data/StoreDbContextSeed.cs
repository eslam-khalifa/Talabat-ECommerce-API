using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using LinkDev.Talabat.Core.Entities.Products;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace LinkDev.Talabat.Infrastructure.Data
{
    public static class StoreDbContextSeed
    {
        public static async Task SeedAsync(StoreDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            if (!dbContext.ProductBrands.Any())
            {
                var brandsData = File.ReadAllText("../LinkDev.Talabat.Infrastructure/Data/DataSeed/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                await dbContext.ProductBrands.AddRangeAsync(brands);
            }
            if (!dbContext.ProductCategories.Any())
            {
                var categoriesData = File.ReadAllText("../LinkDev.Talabat.Infrastructure/Data/DataSeed/categories.json");
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesData);
                dbContext.ProductCategories.AddRange(categories);
            }
            if (!dbContext.Products.Any())
            {
                var productsData = File.ReadAllText("../LinkDev.Talabat.Infrastructure/Data/DataSeed/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                var vendors = await userManager.GetUsersInRoleAsync("Vendor");
                int counter = 0;
                foreach (var product in products)
                {
                    product.VendorId = vendors[counter % vendors.Count].Id;
                    counter++;
                }
                dbContext.Products.AddRange(products);
            }
            if (!dbContext.DeliveryMethods.Any())
            {
                var deliveryMethodsData = File.ReadAllText("../LinkDev.Talabat.Infrastructure/Data/DataSeed/delivery.json");
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
                dbContext.DeliveryMethods.AddRange(deliveryMethods);
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
