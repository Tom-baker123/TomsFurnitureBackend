using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IStoreInformationService
    {
        // Lấy danh sách tất cả thông tin cửa hàng
        Task<List<StoreInformationGetVModel>> GetAllAsync();

        // Lấy thông tin cửa hàng theo ID
        Task<StoreInformationGetVModel?> GetByIdAsync(int id);

        // Tạo mới thông tin cửa hàng
        Task<ResponseResult> CreateAsync(StoreInformationCreateVModel model, string? logoUrl);

        // Xóa thông tin cửa hàng
        Task<ResponseResult> DeleteAsync(int id);

        // Cập nhật thông tin cửa hàng
        Task<ResponseResult> UpdateAsync(StoreInformationUpdateVModel model, string? logoUrl = null);
    }
}
