using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IColorService
    {
        // Lấy danh sách tất cả màu săc
        Task<List<ColorGetVModel>> GetAllAsync();
        // Lấy màu săc theo ID
        Task<ColorGetVModel?> GetByIdAsync(int id);
        // Tạo mới màu săc
        Task<ResponseResult> CreateAsync(ColorCreateVModel model);
        // Xóa màu săc
        Task<ResponseResult> DeleteAsync(int id);
        // Cập nhật màu săc
        Task<ResponseResult> UpdateAsync(ColorUpdateVModel model);
    }
}
