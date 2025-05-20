using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class UnitMapping
    {
        public static UnitGetVModel ModelToVModel (Unit model)
        {
            return new UnitGetVModel
            {
                Id = model.Id,
                UnitName = model.UnitName,
                Description = model.Description,
                IsActive = model.IsActive,
                CreatedDate = model.CreatedDate,
                UpdatedDate = model.UpdatedDate,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy,
            };
        }
        public static Unit VModelToModel (UnitCreateVModel vModel)
        {
            return new Unit
            {
                UnitName = vModel.UnitName,
                Description = vModel.Description,
                IsActive = vModel.IsActive,
                CreatedDate = DateTime.Now,
            };
        }
    }
}
