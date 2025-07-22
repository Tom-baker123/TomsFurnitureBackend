using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IPromotionService
    {
        // Lấy danh sách tất cả khuyến mãi
        Task<List<PromotionGetVModel>> GetAllAsync(decimal? total = null);
        // Lấy khuyến mãi theo ID
        Task<PromotionGetVModel?> GetByIdAsync(int id);
        // Tạo mới khuyến mãi
        Task<ResponseResult> CreateAsync(PromotionCreateVModel model, string createdBy);
        // Cập nhật khuyến mãi
        Task<ResponseResult> UpdateAsync(PromotionUpdateVModel model, string updatedBy);
        // Xóa khuyến mãi
        Task<ResponseResult> DeleteAsync(int id);
    }
}