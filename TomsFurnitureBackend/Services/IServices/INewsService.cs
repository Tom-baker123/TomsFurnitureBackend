using Microsoft.AspNetCore.Http;
using OA.Domain.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface INewsService
    {
        Task<List<NewsGetVModel>> GetAllAsync();
        Task<NewsGetVModel?> GetByIdAsync(int id);
        Task<ResponseResult> CreateAsync(NewsCreateVModel model, IFormFile? imageFile, HttpContext httpContext);
        Task<ResponseResult> UpdateAsync(NewsUpdateVModel model, IFormFile? imageFile, HttpContext httpContext);
        Task<ResponseResult> DeleteAsync(int id);
    }
}