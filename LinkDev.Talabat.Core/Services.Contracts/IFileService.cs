using LinkDev.Talabat.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Services.Contracts
{
    // Interface should define behavior not helpers.
    public interface IFileService<T> where T : BaseEntity
    {
        bool DeleteFile(string fileNameWithExtension);
        bool FileExists(string fileNameWithExtension);
        Task<string?> SaveFileAsync(byte[] fileBytes, string fileName, string[] allowedFileExtensions);
    }
}
