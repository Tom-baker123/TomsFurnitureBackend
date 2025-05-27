using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface ICategoryService
    {
        // Lấy danh sách tất cả danh mục
        Task<List<CategoryGetVModel>> GetAllAsync();
        // Lấy danh mục theo ID
        Task<CategoryGetVModel?> GetByIdAsync(int id);
        // Tạo mới danh mục
        Task<ResponseResult> CreateAsync(CategoryCreateVModel model, string imageUrl);
        // Xóa danh mục
        Task<ResponseResult> DeleteAsync(int id);
        // Cập nhật danh mục
        Task<ResponseResult> UpdateAsync(CategoryUpdateVModel model, string? imageUrl = null); 
    }
}
