using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IUnitService
    {
        Task<List<UnitGetVModel>> GetAllAsync(); // Lấy tất cả sản phẩm
        Task<UnitGetVModel?> GetByIdAsync(int id); // Lấy sản phẩm theo ID ? đại diện cho biến lấy ID
        Task<ResponseResult> CreateAsync(UnitCreateVModel model); // Tạo sản phẩm mới
        Task<ResponseResult> UpdateAsync(UnitUpdateVModel model); // Cập nhật sản phẩm
        Task<ResponseResult> DeleteAsync(int id); // Xóa sản phẩm
    }
}
