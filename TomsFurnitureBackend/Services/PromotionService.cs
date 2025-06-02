using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly TomfurnitureContext _context;
        private readonly ILogger<PromotionService> _logger;

        public PromotionService(TomfurnitureContext context, ILogger<PromotionService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Validation cho tạo mới khuyến mãi
        private static string ValidateCreate(PromotionCreateVModel model)
        {
            // Kiểm tra mã khuyến mãi
            if (string.IsNullOrWhiteSpace(model.PromotionCode))
                return "Mã khuyến mãi là bắt buộc.";
            if (model.PromotionCode.Length > 50)
                return "Mã khuyến mãi không được dài quá 50 ký tự.";

            // Kiểm tra giá trị giảm giá
            if (model.DiscountValue <= 0)
                return "Giá trị giảm giá phải lớn hơn 0.";

            // Kiểm tra đơn hàng tối thiểu
            if (model.OrderMinimum < 0)
                return "Giá trị đơn hàng tối thiểu không được âm.";

            // Kiểm tra số tiền giảm tối đa
            if (model.MaximumDiscountAmount < 0)
                return "Số tiền giảm tối đa không được âm.";

            // Kiểm tra ngày
            if (model.EndDate < model.StartDate)
                return "Ngày kết thúc không được sớm hơn ngày bắt đầu.";

            // Kiểm tra số lần sử dụng
            if (model.CouponUsage < 0)
                return "Số lần sử dụng không được âm.";

            // Kiểm tra PromotionTypeId
            if (model.PromotionTypeId.HasValue && model.PromotionTypeId <= 0)
                return "ID loại khuyến mãi không hợp lệ.";

            return string.Empty;
        }

        // Validation cho cập nhật khuyến mãi
        private static string ValidateUpdate(PromotionUpdateVModel model)
        {
            if (model.Id <= 0)
                return "ID khuyến mãi không hợp lệ.";

            return ValidateCreate(model); // Tái sử dụng validation của Create
        }

        public async Task<List<PromotionGetVModel>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách tất cả khuyến mãi.");
                var promotions = await _context.Promotions
                    .Include(p => p.PromotionType) // Bao gồm thông tin PromotionType
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();
                var result = promotions.Select(p => p.ToGetVModel()).ToList();
                _logger.LogInformation("Lấy danh sách {Count} khuyến mãi thành công.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy danh sách khuyến mãi: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                throw new Exception($"Lỗi khi lấy danh sách khuyến mãi: {ex.Message}");
            }
        }

        public async Task<PromotionGetVModel?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy khuyến mãi với ID: {PromotionId}", id);
                var promotion = await _context.Promotions
                    .Include(p => p.PromotionType) // Bao gồm thông tin PromotionType
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (promotion == null)
                {
                    _logger.LogWarning("Không tìm thấy khuyến mãi với ID: {PromotionId}", id);
                    return null;
                }
                _logger.LogInformation("Lấy khuyến mãi {PromotionId} thành công.", id);
                return promotion.ToGetVModel();
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy khuyến mãi {PromotionId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                throw new Exception($"Lỗi khi lấy khuyến mãi: {ex.Message}");
            }
        }

        public async Task<ResponseResult> CreateAsync(PromotionCreateVModel model, string createdBy)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo khuyến mãi mới với mã: {PromotionCode}", model.PromotionCode);
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validate tạo khuyến mãi thất bại: {Error}", validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra mã khuyến mãi đã tồn tại
                var existingPromotion = await _context.Promotions
                    .AnyAsync(p => p.PromotionCode.ToLower() == model.PromotionCode.ToLower());
                if (existingPromotion)
                {
                    _logger.LogWarning("Mã khuyến mãi đã tồn tại: {PromotionCode}", model.PromotionCode);
                    return new ErrorResponseResult("Mã khuyến mãi đã tồn tại.");
                }

                // B3: Kiểm tra PromotionTypeId (nếu có)
                if (model.PromotionTypeId.HasValue)
                {
                    var promotionTypeExists = await _context.PromotionTypes
                        .AnyAsync(pt => pt.Id == model.PromotionTypeId);
                    if (!promotionTypeExists)
                    {
                        _logger.LogWarning("Không tìm thấy loại khuyến mãi với ID: {PromotionTypeId}", model.PromotionTypeId);
                        return new ErrorResponseResult("Loại khuyến mãi không tồn tại.");
                    }
                }

                // B4: Chuyển ViewModel sang Entity
                var promotion = model.ToEntity(createdBy);

                // B5: Thêm vào DbContext
                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();

                // B6: Trả về kết quả
                var promotionVM = promotion.ToGetVModel();
                _logger.LogInformation("Tạo khuyến mãi {PromotionId} thành công.", promotion.Id);
                return new SuccessResponseResult(promotionVM, "Tạo khuyến mãi thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo khuyến mãi: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi tạo khuyến mãi: {ex.Message}");
            }
        }

        public async Task<ResponseResult> UpdateAsync(PromotionUpdateVModel model, string updatedBy)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật khuyến mãi với ID: {PromotionId}", model.Id);
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validate cập nhật khuyến mãi thất bại: {Error}", validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm khuyến mãi theo ID
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Id == model.Id);
                if (promotion == null)
                {
                    _logger.LogWarning("Không tìm thấy khuyến mãi với ID: {PromotionId}", model.Id);
                    return new ErrorResponseResult("Không tìm thấy khuyến mãi.");
                }

                // B3: Kiểm tra mã khuyến mãi đã tồn tại (ngoại trừ khuyến mãi hiện tại)
                var existingPromotion = await _context.Promotions
                    .AnyAsync(p => p.PromotionCode.ToLower() == model.PromotionCode.ToLower() && p.Id != model.Id);
                if (existingPromotion)
                {
                    _logger.LogWarning("Mã khuyến mãi đã tồn tại: {PromotionCode}", model.PromotionCode);
                    return new ErrorResponseResult("Mã khuyến mãi đã tồn tại.");
                }

                // B4: Kiểm tra PromotionTypeId (nếu có)
                if (model.PromotionTypeId.HasValue)
                {
                    var promotionTypeExists = await _context.PromotionTypes
                        .AnyAsync(pt => pt.Id == model.PromotionTypeId);
                    if (!promotionTypeExists)
                    {
                        _logger.LogWarning("Không tìm thấy loại khuyến mãi với ID: {PromotionTypeId}", model.PromotionTypeId);
                        return new ErrorResponseResult("Loại khuyến mãi không tồn tại.");
                    }
                }

                // B5: Cập nhật entity
                promotion.UpdateEntity(model, updatedBy);

                // B6: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B7: Trả về kết quả
                var promotionVM = promotion.ToGetVModel();
                _logger.LogInformation("Cập nhật khuyến mãi {PromotionId} thành công.", model.Id);
                return new SuccessResponseResult(promotionVM, "Cập nhật khuyến mãi thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật khuyến mãi {PromotionId}: {Error}, InnerException: {InnerError}",
                    model.Id, ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi cập nhật khuyến mãi: {ex.Message}");
            }
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa khuyến mãi với ID: {PromotionId}", id);
                // B1: Tìm khuyến mãi theo ID
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (promotion == null)
                {
                    _logger.LogWarning("Không tìm thấy khuyến mãi với ID: {PromotionId}", id);
                    return new ErrorResponseResult("Không tìm thấy khuyến mãi.");
                }

                // B2: Kiểm tra khuyến mãi có đang được sử dụng trong đơn hàng
                var isUsedInOrders = await _context.Orders
                    .AnyAsync(o => o.PromotionId == id);
                if (isUsedInOrders)
                {
                    _logger.LogWarning("Khuyến mãi {PromotionId} đang được sử dụng trong đơn hàng.", id);
                    return new ErrorResponseResult("Không thể xóa khuyến mãi vì nó đang được sử dụng trong đơn hàng.");
                }

                // B3: Xóa khuyến mãi
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả
                _logger.LogInformation("Xóa khuyến mãi {PromotionId} thành công.", id);
                return new SuccessResponseResult(null, "Xóa khuyến mãi thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa khuyến mãi {PromotionId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi xóa khuyến mãi: {ex.Message}");
            }
        }
    }
}