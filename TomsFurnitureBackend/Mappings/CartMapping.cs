using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.CartVModel;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Extensions
{
    public static class CartMapping
    {
        // Chuyển từ CartCreateVModel sang Cart entity, sử dụng ProVarId
        public static Cart ToEntity(this CartCreateVModel model, int? userId = null)
        {
            return new Cart
            {
                Quantity = model.Quantity,
                ProVarId = model.ProVarId,
                UserId = userId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userId?.ToString() ?? "Guest"
            };
        }

        // Cập nhật Cart entity từ CartUpdateVModel, sử dụng ProVarId
        public static void UpdateEntity(this Cart entity, CartUpdateVModel model)
        {
            entity.Quantity = model.Quantity;
            entity.ProVarId = model.ProVarId;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = entity.UserId?.ToString() ?? "Guest";
        }

        // Chuyển từ Cart entity sang CartGetVModel, lấy ProductName và thông tin biến thể từ ProductVariant
        public static CartGetVModel ToGetVModel(this Cart entity)
        {
            return new CartGetVModel
            {
                Id = entity.Id,
                Quantity = entity.Quantity,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                UserId = entity.UserId,
                ProVarId = entity.ProVarId,
                ProductName = entity.ProVar?.Product?.ProductName,
                ProductVariant = entity.ProVar == null ? null : new ProductVariantGetVModel
                {
                    Id = entity.ProVar.Id,
                    OriginalPrice = entity.ProVar.OriginalPrice,
                    DiscountedPrice = entity.ProVar.DiscountedPrice,
                    StockQty = entity.ProVar.StockQty,
                    ColorId = entity.ProVar.ColorId ?? 0,
                    ColorName = entity.ProVar.Color?.ColorName,
                    ColorCode = entity.ProVar.Color?.ColorCode,
                    SizeId = entity.ProVar.SizeId ?? 0,
                    SizeName = entity.ProVar.Size?.SizeName,
                    MaterialId = entity.ProVar.MaterialId ?? 0,
                    MaterialName = entity.ProVar.Material?.MaterialName,
                    UnitId = entity.ProVar.UnitId ?? 0,
                    UnitName = entity.ProVar.Unit?.UnitName,
                    Images = entity.ProVar.ProductVariantImages?.Select(img => new ProductVariantImageGetVModel
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl,
                        Attribute = img.Attribute,
                        DisplayOrder = img.DisplayOrder,
                        IsActive = img.IsActive,
                        CreatedDate = img.CreatedDate,
                        UpdatedDate = img.UpdatedDate,
                        CreatedBy = img.CreatedBy,
                        UpdatedBy = img.UpdatedBy,
                        ProVarId = img.ProVarId
                    }).ToList() ?? new List<ProductVariantImageGetVModel>()
                }
            };
        }
    }
}