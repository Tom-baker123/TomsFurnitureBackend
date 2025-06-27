using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class BannerExtensions
    {
        // Chuyển từ BannerCreateVModel sang Entity Banner
        public static Banner ToEntity(this BannerCreateVModel model, string imageUrl, string imageUrlMobile)
        {
            return new Banner
            {
                Title = model.Title,
                Description = model.Description,
                ImageUrl = imageUrl,
                ImageUrlmobile = imageUrlMobile,
                LinkUrl = model.LinkUrl,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DisplayOrder = model.DisplayOrder,
                Position = model.Position,
                IsActive = model.IsActive ?? true, // Mặc định là true nếu không có giá trị
                UserId = model.UserId,
                CreatedDate = DateTime.Now
            };
        }

        // Cập nhật thông tin Entity Banner từ BannerUpdateVModel
        public static void UpdateEntity(this Banner entity, BannerUpdateVModel model, string? imageUrl, string? imageUrlMobile)
        {
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.ImageUrl = imageUrl ?? entity.ImageUrl; // Giữ nguyên nếu không có giá trị mới
            entity.ImageUrlmobile = imageUrlMobile ?? entity.ImageUrlmobile; // Giữ nguyên nếu không có giá trị mới
            entity.LinkUrl = model.LinkUrl;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.DisplayOrder = model.DisplayOrder;
            entity.Position = model.Position;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UserId = model.UserId;
            entity.UpdatedDate = DateTime.Now;
        }

        // Chuyển từ Entity Banner sang BannerGetVModel
        public static BannerGetVModel ToGetVModel(this Banner entity)
        {
            return new BannerGetVModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                ImageUrlmobile = entity.ImageUrlmobile,
                LinkUrl = entity.LinkUrl,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                DisplayOrder = entity.DisplayOrder,
                Position = entity.Position,
                IsActive = entity.IsActive,
                UserId = entity.UserId,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}