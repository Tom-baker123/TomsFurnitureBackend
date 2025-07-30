using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using System.Linq;

namespace TomsFurnitureBackend.Mappings
{
    public static class RoomTypeExtensions
    {
        // Chuyển từ RoomTypeCreateVModel sang Entity RoomType
        public static RoomType ToEntity(this RoomTypeCreateVModel model, string? imageUrl, string slug)
        {
            return new RoomType
            {
                RoomTypeName = model.RoomTypeName,
                Slug = slug,
                ImageUrl = imageUrl,
                IsActive = true, // Mặc định là true
                CreatedDate = DateTime.Now
            };
        }

        // Cập nhật thông tin Entity RoomType từ RoomTypeUpdateVModel
        public static void UpdateEntity(this RoomType entity, RoomTypeUpdateVModel model, string? imageUrl, string? slug)
        {
            entity.Id = model.Id;
            entity.RoomTypeName = model.RoomTypeName;
            entity.Slug = slug ?? entity.Slug;
            entity.ImageUrl = imageUrl ?? entity.ImageUrl; // Giữ nguyên ImageUrl nếu không có giá trị mới
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.Now;
        }

        // Chuyển từ Entity RoomType sang RoomTypeGetVModel
        public static RoomTypeGetVModel ToGetVModel(this RoomType entity)
        {
            return new RoomTypeGetVModel
            {
                Id = entity.Id,
                RoomTypeName = entity.RoomTypeName,
                Slug = entity.Slug,
                ImageUrl = entity.ImageUrl,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                Categories = entity.Categories?.Select(c => c.ToGetVModel()).ToList()
            };
        }
    }
}
