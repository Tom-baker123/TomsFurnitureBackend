using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class MaterialService : IMaterialService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext? _context;

        // Constructor nhận DbContext qua DI
        public MaterialService(TomfurnitureContext context)
        {
            _context = context;
        }

        // Validation cho phương thức tạo mới vật liệu
        public static string ValidateCreate(MaterialCreateVModel model)
        {
            // Kiểm tra tên vật liệu không được để trống
            if (string.IsNullOrWhiteSpace(model.MaterialName)) {
                return "Material name is required.";
            }

            // Kiểm tra tên vật liệu không được quá dài 100 ký tự
            if (model.MaterialName.Length >100) {
                return "Material name must be less than 100 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức cập nhật vật liệu
        public static string ValidateUpdate(MaterialUpdateVModel model)
        {
            if (model.Id <= 0)
            {
                return "Invalid material ID.";
            }
            
            // Kiểm tra tên vật liệu không được để trống
            if (string.IsNullOrWhiteSpace(model.MaterialName))
            {
                return "Material name is required.";
            }

            // Kiểm tra tên vật liệu không được quá dài 100 ký tự
            if (model.MaterialName.Length > 100)
            {
                return "Material name must be less than 100 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // [1.] Tạo mới vật liệu
        public async Task<ResponseResult> CreateAsync(MaterialCreateVModel model)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra xem tên vật liệu đã tồn tại chưa
                var existingMaterial = await _context.Materials
                    .AnyAsync(m => m.MaterialName.ToLower() == model.MaterialName.ToLower());
                if (existingMaterial) {
                    return new ErrorResponseResult("Material name already exists.");
                }

                // B3: Chuyển ViewModel sang Entity
                var material = model.ToEntity(); 

                // B4: Thêm vật liệu vào DbContext
                _context.Materials.Add(material);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                // B5: Chuyển Entity thành ViewModel để trả về
                var materialVM = material.ToGetVModel(); 
                return new SuccessResponseResult(materialVM, "Material created successfully.");
            }
            catch (Exception ex) { 
                return new ErrorResponseResult($"An error occurred while creating the material. {ex.Message}");
            }
        }

        // [2.] Xóa vật liệu
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try {
                // B1: Tìm vật liệu theo ID
                var material = await _context.Materials
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (material == null) {
                    return new ErrorResponseResult("Material not found.");
                }

                // B2: Ki tra xem vật liệu có đang được sử dụng trong ProductVariant không
                var isUsedInProductVariant = await _context.ProductVariants
                    .AnyAsync(pv => pv.MaterialId == id);
                if (isUsedInProductVariant) {
                    return new ErrorResponseResult("Material cannot be deleted because it is used in one or more product variants.");
                }

                // B3: Xóa vật liệu khỏi DbContext
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                return new SuccessResponseResult(null, "Material deleted successfully.");
            }
            catch (Exception ex) {
                return new ErrorResponseResult($"An error occurred while deleting the material. {ex.Message}");
            }
        }
        // [3.] Lấy tất cả vật liệu
        public async Task<List<MaterialGetVModel>> GetAllAsync()
        {
            // Lấy tất cả vật liệu từ cơ sở dữ liệu và chuyển đổi sang ViewModel
            var materials = await _context.Materials
                .OrderBy(m => m.Id) // Sắp xếp theo Id
                .ToListAsync();
            return materials.Select(m => m.ToGetVModel()).ToList();
        }
        // [4.] Lấy vật liệu theo ID
        public async Task<MaterialGetVModel?> GetByIdAsync(int id)
        {
            // Tìm vật liệu theo ID và chuyển đổi sang ViewModel
            var material = await _context.Materials
                .FirstOrDefaultAsync(m => m.Id == id);
            return material?.ToGetVModel();
        }
        // [5.] Cập nhật vật liệu
        public async Task<ResponseResult> UpdateAsync(MaterialUpdateVModel model)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult)) {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm vật liệu theo ID
                var material = await _context.Materials
                    .FirstOrDefaultAsync(m => m.Id == model.Id);
                if (material == null) {
                    return new ErrorResponseResult($"Material not found with ID: {model.Id}.");
                }

                // B3: Kiểm tra xem tên vật liệu đã tồn tại chưa (trừ vật liệu hiện tại)
                var existingMaterial = await _context.Materials
                    .AnyAsync(m => m.MaterialName.ToLower() == model.MaterialName.ToLower() && m.Id != model.Id);
                if (existingMaterial) {
                    return new ErrorResponseResult("Material name already exists.");
                }

                // B4: Cập nhật thông tin vật liệu từ ViewModel
                material.UpdateEnttity(model);

                // B5: Lưu thay đổi vào DbContext
                await _context.SaveChangesAsync();

                // B6: Chuyển đổi Entity sang ViewModel để trả về
                var materialVM = material.ToGetVModel();
                return new SuccessResponseResult(materialVM ,"Material updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the material. {ex.Message}");
            }
        }
    }
}
