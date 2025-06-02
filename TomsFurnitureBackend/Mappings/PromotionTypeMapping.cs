using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

public static class PromotionTypeExtensions
{
    // Chuyển từ PromotionTypeCreateVModel sang Entity PromotionType
    public static PromotionType ToEntity(this PromotionTypeCreateVModel model, string createdBy)
    {
        // Tạo mới PromotionType entity với các giá trị từ ViewModel
        return new PromotionType
        {
            PromotionTypeName = model.PromotionTypeName,
            Description = model.Description,
            PromotionUnit = model.PromotionUnit,
            IsActive = true, // Mặc định là true khi tạo mới
            CreatedDate = DateTime.Now,
            CreatedBy = createdBy
        };
    }

    // Cập nhật thông tin Entity PromotionType từ PromotionTypeUpdateVModel
    public static void UpdateEntity(this PromotionType entity, PromotionTypeUpdateVModel model, string updatedBy)
    {
        // Cập nhật các trường của entity
        entity.PromotionTypeName = model.PromotionTypeName;
        entity.Description = model.Description;
        entity.PromotionUnit = model.PromotionUnit;
        entity.IsActive = model.IsActive ?? entity.IsActive;
        entity.UpdatedDate = DateTime.Now;
        entity.UpdatedBy = updatedBy;
    }

    // Chuyển từ Entity PromotionType sang PromotionTypeGetVModel
    public static PromotionTypeGetVModel ToGetVModel(this PromotionType entity)
    {
        return new PromotionTypeGetVModel
        {
            Id = entity.Id,
            PromotionTypeName = entity.PromotionTypeName,
            Description = entity.Description,
            PromotionUnit = entity.PromotionUnit,
            IsActive = entity.IsActive,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate,
            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.UpdatedBy
        };
    }
}