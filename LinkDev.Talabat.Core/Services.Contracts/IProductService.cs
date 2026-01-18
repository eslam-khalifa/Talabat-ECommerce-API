using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Products;
using LinkDev.Talabat.Core.Specifications.ProductSpecs;
using LinkDev.Talabat.Core.Commands;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IProductService
    {
        Task<OperationResult<Product>> CreateProductAsync(CreateProductCommand command);
        Task<OperationResult<bool>> DeleteProductAsync(DeleteProductCommand command);
        Task<OperationResult<IReadOnlyList<Product>>> GetProductsAsync(ProductSpecParams productSpecParams);
        Task<OperationResult<Product>> GetProductAsync(int productId);
        Task<OperationResult<IReadOnlyList<ProductBrand>>> GetProductBrandsAsync();
        Task<OperationResult<IReadOnlyList<ProductCategory>>> GetProductCategoriesAsync();
        Task<OperationResult<int>> GetCountAsync(ProductSpecParams productSpecParams);
        Task<OperationResult<Product>> UpdateProductAsync(UpdateProductCommand command);
    }
}
