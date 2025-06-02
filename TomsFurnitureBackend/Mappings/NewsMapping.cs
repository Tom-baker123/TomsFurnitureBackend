using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class NewsExtensions
    {
        // Chuyển từ NewsCreateVModel sang entity News
        public static News ToEntity(this NewsCreateVModel model, string? newsAvatar, string createdBy)
        {
            // Tạo mới News entity từ ViewModel
            return new News
            {
                Title = model.Title,
                Content = model.Content,
                NewsAvatar = newsAvatar,
                UserId = model.UserId,
                IsActive = true, // Mặc định là true
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };
        }

        // Cập nhật entity News từ NewsUpdateVModel
        public static void UpdateEntity(this News entity, NewsUpdateVModel model, string? newsAvatar, string updatedBy)
        {
            // Cập nhật các trường của entity
            entity.Title = model.Title;
            entity.Content = model.Content;
            entity.UserId = model.UserId;
            entity.NewsAvatar = newsAvatar ?? entity.NewsAvatar; // Giữ nguyên nếu không có ảnh mới
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = updatedBy;
        }

        // Chuyển từ entity News sang NewsGetVModel
        public static NewsGetVModel ToGetVModel(this News entity)
        {
            // Ánh xạ entity sang ViewModel
            return new NewsGetVModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                NewsAvatar = entity.NewsAvatar,
                UserId = entity.UserId,
                UserName = entity.User?.UserName, // Lấy tên người dùng nếu có
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}