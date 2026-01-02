namespace LinkDev.Talabat.APIs.Extensions
{
    public static class FormFileExtensions
    {
        public static async Task<(byte[] fileBytes, string fileName)?> ToByteArrayAsync(this IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0) return null;

            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return (memoryStream.ToArray(), formFile.FileName);
        }
    }
}
