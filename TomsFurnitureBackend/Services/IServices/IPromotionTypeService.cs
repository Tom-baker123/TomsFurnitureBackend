using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IPromotionTypeService
    {
        // Lấy danh sách tất cả loại khuyến mãi
        Task<List<PromotionTypeGetVModel>> GetAllAsync();

        // Lấy loại khuyến mãi theo ID
        Task<PromotionTypeGetVModel?> GetByIdAsync(int id);

        // Tạo mới loại khuyến mãi
        Task<ResponseResult> CreateAsync(PromotionTypeCreateVModel model, string createdBy);

        // Cập nhật loại khuyến mãi
        Task<ResponseResult> UpdateAsync(PromotionTypeUpdateVModel model, string updatedBy);

        // Xóa loại khuyến mãi
        Task<ResponseResult> DeleteAsync(int id);
    }
}