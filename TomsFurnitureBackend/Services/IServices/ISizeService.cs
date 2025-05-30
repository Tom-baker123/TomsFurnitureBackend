using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface ISizeService
    {
        // Lấy danh sách tất cả kích thước
        Task<List<SizeGetVModel>> GetAllAsync();

        // Lấy kích thước theo ID
        Task<SizeGetVModel?> GetByIdAsync(int id);

        // Tạo mới kích thước
        Task<ResponseResult> CreateAsync(SizeCreateVModel model);

        // Xóa kích thước
        Task<ResponseResult> DeleteAsync(int id);

        // Cập nhật kích thước
        Task<ResponseResult> UpdateAsync(SizeUpdateVModel model);
    }
}
