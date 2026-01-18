using LinkDev.Talabat.Core.Entities.Products;
using System.Collections.Generic;

namespace LinkDev.Talabat.Core.Commands
{
    public class CreateProductCommand
    {
        public string CurrentUserId { get; }
        public Product Product { get; }
        public byte[]? FileBytes { get; }
        public string? FileName { get; }

        public CreateProductCommand(string currentUserId, Product product, byte[]? fileBytes, string? fileName)
        {
            CurrentUserId = currentUserId;
            Product = product;
            FileBytes = fileBytes;
            FileName = fileName;
        }
    }
}
