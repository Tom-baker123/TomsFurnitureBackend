using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.CartVModel;

namespace TomsFurnitureBackend.Extensions
{
    public static class CartMapping
    {
        public static Cart ToEntity(this CartCreateVModel model, int? userId = null)
        {
            return new Cart
            {
                Quantity = model.Quantity,
                ProId = model.ProId,
                UserId = userId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userId?.ToString() ?? "Guest"
            };
        }

        public static void UpdateEntity(this Cart entity, CartUpdateVModel model)
        {
            entity.Quantity = model.Quantity;
            entity.ProId = model.ProId;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = entity.UserId?.ToString() ?? "Guest";
        }

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
                ProId = entity.ProId,
                ProductName = entity.Pro?.ProductName
            };
        }
    }
}