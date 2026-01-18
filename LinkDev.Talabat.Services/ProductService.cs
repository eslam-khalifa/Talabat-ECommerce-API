using LinkDev.Talabat.Core;
using LinkDev.Talabat.Core.Specifications.ProductSpecs;
using LinkDev.Talabat.Core.Commands;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Products;
using LinkDev.Talabat.Core.Services.Contracts;

namespace LinkDev.Talabat.Application
{
    public class ProductService(IUnitOfWork unitOfWork, IFileService<Product> fileService) : IProductService
    {
        // Don't forget to validate in CRUD operations
        public async Task<OperationResult<Product>> CreateProductAsync(CreateProductCommand command)
        {
            var product = command.Product;
            product.VendorId = command.CurrentUserId;

            if (command.FileBytes is null || string.IsNullOrEmpty(command.FileName)) return OperationResult<Product>.Fail("File bytes or file name is required.");

            var picturePath = await fileService.SaveFileAsync(command.FileBytes, command.FileName, new[] { ".png", ".jpg", ".jpeg" });
            if (picturePath is null) return OperationResult<Product>.Fail("Failed to save file.");
            product.PictureUrl = picturePath;

            var productRepository = unitOfWork.Repository<Product>();
            var categoryRepository = unitOfWork.Repository<ProductCategory>();
            var brandRepository = unitOfWork.Repository<ProductBrand>();

            if (product.Price <= 0) return OperationResult<Product>.Fail("Price must be greater than 0.");

            var categorySpecifications = new ProductCategorySpecifications(product.CategoryId);
            var category = await categoryRepository.GetWithSpecAsync(categorySpecifications, true);
            if (category is null) return OperationResult<Product>.Fail("Category not found.");
            product.Category = category;

            var brandSpecifications = new ProductBrandSpecifications(product.BrandId);
            var brand = await brandRepository.GetWithSpecAsync(brandSpecifications, true);
            if (brand is null) return OperationResult<Product>.Fail("Brand not found.");
            product.Brand = brand;

            productRepository.Add(product);
            var result = await unitOfWork.CompleteAsync();

            if (result == 0) return OperationResult<Product>.Fail("Failed to create product.");
            else return OperationResult<Product>.Success(product);
        }

        public async Task<OperationResult<bool>> DeleteProductAsync(DeleteProductCommand command)
        {
            var productRepository = unitOfWork.Repository<Product>();
            var existingProduct = await productRepository.GetAsync(command.Id);
            if (existingProduct is null) return OperationResult<bool>.Fail("Product not found.");
            if (!command.CurrentUserRoles.Contains("Admin") && !command.CurrentUserRoles.Contains("SuperAdmin") || command.CurrentUserRoles.Contains("Vendor") && command.CurrentUserId != existingProduct.VendorId) return OperationResult<bool>.Fail("You are not authorized to delete this product.");
            productRepository.Delete(existingProduct);
            var result = await unitOfWork.CompleteAsync();
            if (result == 0) return OperationResult<bool>.Fail("Failed to delete product.");
            else return OperationResult<bool>.Success(true);
        }

        public async Task<OperationResult<int>> GetCountAsync(ProductSpecParams productSpecParams)
        {
            var countSpec = new ProductWithFilterationForCountSpecifications(productSpecParams);
            var count = await unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            return OperationResult<int>.Success(count);
        }

        public async Task<OperationResult<Product>> GetProductAsync(int productId)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productId);
            var product = await unitOfWork.Repository<Product>().GetWithSpecAsync(spec);
            if (product is null) return OperationResult<Product>.Fail("Product not found.");
            return OperationResult<Product>.Success(product);
        }

        public async Task<OperationResult<IReadOnlyList<ProductBrand>>> GetProductBrandsAsync()
        {
            var brands = await unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return OperationResult<IReadOnlyList<ProductBrand>>.Success(brands);
        }

        public async Task<OperationResult<IReadOnlyList<ProductCategory>>> GetProductCategoriesAsync()
        {
            var categories = await unitOfWork.Repository<ProductCategory>().GetAllAsync();
            return OperationResult<IReadOnlyList<ProductCategory>>.Success(categories);
        }

        public async Task<OperationResult<IReadOnlyList<Product>>> GetProductsAsync(ProductSpecParams productSpecParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productSpecParams);
            var products = await unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            return OperationResult<IReadOnlyList<Product>>.Success(products);
        }

        public async Task<OperationResult<Product>> UpdateProductAsync(UpdateProductCommand command)
        {
            var product = command.Product;
            if (!command.CurrentUserRoles.Contains("Admin") && !command.CurrentUserRoles.Contains("SuperAdmin") || command.CurrentUserRoles.Contains("Vendor") && command.CurrentUserId != product.VendorId) return OperationResult<Product>.Fail("You are not authorized to update this product.");

            var productRepository = unitOfWork.Repository<Product>();
            var categoryRepository = unitOfWork.Repository<ProductCategory>();
            var brandRepository = unitOfWork.Repository<ProductBrand>();

            var existingProduct = await productRepository.GetAsync(command.Id); // this instance is tracked already be EFCore.
            if (existingProduct is null) return OperationResult<Product>.Fail("Product not found.");

            if (command.FileName != null && command.FileBytes != null)
            {
                var fileExists = fileService.FileExists(command.FileName);
                if (!fileExists)
                {
                    var picturePath = await fileService.SaveFileAsync(command.FileBytes, command.FileName, new[] { ".png", ".jpg", ".jpeg" });
                    if (picturePath is null) return OperationResult<Product>.Fail("Failed to save file.");
                    existingProduct.PictureUrl = picturePath;
                }
            }

            if (product.Price <= 0) return OperationResult<Product>.Fail("Price must be greater than 0.");
            existingProduct.Price = product.Price;

            var categorySpecifications = new ProductCategorySpecifications(product.CategoryId);
            var category = await categoryRepository.GetWithSpecAsync(categorySpecifications, true);
            if (category is null) return OperationResult<Product>.Fail("Category not found.");
            existingProduct.Category = category;

            var brandSpecifications = new ProductBrandSpecifications(product.BrandId);
            var brand = await brandRepository.GetWithSpecAsync(brandSpecifications, true);
            if (brand is null) return OperationResult<Product>.Fail("Brand not found.");
            existingProduct.Brand = brand;

            var result = await unitOfWork.CompleteAsync();

            if (result == 0) return OperationResult<Product>.Fail("Failed to update product.");
            else return OperationResult<Product>.Success(existingProduct);
        }
    }
}
