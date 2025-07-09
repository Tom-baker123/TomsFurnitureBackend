using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Helpers;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Services
{
    public class ProductVariantImageService : IProductVariantImageService
    {
        private readonly TomfurnitureContext _context;
        public ProductVariantImageService(TomfurnitureContext context)
        {
            _context = context;
        }

        // Validation chung cho thêm/s?a
        private async Task<string?> ValidateAsync(ProductVariantImageCreateVModel model, int? excludeId = null)
        {
            if (model.ProVarId == null || model.ProVarId <= 0)
                return "ProVarId is required and must be valid.";
            var variant = await _context.ProductVariants.FindAsync(model.ProVarId);
            if (variant == null)
                return $"ProductVariant with ID {model.ProVarId} does not exist.";
            if (model.DisplayOrder.HasValue && model.DisplayOrder < 0)
                return "DisplayOrder must be >= 0.";
            // B? ki?m tra DisplayOrder ?ã t?n t?i ?? cho phép swap v? trí
            return null;
        }

        public async Task<List<ProductVariantImageGetVModel>> GetAllAsync()
        {
            var images = await _context.ProductVariantImages.OrderBy(x => x.DisplayOrder).ToListAsync();
            return images.Select(x => x.ToGetVModel()).ToList();
        }

        public async Task<ProductVariantImageGetVModel?> GetByIdAsync(int id)
        {
            var entity = await _context.ProductVariantImages.FindAsync(id);
            return entity?.ToGetVModel();
        }

        public async Task<ResponseResult> CreateAsync(ProductVariantImageCreateVModel model, string imageUrl)
        {
            var validation = await ValidateAsync(model);
            if (validation != null)
                return new ErrorResponseResult(validation);
            var entity = model.ToEntity();
            entity.ImageUrl = imageUrl;
            // Tính toán DisplayOrder n?u ch?a có
            if (!entity.DisplayOrder.HasValue || entity.DisplayOrder <= 0)
            {
                var count = await _context.ProductVariantImages.CountAsync(x => x.ProVarId == entity.ProVarId);
                entity.DisplayOrder = count + 1;
            }
            _context.ProductVariantImages.Add(entity);
            await _context.SaveChangesAsync();
            // S?p x?p l?i DisplayOrder sau khi thêm m?i
            await ProductVariantImageHelper.AdjustDisplayOrder(_context, entity.ProVarId, entity.DisplayOrder ?? 1, entity.Id);
            return new SuccessResponseResult(entity.ToGetVModel(), "Product variant image created successfully.");
        }

        public async Task<ResponseResult> UpdateAsync(ProductVariantImageUpdateVModel model, string? imageUrl = null)
        {
            var entity = await _context.ProductVariantImages.FindAsync(model.Id);
            if (entity == null)
                return new ErrorResponseResult("Product variant image not found.");
            var validation = await ValidateAsync(model, model.Id);
            if (validation != null)
                return new ErrorResponseResult(validation);
            // Lưu lại DisplayOrder cũ trước khi cập nhật
            int? oldDisplayOrder = entity.DisplayOrder;
            entity.UpdateEntity(model);
            if (!string.IsNullOrEmpty(imageUrl))
                entity.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();
            // Sắp xếp lại DisplayOrder sau khi cập nhật, truyền oldDisplayOrder để swap chính xác
            await ProductVariantImageHelper.AdjustDisplayOrder(_context, entity.ProVarId, model.DisplayOrder ?? 1, entity.Id, oldDisplayOrder);
            return new SuccessResponseResult(entity.ToGetVModel(), "Product variant image updated successfully.");
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            var entity = await _context.ProductVariantImages.FindAsync(id);
            if (entity == null)
                return new ErrorResponseResult("Product variant image not found.");
            _context.ProductVariantImages.Remove(entity);
            await _context.SaveChangesAsync();
            return new SuccessResponseResult(null, "Product variant image deleted successfully.");
        }
    }
}
