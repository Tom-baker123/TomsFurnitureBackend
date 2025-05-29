using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using System.Linq;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace TomsFurnitureBackend.Services
{
    public class CategoryService : ICategoryService
    {
        // + Định nghĩa các phương thức cần thiết để quản lý danh mục: lấy tất cả, lấy theo ID, tạo mới, xóa, và cập nhật.
        // + Sử dụng ResponseResult để trả về kết quả thành công hoặc lỗi.

        // Gọi context để truy cập dữ liệu từ cơ sở dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext
        public CategoryService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ----- [Validation phương thức tạo mới danh mục] -------------------------
        public static string ValidateCreate(CategoryCreateVModel model)
        {
            // Kiểm tra tên danh mục không được để trống
            if (string.IsNullOrWhiteSpace(model.CategoryName)) {
                return "Category name is required.";
            }
            
            // Kiểm tra độ dải tên danh mục (tối đa 50 ký tự)
            if (model.CategoryName.Length > 50) {
                return "CategoryName must be less than 50 characters.";
            }

            // Kiểm tra mô tả không được quá 255 ký tự
            if (model.Descriptions != null && model.Descriptions.Length > 255) {
                return "Descriptions must be less than 255 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }
        // ----- [Validation phương thức tạo mới danh mục] -------------------------
        public static string ValidateUpdate(CategoryUpdateVModel model)
        {
            // Kiểm tra Id có giá trị hợp lệ
            if (model.Id <= 0) {
                return "Id must be greater than 0.";
            }

            // Kiem tra tên danh mục không được để trống
            if (string.IsNullOrWhiteSpace(model.CategoryName)) {
                return "Category name is required.";
            }

            // Kiểm tra độ dài tên danh mục (tối đa 50 ký tự)
            if (model.CategoryName.Length > 50) {
                return "CategoryName must be less than 50 characters.";
            }

            // Kiểm tra mô tả không được quá 255 ký tự
            if (model.Descriptions?.Length > 255) {
                return "Descriptions must be less than 255 characters.";
            }
            
            if (model.IsActive == null)
            {
                return "IsActive must be true or false.";
            }
            // Kiểm tra tên danh mục không được để trống
            return string.Empty;
        }


        // [01.] --- Phương thức tạo mới danh mục
        public async Task<ResponseResult> CreateAsync(CategoryCreateVModel model, string imageUrl)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào hợp lệ không?
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult)) {
                    return new ErrorResponseResult(validationResult);
                }

                // - B2: Kiểm tra xem tên danh mục đã tồn tại chưa?
                    // + Trả về true nếu có ít nhất một phần tử trong tập hợp thỏa điều kiện.
                    // + Trả về false nếu không có phần tử nào thỏa điều kiện.
                var existingCategory = await _context.Categories
                    .AnyAsync(c => c.CategoryName.ToLower() == model.CategoryName.ToLower());
                if (existingCategory) {
                    return new ErrorResponseResult("Category name already exists.");
                }

                // B3: Chuyển VModel sang Entity
                var category = model.ToEntity(); // Phương thức mở rộng ToEntity() để chuyển đổi VModel sang Entity

                // B4: Gán đường dẫn ảnh cloudinary nếu có
                category.ImageUrl = imageUrl;

                // B5: Thêm danh mục vào DbContext
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // B6: Chuyển đổi Entity sang VModel để trả về
                var categoryVM = category.ToGetVModel(); // Phương thức mở rộng ToGetVModel() để chuyển đổi Entity sang VModel
                return new SuccessResponseResult(categoryVM, "You have Added category success!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"You have an error when creating category: {ex.Message}");
            }
        }

        // [02.] --- Phương thức xóa danh mục theo ID
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // Tìm damh mục theo ID
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category == null) {
                    return new ErrorResponseResult($"Id {id} not found");
                }

                // Xóa danh mục nếu tồn tại
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return new SuccessResponseResult(category, "You have deleted your category!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"You have an error when deleted category: {ex.Message}");
            }
        }

        // [03.] --- Phương thức lấy tất cả danh mục
        public async Task<List<CategoryGetVModel>> GetAllAsync()
        {
            // Lấy tất cả danh mục từ database và chuyển thành ViewModel
            var categories = await _context.Categories
                .OrderBy(c => c.Id).ToListAsync(); // Sắp xếp theo Id danh mục
            return categories.Select(c => c.ToGetVModel()).ToList();
        }

        // [04.] --- Phương thức lấy danh mục theo ID
        public async Task<CategoryGetVModel?> GetByIdAsync(int id)
        {
            // Tìm danh mục theo Id
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
            return category?.ToGetVModel(); // Chuyển đổi sang ViewModel nếu tìm thấy
        }

        // [05.] --- Phương thức cập nhật danh mục
        public async Task<ResponseResult> UpdateAsync(CategoryUpdateVModel model, string? imageUrl = null)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào hợp lệ không?
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult)) {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm danh mục theo ID
                    // + Nếu không tìm thấy danh mục với Id này, trả về lỗi
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == model.Id);
                if (category == null)
                {
                    return new ErrorResponseResult("Id not found");
                }

                // B3: Kiểm tra xem tên danh mục đã tồn tại chưa? (trừ tên danh mục hiện tại)
                var existingCategory = await _context.Categories
                    .AnyAsync(c => c.CategoryName.ToLower() == model.CategoryName.ToLower() && c.Id != model.Id);
                if (existingCategory) {
                    return new ErrorResponseResult("Category name already exists.");
                }

                // B4: Cập nhật thông tin danh mục từ model
                category.UpdateEntity(model);

                // B5: Cập nhật ImageUrl nếu có
                if (!string.IsNullOrEmpty(imageUrl)) {
                    category.ImageUrl = imageUrl;
                }

                // Lưu thay đổi vào DbContext
                await _context.SaveChangesAsync();

                // Chuyển đổi Entity sang ViewModel để trả về thông báo thành công
                var categoryVM = category.ToGetVModel(); // Phương thức ToGetVModel() để chuyển đổi Entity sang VModel
                return new SuccessResponseResult(categoryVM, "You have updated your category successfully!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"You have an error when updating category: {ex.Message}");
            }
        }
    }
}
