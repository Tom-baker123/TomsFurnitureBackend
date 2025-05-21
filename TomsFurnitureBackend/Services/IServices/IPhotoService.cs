namespace TomsFurnitureBackend.Services.IServices
{
    public interface IPhotoService
    {
        Task<string?> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string publicId);
    }

}
