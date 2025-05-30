using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services.Interfaces
{
    public interface IRoleService
    {
        // Lấy danh sách tất cả vai trò
        Task<List<RoleGetVModel>> GetAllAsync();

        // Lấy vai trò theo ID
        Task<RoleGetVModel?> GetByIdAsync(int id);

        // Tạo mới vai trò
        Task<ResponseResult> CreateAsync(RoleCreateVModel model);

        // Xóa vai trò
        Task<ResponseResult> DeleteAsync(int id);

        // Cập nhật vai trò
        Task<ResponseResult> UpdateAsync(RoleUpdateVModel model);
    }
}