using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface ICountryService
    {
        // Lấy danh sách tất cả xuất xứ
        Task<List<CountryGetVModel>> GetAllAsync();

        // Lấy xuất xứ theo ID
        Task<CountryGetVModel?> GetByIdAsync(int id);

        // Tạo mới xuất xứ
        Task<ResponseResult> CreateAsync(CountryCreateVModel model, string? imageUrl);

        // Xóa xuất xứ
        Task<ResponseResult> DeleteAsync(int id);

        // Cập nhật xuất xứ
        Task<ResponseResult> UpdateAsync(CountryUpdateVModel model, string? imageUrl = null);
    }
}
