using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IBrandService
    {
        // Lấy danh sách tất cả Thương hiệu
        Task<List<BrandGetVModel>> GetAllAsync();
        // Lấy Thương hiệu theo ID
        Task<BrandGetVModel?> GetByIdAsync(int id);
        // Tạo mới Thương hiệu
        Task<ResponseResult> CreateAsync(BrandCreateVModel model, string imageUrl);
        // Xóa Thương hiệu
        Task<ResponseResult> DeleteAsync(int id);
        // Cập nhật Thương hiệu
        Task<ResponseResult> UpdateAsync(BrandUpdateVModel model, string? imageUrl = null);
    }
}
