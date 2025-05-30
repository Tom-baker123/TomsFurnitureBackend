using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IFeedbackService
    {
        // Lấy danh sách tất cả phản hồi
        Task<List<FeedbackGetVModel>> GetAllAsync();

        // Lấy phản hồi theo ID
        Task<FeedbackGetVModel?> GetByIdAsync(int id);

        // Tạo mới phản hồi
        Task<ResponseResult> CreateAsync(FeedbackCreateVModel model);

        // Xóa phản hồi
        Task<ResponseResult> DeleteAsync(int id);

        // Cập nhật phản hồi
        Task<ResponseResult> UpdateAsync(FeedbackUpdateVModel model);
    }
}
