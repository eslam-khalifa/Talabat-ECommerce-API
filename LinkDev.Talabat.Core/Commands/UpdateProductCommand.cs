using LinkDev.Talabat.Core.Entities.Products;
using System.Collections.Generic;

namespace LinkDev.Talabat.Core.Commands
{
    public class UpdateProductCommand
    {
        public int Id { get; }
        public Product Product { get; }
        public byte[]? FileBytes { get; }
        public string? FileName { get; }
        public string CurrentUserId { get; }
        public IList<string> CurrentUserRoles { get; }

        public UpdateProductCommand(int id, Product product, byte[]? fileBytes, string? fileName, string currentUserId, IList<string> currentUserRoles)
        {
            Id = id;
            Product = product;
            FileBytes = fileBytes;
            FileName = fileName;
            CurrentUserId = currentUserId;
            CurrentUserRoles = currentUserRoles;
        }
    }
}
