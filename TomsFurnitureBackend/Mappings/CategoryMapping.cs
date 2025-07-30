using MimeKit.Encodings;
using System.Runtime.CompilerServices;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class CategoryExtensions 
    {
        // Chuyển từ CategoryCreateVModel sang Entity Category
        public static Category ToEntity(this CategoryCreateVModel model)
        {
            return new Category { 
                CategoryName = model.CategoryName,
                Descriptions = model.Descriptions,
                ParentId = model.ParentId,
                RoomTypeId = model.RoomTypeId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
            };
        }

        // Cập nhật thông tin Entity Category từ CategoryUpdateVModel
        public static void UpdateEntity(this Category entity, CategoryUpdateVModel model)
        {
            entity.CategoryName = model.CategoryName;
            entity.Descriptions = model.Descriptions;
            entity.ParentId = model.ParentId;
            entity.RoomTypeId = model.RoomTypeId;
            entity.IsActive = model.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        // Chuyển từ Entity Category sang CategoryGetVModel
        public static CategoryGetVModel ToGetVModel(this Category entity)
        {
            return new CategoryGetVModel
            {
                Id = entity.Id,
                CategoryName = entity.CategoryName,
                Descriptions = entity.Descriptions,
                ParentId = entity.ParentId,
                RoomTypeId = entity.RoomTypeId,
                IsActive = entity.IsActive,
                ImageUrl = entity.ImageUrl,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                RoomTypeName = entity.RoomType != null ? entity.RoomType.RoomTypeName : null
            };
        }
    }
}
