using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System.Text.RegularExpressions;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class SupplierService : ISupplierService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext qua DI
        public SupplierService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation cho phương thức tạo mới nhà cung cấp
        public static string ValidateCreate(SupplierCreateVModel model)
        {
            // Kiểm tra email không được để trống
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email là bắt buộc.";
            }

            // Kiểm tra định dạng email
            if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return "Định dạng email không hợp lệ.";
            }

            // Kiểm tra email không quá 255 ký tự
            if (model.Email.Length > 255)
            {
                return "Email phải dưới 255 ký tự.";
            }

            // Kiểm tra mã số thuế không được để trống
            if (string.IsNullOrWhiteSpace(model.TaxId))
            {
                return "Mã số thuế là bắt buộc.";
            }

            // Kiểm tra mã số thuế không quá 50 ký tự
            if (model.TaxId.Length > 50)
            {
                return "Mã số thuế phải dưới 50 ký tự.";
            }

            // Kiểm tra tên nhà cung cấp nếu có
            if (!string.IsNullOrWhiteSpace(model.SupplierName) && model.SupplierName.Length > 100)
            {
                return "Tên nhà cung cấp phải dưới 100 ký tự.";
            }

            // Kiểm tra tên người liên hệ nếu có
            if (!string.IsNullOrWhiteSpace(model.ContactName) && model.ContactName.Length > 100)
            {
                return "Tên người liên hệ phải dưới 100 ký tự.";
            }

            // Kiểm tra số điện thoại nếu có
            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                if (model.PhoneNumber.Length > 20)
                {
                    return "Số điện thoại phải dưới 20 ký tự.";
                }
                //if (!Regex.IsMatch(model.PhoneNumber, @"^\+?[1-9]\d{1,14}$"))
                //{
                //    return "Invalid phone number format.";
                //}
            }

            // Kiểm tra ghi chú nếu có
            if (!string.IsNullOrWhiteSpace(model.Notes) && model.Notes.Length > 500)
            {
                return "Ghi chú phải dưới 500 ký tự.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức cập nhật nhà cung cấp
        public static string ValidateUpdate(SupplierUpdateVModel model)
        {
            // Kiểm tra Id hợp lệ
            if (model.Id <= 0)
            {
                return "ID nhà cung cấp không hợp lệ.";
            }

            // Áp dụng các validation của Create
            return ValidateCreate(model);
        }

        // [1.] Tạo mới nhà cung cấp
        public async Task<ResponseResult> CreateAsync(SupplierCreateVModel model, string? imageUrl)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra email đã tồn tại chưa
                var existingEmail = await _context.Suppliers
                    .AnyAsync(s => s.Email.ToLower() == model.Email.ToLower());
                if (existingEmail)
                {
                    return new ErrorResponseResult("Email đã tồn tại.");
                }

                // B3: Kiểm tra mã số thuế đã tồn tại chưa
                var existingTaxId = await _context.Suppliers
                    .AnyAsync(s => s.TaxId.ToLower() == model.TaxId.ToLower());
                if (existingTaxId)
                {
                    return new ErrorResponseResult("Mã số thuế đã tồn tại.");
                }

                // B4: Chuyển ViewModel sang Entity
                var supplier = model.ToEntity(imageUrl);

                // B5: Thêm nhà cung cấp vào DbContext
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                // B6: Trả về kết quả thành công
                var supplierVM = supplier.ToGetVModel();
                return new SuccessResponseResult(supplierVM, "Tạo nhà cung cấp thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Có lỗi xảy ra khi tạo nhà cung cấp: {ex.Message}");
            }
        }

        // [2.] Xóa nhà cung cấp
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm nhà cung cấp theo ID
                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (supplier == null)
                {
                    return new ErrorResponseResult($"Supplier not found with ID: {id}.");
                }

                // B2: Kiểm tra xem nhà cung cấp có đang được sử dụng trong sản phẩm không
                var isUsedInProducts = await _context.Products
                    .AnyAsync(p => p.SupplierId == id);
                if (isUsedInProducts)
                {
                    return new ErrorResponseResult("Supplier cannot be deleted because it is associated with one or more products.");
                }

                // B3: Xóa nhà cung cấp
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Supplier deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while deleting the supplier: {ex.Message}");
            }
        }

        // [3.] Lấy tất cả nhà cung cấp
        public async Task<List<SupplierGetVModel>> GetAllAsync()
        {
            // Lấy tất cả nhà cung cấp từ database và chuyển thành ViewModel
            var suppliers = await _context.Suppliers
                .OrderBy(s => s.SupplierName ?? s.Id.ToString()) // Sắp xếp theo tên hoặc ID nếu tên null
                .ToListAsync();
            return suppliers.Select(s => s.ToGetVModel()).ToList();
        }

        // [4.] Lấy nhà cung cấp theo ID
        public async Task<SupplierGetVModel?> GetByIdAsync(int id)
        {
            // Tìm nhà cung cấp theo ID
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id);
            return supplier?.ToGetVModel();
        }

        // [5.] Cập nhật nhà cung cấp
        public async Task<ResponseResult> UpdateAsync(SupplierUpdateVModel model, string? imageUrl = null)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm nhà cung cấp theo ID
                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.Id == model.Id);
                if (supplier == null)
                {
                    return new ErrorResponseResult($"Supplier not found with ID: {model.Id}.");
                }

                // B3: Kiểm tra email đã tồn tại chưa (ngoại trừ nhà cung cấp hiện tại)
                var existingEmail = await _context.Suppliers
                    .AnyAsync(s => s.Email.ToLower() == model.Email.ToLower() && s.Id != model.Id);
                if (existingEmail)
                {
                    return new ErrorResponseResult("Email already exists.");
                }

                // B4: Kiểm tra mã số thuế đã tồn tại chưa (ngoại trừ nhà cung cấp hiện tại)
                var existingTaxId = await _context.Suppliers
                    .AnyAsync(s => s.TaxId.ToLower() == model.TaxId.ToLower() && s.Id != model.Id);
                if (existingTaxId)
                {
                    return new ErrorResponseResult("Tax ID already exists.");
                }

                // B5: Cập nhật thông tin nhà cung cấp
                supplier.UpdateEntity(model, imageUrl);

                // B6: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B7: Chuyển Entity thành ViewModel để trả về
                var supplierVM = supplier.ToGetVModel();
                return new SuccessResponseResult(supplierVM, "Supplier updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the supplier: {ex.Message}");
            }
        }
    }
}