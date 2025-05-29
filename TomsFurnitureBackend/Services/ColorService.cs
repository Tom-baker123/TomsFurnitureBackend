using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System.Text.RegularExpressions;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class ColorService : IColorService
    {
        // Gọi context từ DI container để truy cập dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext cho việc truy cập dữ liệu
        public ColorService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ----- [Validation phương thức tạo mới màu sắc] -------------------------
        public static string ValidateCreate(ColorCreateVModel model)
        {
            // Kiểm tra tên màu sắc không được để trống
            if (string.IsNullOrWhiteSpace(model.ColorName))
            {
                return "Color name is required.";
            }

            // Kiểm tra độ dài tên màu sắc (tối đa 50 ký tự)
            if (model.ColorName.Length > 50)
            {
                return "ColorName must be less than 50 characters.";
            }

            // Kiểm tra mã màu sắc phù hợp không?
            if (string.IsNullOrEmpty(model.ColorCode))
            {
                //if (!Regex.IsMatch(model.ColorCode, @"^#[0-9A-Fa-f]{6}$")) {
                //    return "Mã màu phải là định dạng hex hợp lệ (ví dụ: #FFFFFF).";
                //}
                return "ColorCode not right format!";
            }

            // Kiểm tra trạng thái hoạt động (IsActive) phải là true hoặc false
            //if (model.IsActive.HasValue && model.IsActive != true && model.IsActive != false)
            //{
            //    return "IsActive must be true or false.";
            //}

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }
        // ----- [Validation phương thức tạo mới màu sắc] -------------------------
        public static string ValidateUpdate(ColorUpdateVModel model)
        {
            // Kiểm tra Id có giá trị hợp lệ
            if (model.Id <= 0)
            {
                return "Id must be greater than 0.";
            }

            // Kiem tra tên màu sắc không được để trống
            if (string.IsNullOrWhiteSpace(model.ColorName))
            {
                return "Color name is required.";
            }

            // Kiểm tra độ dài tên màu sắc (tối đa 50 ký tự)
            if (model.ColorName.Length > 50)
            {
                return "ColorName must be less than 50 characters.";
            }

            // Kiểm tra mã màu sắc phù hợp không?
            if (string.IsNullOrEmpty(model.ColorCode))
            {
                //if (!Regex.IsMatch(model.ColorCode, @"^#[0-9A-Fa-f]{6}$")) {
                //    return "Mã màu phải là định dạng hex hợp lệ (ví dụ: #FFFFFF).";
                //}
                return "ColorCode not right format!";
            }

            if (model.IsActive.HasValue && model.IsActive != true && model.IsActive != false)
            {
                return "IsActive must be true or false.";
            }
            // Kiểm tra tên màu sắc không được để trống
            return string.Empty;
        }


        // [01.] --- Phương thức tạo mới màu sắc
        public async Task<ResponseResult> CreateAsync(ColorCreateVModel model)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào hợp lệ không?
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // - B2: Kiểm tra xem tên màu sắc đã tồn tại chưa?
                // + Trả về true nếu có ít nhất một phần tử trong tập hợp thỏa điều kiện.
                // + Trả về false nếu không có phần tử nào thỏa điều kiện.
                var existingColor = await _context.Colors
                    .AnyAsync(c => c.ColorName.ToLower() == model.ColorName.ToLower());
                if (existingColor)
                {
                    return new ErrorResponseResult("Color name already exists.");
                }

                // B3: Chuyển VModel sang Entity
                var color = model.ToEntity(); // Phương thức mở rộng ToEntity() để chuyển đổi VModel sang Entity

                // B4: Thêm màu sắc vào DbContext
                _context.Colors.Add(color);
                await _context.SaveChangesAsync();

                // B5: Chuyển đổi Entity sang VModel để trả về
                var colorVM = color.ToGetVModel(); // Phương thức mở rộng ToGetVModel() để chuyển đổi Entity sang VModel
                return new SuccessResponseResult(colorVM, "You have Added color success!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"You have an error when creating color: {ex.Message}");
            }
        }

        // [02.] --- Phương thức xóa màu sắc theo ID
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // Tìm damh mục theo ID
                var color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
                if (color == null)
                {
                    return new ErrorResponseResult($"Id {id} not found");
                }

                // Kiểm tra xem màu sắc có đang được sử dụng trong ProductVariant không không?
                var isUsedInProductVariant = await _context.ProductVariants.AnyAsync(pv => pv.Id == id);
                if (isUsedInProductVariant) {
                    return new ErrorResponseResult("This color is being used in ProductVariant and cannot be deleted.");
                }

                // Xóa màu sắc nếu tồn tại
                _context.Colors.Remove(color);
                await _context.SaveChangesAsync();

                return new SuccessResponseResult(color, "You have deleted your color!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"You have an error when deleted color: {ex.Message}");
            }
        }

        // [03.] --- Phương thức lấy tất cả màu sắc
        public async Task<List<ColorGetVModel>> GetAllAsync()
        {
            // Lấy tất cả màu sắc từ database và chuyển thành ViewModel
            var colors = await _context.Colors
                .OrderBy(c => c.Id) // Sắp xếp theo Id màu sắc
                .ToListAsync(); // Chuyển đổi sang danh sách
            return colors.Select(c => c.ToGetVModel()).ToList();
        }

        // [04.] --- Phương thức lấy màu sắc theo ID
        public async Task<ColorGetVModel?> GetByIdAsync(int id)
        {
            // Tìm màu sắc theo Id
            var color = await _context.Colors
                .FirstOrDefaultAsync(c => c.Id == id);
            return color?.ToGetVModel(); // Chuyển đổi sang ViewModel nếu tìm thấy
        }

        // [05.] --- Phương thức cập nhật màu sắc
        public async Task<ResponseResult> UpdateAsync(ColorUpdateVModel model)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào hợp lệ không?
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm màu sắc theo ID
                // + Nếu không tìm thấy màu sắc với Id này, trả về lỗi
                var color = await _context.Colors
                    .FirstOrDefaultAsync(c => c.Id == model.Id);
                if (color == null)
                {
                    return new ErrorResponseResult("Id not found");
                }

                // B3: Kiểm tra xem tên màu sắc đã tồn tại chưa? (trừ tên màu sắc hiện tại)
                var existingColor = await _context.Colors
                    .AnyAsync(c => c.ColorName.ToLower() == model.ColorName.ToLower() && c.Id != model.Id);
                if (existingColor)
                {
                    return new ErrorResponseResult("Color name already exists.");
                }

                // B4: Cập nhật thông tin màu sắc từ model
                color.UpdateEntity(model);

                // B5: Lưu thay đổi vào DbContext
                await _context.SaveChangesAsync();

                // B6: Chuyển đổi Entity sang ViewModel để trả về Success
                var colorVM = color.ToGetVModel(); // Phương thức ToGetVModel() để chuyển đổi Entity sang VModel
                return new SuccessResponseResult(colorVM, "You have updated your color successfully!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"You have an error when updating color: {ex.Message}");
            }
        }
    }
}
