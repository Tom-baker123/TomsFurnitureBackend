using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class SupplierExtensions
    {
        // Chuyển từ SupplierCreateVModel sang Entity Supplier
        public static Supplier ToEntity(this SupplierCreateVModel model, string? imageUrl)
        {
            // Tạo mới Supplier entity với các giá trị từ ViewModel
            return new Supplier
            {
                SupplierName = model.SupplierName,
                ContactName = model.ContactName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ImageUrl = imageUrl,
                Notes = model.Notes,
                TaxId = model.TaxId,
                IsActive = true, // Mặc định là true khi tạo mới
                CreatedDate = DateTime.UtcNow // Sử dụng UTC để nhất quán
            };
        }

        // Cập nhật thông tin Entity Supplier từ SupplierUpdateVModel
        public static void UpdateEntity(this Supplier entity, SupplierUpdateVModel model, string? imageUrl)
        {
            // Cập nhật các thuộc tính của entity
            entity.SupplierName = model.SupplierName;
            entity.ContactName = model.ContactName;
            entity.Email = model.Email;
            entity.PhoneNumber = model.PhoneNumber;
            entity.ImageUrl = imageUrl ?? entity.ImageUrl; // Giữ nguyên ImageUrl nếu không có giá trị mới
            entity.Notes = model.Notes;
            entity.TaxId = model.TaxId;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi
        }

        // Chuyển từ Entity Supplier sang SupplierGetVModel
        public static SupplierGetVModel ToGetVModel(this Supplier entity)
        {
            return new SupplierGetVModel
            {
                Id = entity.Id,
                SupplierName = entity.SupplierName,
                ContactName = entity.ContactName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                ImageUrl = entity.ImageUrl,
                Notes = entity.Notes,
                TaxId = entity.TaxId,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}