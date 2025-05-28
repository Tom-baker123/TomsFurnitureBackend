using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IMaterialService
    {
        // Lấy danh sách tất cả vật liệu
        Task<List<MaterialGetVModel>> GetAllAsync();
        // Lấy vật liệu theo ID
        Task<MaterialGetVModel?> GetByIdAsync(int id);
        // Tạo mới vật liệu
        Task<ResponseResult> CreateAsync(MaterialCreateVModel model);
        // Xóa vật liệu
        Task<ResponseResult> DeleteAsync(int id);
        // Cập nhật vật liệu
        Task<ResponseResult> UpdateAsync(MaterialUpdateVModel model);
    }
}
