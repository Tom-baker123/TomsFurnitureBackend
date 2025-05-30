using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IPaymentMethodService
    {
        // Lấy danh sách tất cả phương thức thanh toán
        Task<List<PaymentMethodGetVModel>> GetAllAsync();

        // Lấy phương thức thanh toán theo ID
        Task<PaymentMethodGetVModel?> GetByIdAsync(int id);

        // Tạo mới phương thức thanh toán
        Task<ResponseResult> CreateAsync(PaymentMethodCreateVModel model);

        // Xóa phương thức thanh toán
        Task<ResponseResult> DeleteAsync(int id);

        // Cập nhật phương thức thanh toán
        Task<ResponseResult> UpdateAsync(PaymentMethodUpdateVModel model);
    }
}
