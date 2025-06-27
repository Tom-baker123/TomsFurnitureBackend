using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IBannerService
    {
        // Lấy danh sách tất cả banner
        Task<List<BannerGetVModel>> GetAllAsync();
        // Lấy banner theo ID
        Task<BannerGetVModel?> GetByIdAsync(int id);
        // Tạo mới banner
        Task<ResponseResult> CreateAsync(BannerCreateVModel model, string imageUrl, string imageUrlMobile);
        // Xóa banner
        Task<ResponseResult> DeleteAsync(int id);
        // Cập nhật banner
        Task<ResponseResult> UpdateAsync(BannerUpdateVModel model, string? imageUrl = null, string? imageUrlMobile = null);
    }
}