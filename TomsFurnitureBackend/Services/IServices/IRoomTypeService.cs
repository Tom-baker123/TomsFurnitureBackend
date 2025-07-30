using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IRoomTypeService
    {
        // Lấy danh sách tất cả loại phòng
        Task<List<RoomTypeGetVModel>> GetAllAsync();
        // Lấy loại phòng theo ID
        Task<RoomTypeGetVModel?> GetByIdAsync(int id);
        // Tạo mới loại phòng
        Task<ResponseResult> CreateAsync(RoomTypeCreateVModel model, string imageUrl);
        // Xóa loại phòng
        Task<ResponseResult> DeleteAsync(int id);
        // Cập nhật loại phòng
        Task<ResponseResult> UpdateAsync(RoomTypeUpdateVModel model, string? imageUrl = null);
    }
}
