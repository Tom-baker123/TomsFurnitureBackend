using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class PaymentMethodExtensions
    {
        // Chuyển từ PaymentMethodCreateVModel sang Entity PaymentMethod
        public static PaymentMethod ToEntity(this PaymentMethodCreateVModel model)
        {
            // Tạo mới PaymentMethod entity với các giá trị từ ViewModel
            return new PaymentMethod
            {
                NamePaymentMethod = model.NamePaymentMethod,
                IsActive = true, // Mặc định là true khi tạo mới
                CreatedDate = DateTime.UtcNow // Sử dụng UTC để nhất quán
            };
        }

        // Cập nhật thông tin Entity PaymentMethod từ PaymentMethodUpdateVModel
        public static void UpdateEntity(this PaymentMethod entity, PaymentMethodUpdateVModel model)
        {
            // Cập nhật các thuộc tính của entity
            entity.NamePaymentMethod = model.NamePaymentMethod;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi
        }

        // Chuyển từ Entity PaymentMethod sang PaymentMethodGetVModel
        public static PaymentMethodGetVModel ToGetVModel(this PaymentMethod entity)
        {
            return new PaymentMethodGetVModel
            {
                Id = entity.Id,
                NamePaymentMethod = entity.NamePaymentMethod ?? string.Empty,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}
