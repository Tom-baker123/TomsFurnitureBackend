using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class UnitExtensions
    {
        // Ánh xạ từ UnitCreateVModel sang Entity Unit (cho hàm thêm)
        public static Unit ToEntity(this UnitCreateVModel model)
        {
            return new Unit
            {
                UnitName = model.UnitName,
                Description = model.Description,
                IsActive = true,
                CreatedDate = DateTime.Now,
            };
        }

        // Cập nhật Entity Unit từ UnitUpdateVModel (cho hàm cập nhật)
        public static void UpdateEntity(this Unit entity, UnitUpdateVModel model)
        {

            entity.UnitName = model.UnitName;
            entity.Description = model.Description;
            entity.IsActive = model.IsActive ?? true;
            entity.UpdatedDate = DateTime.Now;
        }

        // Ánh xạ từ Entity Unit sang UnitGetVModel (cho hàm get)
        public static UnitGetVModel ToGetVModel(this Unit entity)
        {
            return new UnitGetVModel
            {
                Id = entity.Id,
                UnitName = entity.UnitName,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
            };
        }
    }
}
