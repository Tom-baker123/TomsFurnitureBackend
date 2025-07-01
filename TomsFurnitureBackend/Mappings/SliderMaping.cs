using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class SliderExtensions
    {
        // Ánh xạ để tạo entity từ model
        public static Slider ToEntity(this SliderCreateVModel model)
        {
            return new Slider
            {
                Title = model.Title,
                Description = model.Description,
                LinkUrl = string.IsNullOrWhiteSpace(model.LinkUrl) ? "/" : model.LinkUrl.Trim(), // Sử dụng IsNullOrWhiteSpace và Trim
                IsPoster = model.IsPoster,
                Position = string.IsNullOrWhiteSpace(model.Position) ? "Home Page" : model.Position.Trim(), // Sử dụng IsNullOrWhiteSpace và Trim
                DisplayOrder = model.DisplayOrder,
                IsActive = true,
                ProductId = model.ProductId,
                CreatedDate = DateTime.UtcNow,
            };
        }

        // Ánh xạ cập nhật thông tin slider
        public static void UpdateEntity(this Slider entity, SliderUpdateVModel model)
        {
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.LinkUrl = string.IsNullOrWhiteSpace(model.LinkUrl) ? "/" : model.LinkUrl.Trim(); // Sử dụng IsNullOrWhiteSpace và Trim
            entity.IsPoster = model.IsPoster;
            entity.Position = string.IsNullOrWhiteSpace(model.Position) ? "Home Page" : model.Position.Trim(); // Sử dụng IsNullOrWhiteSpace và Trim
            entity.DisplayOrder = model.DisplayOrder;
            entity.IsActive = model.IsActive;
            entity.ProductId = model.ProductId;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        // Ánh xạ lấy thông tin slider
        public static SliderGetVModel ToGetVModel(this Slider entity)
        {
            var model = new SliderGetVModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                LinkUrl = entity.LinkUrl,
                IsPoster = entity.IsPoster,
                Position = entity.Position,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                ProductId = entity.ProductId,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                ImageUrl = entity.ImageUrl
            };
            return model;
        }
    }
}