using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class OrderStatusMapping
    {
        public static OrderStatus ToEntity(this OrderStatusCreateVModel model)
        {
            return new OrderStatus
            {
                OrderStatusName = model.OrderStatusName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this OrderStatus entity, OrderStatusUpdateVModel model)
        {
            entity.OrderStatusName = model.OrderStatusName;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        public static OrderStatusGetVModel ToGetVModel(this OrderStatus entity)
        {
            return new OrderStatusGetVModel
            {
                Id = entity.Id,
                OrderStatusName = entity.OrderStatusName,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}
