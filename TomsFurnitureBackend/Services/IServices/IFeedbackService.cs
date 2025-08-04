using OA.Domain.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IFeedbackService
    {
        Task<List<FeedbackGetVModel>> GetAllAsync();
        Task<FeedbackGetVModel?> GetByIdAsync(int id);
        Task<ResponseResult> CreateAsync(FeedbackCreateVModel model, HttpContext httpContext);
        Task<ResponseResult> DeleteAsync(int id);
        Task<ResponseResult> UpdateAsync(FeedbackUpdateVModel model, HttpContext httpContext);
    }
}