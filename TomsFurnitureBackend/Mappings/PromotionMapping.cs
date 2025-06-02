using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class PromotionExtensions
    {
        // Chuyển từ PromotionCreateVModel sang Entity Promotion
        public static Promotion ToEntity(this PromotionCreateVModel model, string createdBy)
        {
            // Tạo mới Promotion entity với các giá trị từ ViewModel
            return new Promotion
            {
                PromotionCode = model.PromotionCode,
                DiscountValue = model.DiscountValue,
                OrderMinimum = model.OrderMinimum,
                MaximumDiscountAmount = model.MaximumDiscountAmount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CouponUsage = model.CouponUsage,
                PromotionTypeId = model.PromotionTypeId,
                IsActive = true, // Mặc định là true khi tạo mới
                CreatedDate = DateTime.Now,
                CreatedBy = createdBy
            };
        }

        // Cập nhật thông tin Entity Promotion từ PromotionUpdateVModel
        public static void UpdateEntity(this Promotion entity, PromotionUpdateVModel model, string updatedBy)
        {
            // Cập nhật các trường của entity
            entity.PromotionCode = model.PromotionCode ?? entity.PromotionCode;
            entity.DiscountValue = model.DiscountValue;
            entity.OrderMinimum = model.OrderMinimum;
            entity.MaximumDiscountAmount = model.MaximumDiscountAmount;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.CouponUsage = model.CouponUsage;
            entity.PromotionTypeId = model.PromotionTypeId ?? entity.PromotionTypeId;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedBy = updatedBy;
        }

        // Chuyển từ Entity Promotion sang PromotionGetVModel
        public static PromotionGetVModel ToGetVModel(this Promotion entity)
        {
            return new PromotionGetVModel
            {
                Id = entity.Id,
                PromotionCode = entity.PromotionCode,
                DiscountValue = entity.DiscountValue,
                OrderMinimum = entity.OrderMinimum,
                MaximumDiscountAmount = entity.MaximumDiscountAmount,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                CouponUsage = entity.CouponUsage,
                PromotionTypeId = entity.PromotionTypeId,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                PromotionType = entity.PromotionType?.ToGetVModel() // Bao gồm thông tin PromotionType
            };
        }
    }
}