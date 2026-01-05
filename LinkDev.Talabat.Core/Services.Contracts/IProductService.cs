using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Products;
using LinkDev.Talabat.Core.Specifications.ProductSpecs;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IProductService
    {
        Task<OperationResult<Product>> CreateProductAsync(string currentUserId, Product product, byte[]? fileBytes, string? fileName);
        Task<OperationResult<bool>> DeleteProductAsync(int id, string currentUserId, IList<string> currentUserRoles);
        Task<OperationResult<IReadOnlyList<Product>>> GetProductsAsync(ProductSpecParams productSpecParams);
        Task<OperationResult<Product>> GetProductAsync(int productId);
        Task<OperationResult<IReadOnlyList<ProductBrand>>> GetProductBrandsAsync();
        Task<OperationResult<IReadOnlyList<ProductCategory>>> GetProductCategoriesAsync();
        Task<OperationResult<int>> GetCountAsync(ProductSpecParams productSpecParams);
        Task<OperationResult<Product>> UpdateProductAsync(int id, Product product, byte[] fileBytes, string fileName, string currentUserId, IList<string> currentUserRoles);
    }
}
