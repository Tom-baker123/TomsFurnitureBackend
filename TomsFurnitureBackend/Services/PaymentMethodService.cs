using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext qua DI
        public PaymentMethodService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation cho phương thức tạo mới
        public static string ValidateCreate(PaymentMethodCreateVModel model)
        {
            // Kiểm tra NamePaymentMethod không được để trống
            if (string.IsNullOrWhiteSpace(model.NamePaymentMethod))
            {
                return "Payment method name is required.";
            }

            // Kiểm tra NamePaymentMethod không quá 50 ký tự
            if (model.NamePaymentMethod.Length > 50)
            {
                return "Payment method name must be less than 50 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức cập nhật
        public static string ValidateUpdate(PaymentMethodUpdateVModel model)
        {
            // Kiểm tra Id hợp lệ
            if (model.Id <= 0)
            {
                return "Invalid payment method ID.";
            }

            // Áp dụng các validation của Create
            return ValidateCreate(model);
        }

        // [1.] Tạo mới phương thức thanh toán
        public async Task<ResponseResult> CreateAsync(PaymentMethodCreateVModel model)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra NamePaymentMethod đã tồn tại chưa
                var existingMethod = await _context.PaymentMethods
                    .AnyAsync(pm => pm.NamePaymentMethod != null &&
                                  pm.NamePaymentMethod.ToLower() == model.NamePaymentMethod.ToLower());
                if (existingMethod)
                {
                    return new ErrorResponseResult("Payment method name already exists.");
                }

                // B3: Chuyển ViewModel sang Entity
                var paymentMethod = model.ToEntity();

                // B4: Thêm phương thức thanh toán vào DbContext
                _context.PaymentMethods.Add(paymentMethod);
                await _context.SaveChangesAsync();

                // B5: Trả về kết quả thành công
                var paymentMethodVM = paymentMethod.ToGetVModel();
                return new SuccessResponseResult(paymentMethodVM, "Payment method created successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while creating the payment method: {ex.Message}");
            }
        }

        // [2.] Xóa phương thức thanh toán
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm phương thức thanh toán theo ID
                var paymentMethod = await _context.PaymentMethods
                    .FirstOrDefaultAsync(pm => pm.Id == id);
                if (paymentMethod == null)
                {
                    return new ErrorResponseResult($"Payment method not found with ID: {id}.");
                }

                // B2: Kiểm tra xem phương thức thanh toán có đang được sử dụng trong Orders không
                var isUsedInOrders = await _context.Orders
                    .AnyAsync(o => o.PaymentMethodId == id);
                if (isUsedInOrders)
                {
                    return new ErrorResponseResult("Payment method cannot be deleted because it is associated with one or more orders.");
                }

                // B3: Xóa phương thức thanh toán
                _context.PaymentMethods.Remove(paymentMethod);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Payment method deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while deleting the payment method: {ex.Message}");
            }
        }

        // [3.] Lấy tất cả phương thức thanh toán
        public async Task<List<PaymentMethodGetVModel>> GetAllAsync()
        {
            // Lấy tất cả phương thức thanh toán và sắp xếp theo NamePaymentMethod
            var paymentMethods = await _context.PaymentMethods
                .OrderBy(pm => pm.NamePaymentMethod)
                .ToListAsync();
            return paymentMethods.Select(pm => pm.ToGetVModel()).ToList();
        }

        // [4.] Lấy phương thức thanh toán theo ID
        public async Task<PaymentMethodGetVModel?> GetByIdAsync(int id)
        {
            // Tìm phương thức thanh toán theo ID
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == id);
            return paymentMethod?.ToGetVModel();
        }

        // [5.] Cập nhật phương thức thanh toán
        public async Task<ResponseResult> UpdateAsync(PaymentMethodUpdateVModel model)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm phương thức thanh toán theo ID
                var paymentMethod = await _context.PaymentMethods
                    .FirstOrDefaultAsync(pm => pm.Id == model.Id);
                if (paymentMethod == null)
                {
                    return new ErrorResponseResult($"Payment method not found with ID: {model.Id}.");
                }

                // B3: Kiểm tra NamePaymentMethod đã tồn tại chưa (ngoại trừ phương thức hiện tại)
                var existingMethod = await _context.PaymentMethods
                    .AnyAsync(pm => pm.NamePaymentMethod != null &&
                                  pm.NamePaymentMethod.ToLower() == model.NamePaymentMethod.ToLower() &&
                                  pm.Id != model.Id);
                if (existingMethod)
                {
                    return new ErrorResponseResult("Payment method name already exists.");
                }

                // B4: Cập nhật thông tin phương thức thanh toán
                paymentMethod.UpdateEntity(model);

                // B5: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B6: Trả về ViewModel
                var paymentMethodVM = paymentMethod.ToGetVModel();
                return new SuccessResponseResult(paymentMethodVM, "Payment method updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the payment method: {ex.Message}");
            }
        }
    }
}