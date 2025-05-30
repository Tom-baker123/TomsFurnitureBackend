using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface ISupplierService
    {
        // Lấy danh sách tất cả nhà cung cấp
        Task<List<SupplierGetVModel>> GetAllAsync();

        // Lấy nhà cung cấp theo ID
        Task<SupplierGetVModel?> GetByIdAsync(int id);

        // Tạo mới nhà cung cấp
        Task<ResponseResult> CreateAsync(SupplierCreateVModel model, string? imageUrl);

        // Xóa nhà cung cấp
        Task<ResponseResult> DeleteAsync(int id);

        // Cập nhật nhà cung cấp
        Task<ResponseResult> UpdateAsync(SupplierUpdateVModel model, string? imageUrl = null);
    }
}
