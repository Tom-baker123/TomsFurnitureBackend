using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IOrderStatusService
    {
        Task<List<OrderStatusGetVModel>> GetAllAsync();
        Task<OrderStatusGetVModel?> GetByIdAsync(int id);
        Task<ResponseResult> CreateAsync(OrderStatusCreateVModel model);
        Task<ResponseResult> UpdateAsync(OrderStatusUpdateVModel model);
        Task<ResponseResult> DeleteAsync(int id);
    }
}
