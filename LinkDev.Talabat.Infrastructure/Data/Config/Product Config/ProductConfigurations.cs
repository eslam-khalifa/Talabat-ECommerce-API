using LinkDev.Talabat.Core.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDev.Talabat.Infrastructure.Data.Config.Product_Config
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.Price).IsRequired().HasPrecision(18, 2);
            builder.Property(p => p.PictureUrl).IsRequired().HasMaxLength(1000);
            builder.HasOne(p => p.Category).WithMany().HasForeignKey(p => p.CategoryId);
            builder.HasOne(p => p.Brand).WithMany().HasForeignKey(p => p.BrandId);
        }
    }
}
