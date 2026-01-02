using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Services.Contracts;
using Microsoft.AspNetCore.Hosting;

namespace LinkDev.Talabat.Application
{
    public class FileService<T>(IWebHostEnvironment webHostEnvironment) : IFileService<T> where T : BaseEntity
    {
        public bool DeleteFile(string fileNameWithExtension)
        {
            var folderName = GetFolderName<T>();
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", folderName, fileNameWithExtension);
            if (!File.Exists(filePath)) return false;
            File.Delete(filePath);
            return true;
        }

        public bool FileExists(string fileNameWithExtension)
        {
            var folderName = GetFolderName<T>();
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", folderName, fileNameWithExtension);
            return File.Exists(filePath);
        }

        public async Task<string?> SaveFileAsync(byte[] fileBytes, string fileName, string[] allowedFileExtensions)
        {
            const long MaxFileSize = 2 * 1024 * 1024;
            if (fileBytes.Length > MaxFileSize) return null;

            if (fileBytes is null || fileBytes.Length == 0 || fileName is null || allowedFileExtensions is null) return null;

            var folderName = GetFolderName<T>();
            var fileExtension = Path.GetExtension(fileName).ToLower();
            if (!allowedFileExtensions.Contains(fileExtension)) return null;

            string entityTypeName = typeof(T).Name.ToLower();
            var folderPath = Path.Combine(webHostEnvironment.WebRootPath, "images", folderName);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            return $"images/{folderName}/{uniqueFileName}";
        }

        private string GetFolderName<TEntity>()
        {
            return typeof(TEntity).Name switch
            {
                "Product" => "products",
                "Category" => "categories",
                "Brand" => "brands",
                _ => "others"
            };
        }
    }
}
