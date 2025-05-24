using Microsoft.AspNetCore.Http;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.Interfaces
{
    public interface ISliderService
    {
        
        Task<List<SliderGetVModel>> GetAllAsync(); // Lấy tất cả Slider
        Task<SliderGetVModel>? GetByIdAsync(int id); // Lấy theo Slider theo ID 
        Task<ResponseResult> CreateAsync(SliderCreateVModel model, string imageUrl); // Tạo Slider
        Task<ResponseResult> DeleteAsync(int id); // Xóa Slider
        Task<ResponseResult> UpdateAsync(SliderUpdateVModel model, string? imageUrl = null); // Cập nhật Slider nếu không có tham số là null
    }
}
