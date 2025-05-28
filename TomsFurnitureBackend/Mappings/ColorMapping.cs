using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class ColorExtensions
    {
        // Chuyển từ ColorCreateVModel sang Entity Color
        public static Color ToEntity(this ColorCreateVModel model) {
            return new Color {
                ColorName = model.ColorName,
                ColorCode = model.ColorCode,
                IsActive = true, // Mặc định là true nếu không có giá trị
                CreatedDate = DateTime.Now,
            };
        }

        // Cnhật từ ColorUpdateVModel sang Entity Color
        public static void UpdateEntity(this Color entity, ColorUpdateVModel model)
        {
            entity.ColorName = model.ColorName;
            entity.ColorCode = model.ColorCode;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.Now;
        }

        // Chuyển từ Entity Color sang ColorGetVModel
        public static ColorGetVModel ToGetVModel(this Color entity) {
            return new ColorGetVModel
            {
                Id = entity.Id,
                ColorName = entity.ColorName,
                ColorCode = entity.ColorCode,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
            };
        }
    }
}
