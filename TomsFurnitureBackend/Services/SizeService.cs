using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class SizeService : ISizeService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext qua DI
        public SizeService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation cho phương thức tạo mới kích thước
        public static string ValidateCreate(SizeCreateVModel model)
        {
            // Kiểm tra tên kích thước không được để trống
            if (string.IsNullOrWhiteSpace(model.SizeName))
            {
                return "Size name is required.";
            }

            // Kiểm tra tên kích thước không được quá dài 50 ký tự
            if (model.SizeName.Length > 50)
            {
                return "Size name must be less than 50 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức cập nhật kích thước
        public static string ValidateUpdate(SizeUpdateVModel model)
        {
            // Kiểm tra tên kích thước không được để trống
            if (string.IsNullOrWhiteSpace(model.SizeName))
            {
                return "Size name is required.";
            }

            // Kiểm tra tên kích thước không được quá dài 50 ký tự
            if (model.SizeName.Length > 50)
            {
                return "Size name must be less than 50 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // [1.] Tạo mới kích thước
        public async Task<ResponseResult> CreateAsync(SizeCreateVModel model)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra xem tên kích thước đã tồn tại chưa
                var existingSize = await _context.Sizes
                    .AnyAsync(s => s.SizeName.ToLower() == model.SizeName.ToLower());
                if (existingSize)
                {
                    return new ErrorResponseResult("Size name already exists.");
                }

                // B3: Chuyển ViewModel sang Entity
                var size = model.ToEntity();

                // B4: Thêm kích thước vào DbContext
                _context.Sizes.Add(size);
                await _context.SaveChangesAsync();

                // B5: Trả về kết quả thành công
                var sizeVM = size.ToGetVModel();
                return new SuccessResponseResult(sizeVM, "Size created successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while creating the size: {ex.Message}");
            }
        }

        // [2.] Xóa kích thước
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm kích thước theo ID
                var size = await _context.Sizes
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (size == null)
                {
                    return new ErrorResponseResult($"Size not found with ID: {id}.");
                }

                // B2: Kiểm tra xem kích thước có đang được sử dụng trong biến thể sản phẩm không
                var isUsedInProducts = await _context.ProductVariants
                    .AnyAsync(pv => pv.SizeId == id);
                if (isUsedInProducts)
                {
                    return new ErrorResponseResult("Size cannot be deleted because it is associated with one or more products.");
                }

                // B3: Xóa kích thước
                _context.Sizes.Remove(size);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Size deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while deleting the size: {ex.Message}");
            }
        }
        

        // [3.] Lấy tất cả kích thước
        public async Task<List<SizeGetVModel>> GetAllAsync()
        {
            // Lấy tất cả kích thước từ database và chuyển thành ViewModel
            var sizes = await _context.Sizes
                .OrderBy(s => s.SizeName) // Sắp xếp theo tên kích thước
                .ToListAsync();
            return sizes.Select(s => s.ToGetVModel()).ToList();
        }

        // [4.] Lấy kích thước theo ID
        public async Task<SizeGetVModel>? GetByIdAsync(int id)
        {
            // Tìm kích thước theo ID
            var size = await _context.Sizes
                .FirstOrDefaultAsync(s => s.Id == id);
            return size?.ToGetVModel();
        }

        // [5.] Cập nhật kích thước
        public async Task<ResponseResult> UpdateAsync(SizeUpdateVModel model)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm kích thước theo ID
                var size = await _context.Sizes
                    .FirstOrDefaultAsync(s => s.Id == model.Id);
                if (size == null)
                {
                    return new ErrorResponseResult($"Size not found with ID: {model.Id}.");
                }

                // B3: Kiểm tra xem tên kích thước đã tồn tại chưa (ngoại trừ kích thước hiện tại)
                var existingSize = await _context.Sizes
                    .AnyAsync(s => s.SizeName.ToLower() == model.SizeName.ToLower() && s.Id != model.Id);
                if (existingSize)
                {
                    return new ErrorResponseResult("Size name already exists.");
                }

                // B4: Cập nhật thông tin kích thước
                size.UpdateEntity(model);

                // B5: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B6: Chuyển Entity thành ViewModel để trả về
                var sizeVM = size.ToGetVModel();
                return new SuccessResponseResult(sizeVM, "Size updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the size: {ex.Message}");
            }
        }
    }
}