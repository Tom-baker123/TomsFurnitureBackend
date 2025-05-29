using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class BrandExtensions
    {
        // Chuyển từ BrandCreateVModel sang Entity Brand
        public static Brand ToEntity(this BrandCreateVModel model, string? imageUrl)
        {
            // Tạo mới Brand entity với các giá trị từ ViewModel
            return new Brand {
                BrandName = model.BrandName,
                ImageUrl = imageUrl,
                IsActive = true, // Mặc định là true nếu không có giá trị
                CreatedDate = DateTime.Now,
            };
        }
        // Cập nhật thông tin Entity Brand từ BrandUpdateVModel
        public static void UpdateEntity(this Brand entity, BrandUpdateVModel model, string? imageUrl)
        {
            entity.Id = model.Id;
            entity.ImageUrl = imageUrl ?? entity.ImageUrl; // Giữ nguyên ImageUrl nếu không có giá trị mới
            entity.BrandName = model.BrandName;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.Now;
        }
        // Chuyển từ Entity Brand sang BrandGetVModel
        public static BrandGetVModel ToGetVModel(this Brand entity)
        {
            {
                return new BrandGetVModel
                {
                    Id = entity.Id,
                    BrandName = entity.BrandName,
                    ImageUrl = entity.ImageUrl,
                    IsActive = entity.IsActive,
                    CreatedDate = entity.CreatedDate,
                    UpdatedDate = entity.UpdatedDate,
                    CreatedBy = entity.CreatedBy,
                    UpdatedBy = entity.UpdatedBy
                };
            }
        }
    }
}
