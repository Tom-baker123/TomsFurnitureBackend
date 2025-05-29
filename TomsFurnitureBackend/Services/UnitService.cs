using OA.Domain.Common.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TomsFurnitureBackend.Services
{
    public class UnitService : IUnitService
    {
        private readonly TomfurnitureContext _context;
        public UnitService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Chỉ lấy sang VModel để sử dụng trong Service này
        }

        // Validation Phương thức thêm 
        public static string ValidateCreate(UnitCreateVModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UnitName))
            {
                return "Tên đơn vị không được để trống.";
            }
            if (model.UnitName.Length > 50)
            {
                return "Tên đơn vị không được vượt quá 50 ký tự.";
            }
            if (model.Description != null && model.Description.Length > 200)
            {
                return "Mô tả không được vượt quá 200 ký tự.";
            }
            return string.Empty;
        }
        // Validation Phương thức cập nhật
        public static string ValidateUpdate(UnitUpdateVModel model)
        {
            if (model.Id <= 0)
            {
                return "ID không hợp lệ.";
            }
            if (string.IsNullOrWhiteSpace(model.UnitName))
            {
                return "Tên đơn vị không được để trống.";
            }
            if (model.UnitName.Length > 50)
            {
                return "Tên đơn vị không được vượt quá 50 ký tự.";
            }
            if (model.Description != null && model.Description.Length > 200)
            {
                return "Mô tả không được vượt quá 200 ký tự.";
            }
            return string.Empty;
        }
        // Phương thức tạo mới đơn vị
        public async Task<ResponseResult> CreateAsync(UnitCreateVModel model)
        {
            try
            {
                // kiểm tra dữ liệu đầu vào (kq = empty là đúng)
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // Kiểm tra có trùng untiName không
                var existingUnit = await _context.Units.
                    FirstOrDefaultAsync(u => u.UnitName.ToLower() == model.UnitName.ToLower());

                // Chuyển đổi sang Entity để lưu vào cơ sở dữ liệu
                var unit = model.ToEntity();
                _context.Units.Add(unit);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                // Chuyển đổi lại sang VModel để trả về kết quả.
                var unitVM = unit.ToGetVModel();

                return new SuccessResponseResult(unitVM, "Add Unit Success!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while creating the unit: {ex.Message}");
            }
        }
        // Phương thức xóa đơn vị theo ID
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // Kiểm tra đơn vị có tồn tại không
                var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == id);

                if (unit == null)
                {
                    return new ErrorResponseResult($"Id: {id} not found!");
                }

                _context.Units.Remove(unit); // Xóa đơn vị
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                return new SuccessResponseResult(id, "Delete Unit Success!");
            }
            catch
            {
                return new ErrorResponseResult("An error occurred while deleting the unit.");
            }
        }
        // Phương thức lấy tất cả đơn vị
        public async Task<List<UnitGetVModel>> GetAllAsync()
        {

            var units = await _context.Units
                .OrderBy(u => u.Id)
                .ToListAsync();
            return [.. units.Select(u => u.ToGetVModel())];
        }
        // Phương thức lấy đơn vị theo ID
        public async Task<UnitGetVModel?> GetByIdAsync(int id)
        {
            var unit = await _context.Units
                .FirstOrDefaultAsync(u => u.Id == id);
            return unit?.ToGetVModel();
        }

        // Phương thức cập nhật đơn vị
        public async Task<ResponseResult> UpdateAsync(UnitUpdateVModel model)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // Tìm Unit theo ID
                var unit = await _context.Units
                    .FirstOrDefaultAsync(u => u.Id == model.Id);
                if (unit == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy đơn vị với ID: {model.Id}.");
                }

                // Kiểm tra có trùng tên đơn vị không
                var existingUnit = await _context.Units
                    .FirstOrDefaultAsync(u => u.UnitName.ToLower() == model.UnitName.ToLower() && u.Id != model.Id);
                if (existingUnit != null)
                {
                    return new ErrorResponseResult("Tên đơn vị đã tồn tại với ID khác.");
                }

                // Cập nhật thông tin Unit từ VModel sang Entity (không cập nhật Id)
                unit.UpdateEntity(model);
                await _context.SaveChangesAsync();

                // Chuyển đổi lại sang VModel để trả về kết quả
                var unitVM = unit.ToGetVModel();
                return new SuccessResponseResult(unitVM, "Cập nhật đơn vị thành công!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi cập nhật đơn vị: {ex.Message}");
            }
        }
    }
}
