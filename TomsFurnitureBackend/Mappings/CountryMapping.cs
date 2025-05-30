using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class CountryExtensions
    {
        // Chuyển từ CountryCreateVModel sang Entity Country
        public static Country ToEntity(this CountryCreateVModel model, string? imageUrl)
        {
            // Tạo mới Country entity với các giá trị từ ViewModel
            return new Country
            {
                CountryName = model.CountryName,
                ImageUrl = imageUrl,
                IsActive = true, // Mặc định là true khi tạo mới
                CreatedDate = DateTime.UtcNow // Sử dụng UTC để nhất quán
            };
        }

        // Cập nhật thông tin Entity Country từ CountryUpdateVModel
        public static void UpdateEntity(this Country entity, CountryUpdateVModel model, string? imageUrl)
        {
            // Cập nhật các thuộc tính của entity
            entity.CountryName = model.CountryName;
            entity.ImageUrl = imageUrl ?? entity.ImageUrl; // Giữ nguyên ImageUrl nếu không có giá trị mới
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi
        }

        // Chuyển từ Entity Country sang CountryGetVModel
        public static CountryGetVModel ToGetVModel(this Country entity)
        {
            return new CountryGetVModel
            {
                Id = entity.Id,
                CountryName = entity.CountryName,
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
