using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class SizeExtensions
    {
        // Chuyển từ SizeCreateVModel sang Entity Size
        public static Size ToEntity(this SizeCreateVModel model)
        {
            // Tạo mới Size entity với các giá trị từ ViewModel
            return new Size
            {
                SizeName = model.SizeName,
                IsActive = true, // Mặc định là true khi tạo mới
                CreatedDate = DateTime.Now // Sử dụng UTC để nhất quán
            };
        }

        // Cập nhật thông tin Entity Size từ SizeUpdateVModel
        public static void UpdateEntity(this Size entity, SizeUpdateVModel model)
        {
            // Cập nhật các thuộc tính của entity
            entity.SizeName = model.SizeName;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.Now; // Cập nhật thời gian sửa đổi
        }

        // Chuyển từ Entity Size sang SizeGetVModel
        public static SizeGetVModel ToGetVModel(this Size entity)
        {
            return new SizeGetVModel
            {
                Id = entity.Id,
                SizeName = entity.SizeName,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}