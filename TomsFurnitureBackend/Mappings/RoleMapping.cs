using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class RoleExtensions
    {
        // Chuyển từ RoleCreateVModel sang Entity Role
        public static Role ToEntity(this RoleCreateVModel model)
        {
            // Tạo mới Role entity với các giá trị từ ViewModel
            return new Role
            {
                RoleName = model.RoleName,
                IsActive = true, // Mặc định là true khi tạo mới
                CreatedDate = DateTime.UtcNow // Sử dụng UTC để nhất quán
            };
        }

        // Cập nhật thông tin Entity Role từ RoleUpdateVModel
        public static void UpdateEntity(this Role entity, RoleUpdateVModel model)
        {
            // Cập nhật các thuộc tính của entity
            entity.RoleName = model.RoleName;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi
        }

        // Chuyển từ Entity Role sang RoleGetVModel
        public static RoleGetVModel ToGetVModel(this Role entity)
        {
            return new RoleGetVModel
            {
                Id = entity.Id,
                RoleName = entity.RoleName,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}