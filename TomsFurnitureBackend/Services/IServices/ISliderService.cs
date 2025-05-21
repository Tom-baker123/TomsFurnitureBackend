using Microsoft.AspNetCore.Http;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.Interfaces
{
    public interface ISliderService
    {
        Task<ResponseResult> Create(SliderCreateVModel model, string imageUrl);
        //  Task<ResponseResult> Update(int id, SliderUpdateVModel model);
        Task<ResponseResult> GetByIdAsync(int id);
    }
}
