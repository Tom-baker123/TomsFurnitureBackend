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
    public class PromotionTypeService : IPromotionTypeService
    {
        private readonly TomfurnitureContext _context;
        private readonly ILogger<PromotionTypeService> _logger;

        public PromotionTypeService(TomfurnitureContext context, ILogger<PromotionTypeService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Validation cho tạo mới loại khuyến mãi
        private static string ValidateCreate(PromotionTypeCreateVModel model)
        {
            // Kiểm tra tên loại khuyến mãi
            if (string.IsNullOrWhiteSpace(model.PromotionTypeName))
                return "Tên loại khuyến mãi là bắt buộc.";
            if (model.PromotionTypeName.Length > 100)
                return "Tên loại khuyến mãi không được dài quá 100 ký tự.";

            // Kiểm tra đơn vị khuyến mãi
            if (model.PromotionUnit < 0)
                return "Đơn vị khuyến mãi không được âm.";

            return string.Empty;
        }

        // Validation cho cập nhật loại khuyến mãi
        private static string ValidateUpdate(PromotionTypeUpdateVModel model)
        {
            if (model.Id <= 0)
                return "ID loại khuyến mãi không hợp lệ.";

            return ValidateCreate(model); // Tái sử dụng validation của Create
        }

        public async Task<List<PromotionTypeGetVModel>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách tất cả loại khuyến mãi.");
                var promotionTypes = await _context.PromotionTypes
                    .OrderByDescending(pt => pt.CreatedDate)
                    .ToListAsync();
                var result = promotionTypes.Select(pt => pt.ToGetVModel()).ToList();
                _logger.LogInformation("Lấy danh sách {Count} loại khuyến mãi thành công.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy danh sách loại khuyến mãi: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                throw new Exception($"Lỗi khi lấy danh sách loại khuyến mãi: {ex.Message}");
            }
        }

        public async Task<PromotionTypeGetVModel?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy loại khuyến mãi với ID: {PromotionTypeId}", id);
                var promotionType = await _context.PromotionTypes
                    .FirstOrDefaultAsync(pt => pt.Id == id);
                if (promotionType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại khuyến mãi với ID: {PromotionTypeId}", id);
                    return null;
                }
                _logger.LogInformation("Lấy loại khuyến mãi {PromotionTypeId} thành công.", id);
                return promotionType.ToGetVModel();
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy loại khuyến mãi {PromotionTypeId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                throw new Exception($"Lỗi khi lấy loại khuyến mãi: {ex.Message}");
            }
        }

        public async Task<ResponseResult> CreateAsync(PromotionTypeCreateVModel model, string createdBy)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo loại khuyến mãi mới với tên: {PromotionTypeName}", model.PromotionTypeName);
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validate tạo loại khuyến mãi thất bại: {Error}", validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra tên loại khuyến mãi đã tồn tại
                var existingPromotionType = await _context.PromotionTypes
                    .AnyAsync(pt => pt.PromotionTypeName.ToLower() == model.PromotionTypeName.ToLower());
                if (existingPromotionType)
                {
                    _logger.LogWarning("Tên loại khuyến mãi đã tồn tại: {PromotionTypeName}", model.PromotionTypeName);
                    return new ErrorResponseResult("Tên loại khuyến mãi đã tồn tại.");
                }

                // B3: Chuyển ViewModel sang Entity
                var promotionType = model.ToEntity(createdBy);

                // B4: Thêm vào DbContext
                _context.PromotionTypes.Add(promotionType);
                await _context.SaveChangesAsync();

                // B5: Trả về kết quả
                var promotionTypeVM = promotionType.ToGetVModel();
                _logger.LogInformation("Tạo loại khuyến mãi {PromotionTypeId} thành công.", promotionType.Id);
                return new SuccessResponseResult(promotionTypeVM, "Tạo loại khuyến mãi thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo loại khuyến mãi: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi tạo loại khuyến mãi: {ex.Message}");
            }
        }

        public async Task<ResponseResult> UpdateAsync(PromotionTypeUpdateVModel model, string updatedBy)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật loại khuyến mãi với ID: {PromotionTypeId}", model.Id);
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validate cập nhật loại khuyến mãi thất bại: {Error}", validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm loại khuyến mãi theo ID
                var promotionType = await _context.PromotionTypes
                    .FirstOrDefaultAsync(pt => pt.Id == model.Id);
                if (promotionType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại khuyến mãi với ID: {PromotionTypeId}", model.Id);
                    return new ErrorResponseResult("Không tìm thấy loại khuyến mãi.");
                }

                // B3: Kiểm tra tên loại khuyến mãi đã tồn tại (ngoại trừ loại khuyến mãi hiện tại)
                var existingPromotionType = await _context.PromotionTypes
                    .AnyAsync(pt => pt.PromotionTypeName.ToLower() == model.PromotionTypeName.ToLower() && pt.Id != model.Id);
                if (existingPromotionType)
                {
                    _logger.LogWarning("Tên loại khuyến mãi đã tồn tại: {PromotionTypeName}", model.PromotionTypeName);
                    return new ErrorResponseResult("Tên loại khuyến mãi đã tồn tại.");
                }

                // B4: Cập nhật entity
                promotionType.UpdateEntity(model, updatedBy);

                // B5: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B6: Trả về kết quả
                var promotionTypeVM = promotionType.ToGetVModel();
                _logger.LogInformation("Cập nhật loại khuyến mãi {PromotionTypeId} thành công.", model.Id);
                return new SuccessResponseResult(promotionTypeVM, "Cập nhật loại khuyến mãi thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật loại khuyến mãi {PromotionTypeId}: {Error}, InnerException: {InnerError}",
                    model.Id, ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi cập nhật loại khuyến mãi: {ex.Message}");
            }
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa loại khuyến mãi với ID: {PromotionTypeId}", id);
                // B1: Tìm loại khuyến mãi theo ID
                var promotionType = await _context.PromotionTypes
                    .FirstOrDefaultAsync(pt => pt.Id == id);
                if (promotionType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại khuyến mãi với ID: {PromotionTypeId}", id);
                    return new ErrorResponseResult("Không tìm thấy loại khuyến mãi.");
                }

                // B2: Kiểm tra loại khuyến mãi có đang được sử dụng trong khuyến mãi
                var isUsedInPromotions = await _context.Promotions
                    .AnyAsync(p => p.PromotionTypeId == id);
                if (isUsedInPromotions)
                {
                    _logger.LogWarning("Loại khuyến mãi {PromotionTypeId} đang được sử dụng trong khuyến mãi.", id);
                    return new ErrorResponseResult("Không thể xóa loại khuyến mãi vì nó đang được sử dụng.");
                }

                // B3: Xóa loại khuyến mãi
                _context.PromotionTypes.Remove(promotionType);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả
                _logger.LogInformation("Xóa loại khuyến mãi {PromotionTypeId} thành công.", id);
                return new SuccessResponseResult(null, "Xóa loại khuyến mãi thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa loại khuyến mãi {PromotionTypeId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi xóa loại khuyến mãi: {ex.Message}");
            }
        }
    }
}