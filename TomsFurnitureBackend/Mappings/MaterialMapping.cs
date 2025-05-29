using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class MaterialExtensions
    {
        // Chuyển từ MaterialCreateVModel sang Entity Material
        public static Material ToEntity(this MaterialCreateVModel model)
        {
            // Tạo mới Material entity với các giá trị từ ViewModel
            {
                return new Material
                {
                    MaterialName = model.MaterialName,
                    IsActive = true, // Mặc định là true khi tạo mới
                    CreatedDate = DateTime.Now, // Sử dụng UTC để nhất quán
                };
            }
        }

        // Cập nhật thông tin Entity Material từ MaterialUpdateVModel
        public static void UpdateEnttity(this Material entity, MaterialUpdateVModel model)
        {
            // Cập nhật các thuộc tính của entity
            entity.MaterialName = model.MaterialName;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu IsActive không được cung cấp
            entity.UpdatedDate = DateTime.Now; // Cập nhật ngày giờ hiện tại
        }

        // Chuyển từ Entity Material sang MaterialGetVModel
        public static MaterialGetVModel ToGetVModel(this Material entity) {
            // Tạo ViewModel từ entity
            
            return new MaterialGetVModel
            {
                Id = entity.Id,
                MaterialName = entity.MaterialName,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
            };
        }
    }
}
