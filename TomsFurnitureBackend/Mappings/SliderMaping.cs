using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class SliderExtensions
    {
        public static Slider ToEntity(this SliderCreateVModel model)
        {
            return new Slider
            {
                Title = model.Title,
                Description = model.Description,
                LinkUrl = model.LinkUrl,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsPoster = model.IsPoster,
                Position = model.Position,
                DisplayOrder = model.DisplayOrder,
                IsActive = true,
                ProductId = model.ProductId,
                CreatedDate = DateTime.UtcNow,
            };
        }

        public static void UpdateEntity(this Slider entity, SliderUpdateVModel model)
        {
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.LinkUrl = model.LinkUrl;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.IsPoster = model.IsPoster;
            entity.Position = model.Position;
            entity.DisplayOrder = model.DisplayOrder;
            entity.IsActive = model.IsActive;
            entity.ProductId = model.ProductId;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        public static SliderGetVModel ToGetVModel(this Slider entity)
        {
            var model = new SliderGetVModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                LinkUrl = entity.LinkUrl,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
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
