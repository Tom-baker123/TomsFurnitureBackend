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
    }
}
