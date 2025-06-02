using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface INewsService
    {
        // Lấy danh sách tất cả tin tức
        Task<List<NewsGetVModel>> GetAllAsync();
        // Lấy tin tức theo ID
        Task<NewsGetVModel?> GetByIdAsync(int id);
        // Tạo tin tức mới
        Task<ResponseResult> CreateAsync(NewsCreateVModel model, string? newsAvatar, string createdBy);
        // Cập nhật tin tức
        Task<ResponseResult> UpdateAsync(NewsUpdateVModel model, string? newsAvatar, string updatedBy);
        // Xóa tin tức
        Task<ResponseResult> DeleteAsync(int id);
    }
}