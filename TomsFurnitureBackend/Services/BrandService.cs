using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System.ClientModel.Primitives;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class BrandService : IBrandService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext? _context;

        // Constructor nhận DbContext qua DI
        public BrandService(TomfurnitureContext context)
        {
            _context = context;
        }

        // Validation cho phương thức tạo mới thương hiệu
        public static string ValidateCreate(BrandCreateVModel model)
        {
            // Kiểm tra tên thương hiệu không được để trống
            if (string.IsNullOrWhiteSpace(model.BrandName))
            {
                return "Brand name is required.";
            }

            // Kiểm tra tên thương hiệu không được quá dài 100 ký tự
            if (model.BrandName.Length > 100)
            {
                return "Brand name must be less than 100 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức cập nhật thương hiệu
        public static string ValidateUpdate(BrandUpdateVModel model)
        {
            if (model.Id <= 0)
            {
                return "Invalid Brand ID.";
            }

            // Kiểm tra tên thương hiệu không được để trống
            if (string.IsNullOrWhiteSpace(model.BrandName))
            {
                return "Brand name is required.";
            }

            // Kiểm tra tên thương hiệu không được quá dài 100 ký tự
            if (model.BrandName.Length > 100)
            {
                return "Brand name must be less than 100 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // [1.] Tạo mới thương hiệu
        public async Task<ResponseResult> CreateAsync(BrandCreateVModel model, string imageUrl)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra xem tên thương hiệu đã tồn tại chưa
                var existingBrand = await _context.Brands
                    .AnyAsync(b => b.BrandName.ToLower() == model.BrandName.ToLower());
                if (existingBrand) {
                    return new ErrorResponseResult("Brand name already exists.");
                }

                // B3: Chuyển ViewModel sang Entity (Để lưu vào DB)
                var brand = model.ToEntity(imageUrl);

                // B4: Thêm thương hiệu vào DbContext
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();

                // B5: Trả về kết quả thành công
                var brandVM = brand.ToGetVModel();
                return new SuccessResponseResult(brandVM, "Brand created successfully.");
            }
            catch (Exception ex) { 
                return new ErrorResponseResult("An error occurred while creating the brand: " + ex.Message);
            }
        }
        // [2.] Xóa thương hiệu
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm thương hiệu theo ID
                var brand = await _context.Brands
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (brand == null)
                {
                    return new ErrorResponseResult("Brand not found.");
                }

                // B2: Kiểm tra xem thương hiệu có đang được sử dụng trong sản phẩm không
                var isUsedInProducts = await _context.Products
                    .AnyAsync(p => p.BrandId == id);
                if (isUsedInProducts)
                {
                    return new ErrorResponseResult("Brand cannot be deleted because it is associated with one or more products.");
                }

                // B3: Xóa thương hiệu
                _context.Remove(brand);
                await _context.SaveChangesAsync();

                return new SuccessResponseResult("Brand deleted successfully.");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return new ErrorResponseResult("An error occurred while deleting the brand: " + ex.Message);
            }
        }
        // [3.] Lấy tất cả thương hiệu
        public async Task<List<BrandGetVModel>> GetAllAsync()
        {
            // Lấy tất cả thương hiệu từ database và chuyển thành ViewModel
            var brands = await _context.Brands
                .OrderBy(b => b.Id)
                .ToListAsync();
            return brands.Select(b => b.ToGetVModel()).ToList();
        }
        // [4.] Lấy thương hiệu theo ID
        public async Task<BrandGetVModel?> GetByIdAsync(int id)
        {
            // Tìm thương hiệu theo ID
            var brands = await _context.Brands
                .FirstOrDefaultAsync(b => b.Id == id);

            return brands?.ToGetVModel();
        }
        // [5.] Cập nhật thương hiệu
        public async Task<ResponseResult> UpdateAsync(BrandUpdateVModel model, string? imageUrl = null)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult)) {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm thương hiệu theo ID
                var brand = await _context.Brands
                    .FirstOrDefaultAsync(b => b.Id == model.Id);
                if (brand == null) {
                    return new ErrorResponseResult($"Brand not found with ID: {model.Id}.");
                }

                // B3: Kiểm tra xem tên thương hiệu đã tồn tại chưa (ngoại trừ thương hiệu hiện tại)
                var existingBrand = await _context.Brands
                    .AnyAsync(b => b.BrandName.ToLower() == model.BrandName.ToLower() && b.Id != model.Id);
                if (existingBrand)
                {
                    return new ErrorResponseResult("Brand name already exists.");
                }

                // B4: Cập nhật thông tin thương hiệu
                brand.UpdateEntity(model, imageUrl);

                // B5: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B6: Chuyển Entity thành ViewModel để trả về
                var brandVM = brand.ToGetVModel();
                return new SuccessResponseResult(brandVM, "Brand updated successfully.");
            } catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the brand. {ex.Message}");
            }
        }
    }
}
