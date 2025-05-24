using Microsoft.AspNetCore.Http;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.Interfaces
{
    public interface ISliderService
    {
        
        Task<List<SliderGetVModel>> GetAllAsync(); // Phương thức lấy tất cả Slider
        Task<SliderGetVModel>? GetByIdAsync(int id); // Phương thức lấy theo ID của Slider
        Task<ResponseResult> Create(SliderCreateVModel model, string imageUrl); // Phương thức tạo Slider
        Task<ResponseResult> DeleteAsync(int id);
        Task<ResponseResult> Update(SliderUpdateVModel model);

    }
}
