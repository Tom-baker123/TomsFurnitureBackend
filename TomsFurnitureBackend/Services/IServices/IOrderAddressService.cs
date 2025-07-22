using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IOrderAddressService
    {
        Task<List<OrderAddressGetVModel>> GetAllAsync(int? userId = null, bool? isDeafaultAddress = null);
        Task<OrderAddressGetVModel?> GetByIdAsync(int id);
        Task<ResponseResult> CreateAsync(OrderAddressCreateVModel model);
        Task<ResponseResult> UpdateAsync(OrderAddressUpdateVModel model);
        Task<ResponseResult> DeleteAsync(int id);
    }
}
