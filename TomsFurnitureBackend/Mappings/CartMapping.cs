using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.CartVModel;

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

        // Chuyển từ Cart entity sang CartGetVModel, lấy ProductName từ ProductVariant
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
                ProductName = entity.ProVar?.Product?.ProductName
            };
        }
    }
}