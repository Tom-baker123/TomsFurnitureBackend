using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TomsFurnitureBackend.Helpers
{
    public class CloudinaryHelper
    {
        // Hàm xử lý upload ảnh cho Slider
        public static async Task<string?> HandleSliderImageUpload(Cloudinary cloudinary, IFormFile imageFile, ILogger? logger = null)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                logger?.LogError("Unsupported file type: {FileName}", imageFile.FileName);
                throw new ArgumentException("Unsupported file type");
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
            {
                logger?.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl.AbsoluteUri;
        }

        // Hàm trích xuất publicId từ URL Cloudinary
        public static string? ExtractPublicIdFromUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return null;
                var uri = new Uri(url);
                var path = uri.AbsolutePath;
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2)
                {
                    var lastSegment = segments[^1];
                    return Path.GetFileNameWithoutExtension(lastSegment);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
