using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class NewsExtensions
    {
        // Chuyển từ NewsCreateVModel sang entity News
        public static News ToEntity(this NewsCreateVModel model, string? newsAvatar, string createdBy, int? userId)
        {
            return new News
            {
                Title = model.Title,
                Content = model.Content,
                NewsAvatar = newsAvatar,
                UserId = userId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };
        }

        // Cập nhật entity News từ NewsUpdateVModel
        public static void UpdateEntity(this News entity, NewsUpdateVModel model, string? newsAvatar, string updatedBy, int? userId)
        {
            entity.Title = model.Title;
            entity.Content = model.Content;
            entity.UserId = userId;
            entity.NewsAvatar = newsAvatar ?? entity.NewsAvatar;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = updatedBy;
        }

        // Chuyển từ entity News sang NewsGetVModel
        public static NewsGetVModel ToGetVModel(this News entity)
        {
            return new NewsGetVModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                NewsAvatar = entity.NewsAvatar,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                UserName = entity.User?.UserName
            };
        }
    }
}