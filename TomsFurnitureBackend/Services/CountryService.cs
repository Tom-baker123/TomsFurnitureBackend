using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class CountryService : ICountryService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext qua DI
        public CountryService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation cho phương thức tạo mới xuất xứ
        public static string ValidateCreate(CountryCreateVModel model)
        {
            // Kiểm tra tên xuất xứ không được để trống
            if (string.IsNullOrWhiteSpace(model.CountryName))
            {
                return "Country name is required.";
            }

            // Kiểm tra tên xuất xứ không được quá dài 100 ký tự
            if (model.CountryName.Length > 100)
            {
                return "Country name must be less than 100 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức cập nhật xuất xứ
        public static string ValidateUpdate(CountryUpdateVModel model)
        {
            // Kiểm tra Id hợp lệ
            if (model.Id <= 0)
            {
                return "Invalid country ID.";
            }

            // Kiểm tra tên xuất xứ không được để trống
            if (string.IsNullOrWhiteSpace(model.CountryName))
            {
                return "Country name is required.";
            }

            // Kiểm tra tên xuất xứ không được quá dài 100 ký tự
            if (model.CountryName.Length > 100)
            {
                return "Country name must be less than 100 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // [1.] Tạo mới xuất xứ
        public async Task<ResponseResult> CreateAsync(CountryCreateVModel model, string? imageUrl)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra xem tên xuất xứ đã tồn tại chưa
                var existingCountry = await _context.Countries
                    .AnyAsync(c => c.CountryName.ToLower() == model.CountryName.ToLower());
                if (existingCountry)
                {
                    return new ErrorResponseResult("Country name already exists.");
                }

                // B3: Chuyển ViewModel sang Entity
                var country = model.ToEntity(imageUrl);

                // B4: Thêm xuất xứ vào DbContext
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();

                // B5: Trả về kết quả thành công
                var countryVM = country.ToGetVModel();
                return new SuccessResponseResult(countryVM, "Country created successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while creating the country: {ex.Message}");
            }
        }

        // [2.] Xóa xuất xứ
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm xuất xứ theo ID
                var country = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (country == null)
                {
                    return new ErrorResponseResult($"Country not found with ID: {id}.");
                }

                // B2: Kiểm tra xem xuất xứ có đang được sử dụng trong sản phẩm không
                var isUsedInProducts = await _context.Products
                    .AnyAsync(p => p.CountriesId == id);
                if (isUsedInProducts)
                {
                    return new ErrorResponseResult("Country cannot be deleted because it is associated with one or more products.");
                }

                // B3: Xóa xuất xứ
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Country deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while deleting the country: {ex.Message}");
            }
        }

        // [3.] Lấy tất cả xuất xứ
        public async Task<List<CountryGetVModel>> GetAllAsync()
        {
            // Lấy tất cả xuất xứ từ database và chuyển thành ViewModel
            var countries = await _context.Countries
                .OrderBy(c => c.CountryName) // Sắp xếp theo tên xuất xứ
                .ToListAsync();
            return countries.Select(c => c.ToGetVModel()).ToList();
        }

        // [4.] Lấy xuất xứ theo ID
        public async Task<CountryGetVModel?> GetByIdAsync(int id)
        {
            // Tìm xuất xứ theo ID
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Id == id);
            return country?.ToGetVModel();
        }

        // [5.] Cập nhật xuất xứ
        public async Task<ResponseResult> UpdateAsync(CountryUpdateVModel model, string? imageUrl = null)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm xuất xứ theo ID
                var country = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Id == model.Id);
                if (country == null)
                {
                    return new ErrorResponseResult($"Country not found with ID: {model.Id}.");
                }

                // B3: Kiểm tra xem tên xuất xứ đã tồn tại chưa (ngoại trừ xuất xứ hiện tại)
                var existingCountry = await _context.Countries
                    .AnyAsync(c => c.CountryName.ToLower() == model.CountryName.ToLower() && c.Id != model.Id);
                if (existingCountry)
                {
                    return new ErrorResponseResult("Country name already exists.");
                }

                // B4: Cập nhật thông tin xuất xứ
                country.UpdateEntity(model, imageUrl);

                // B5: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B6: Chuyển Entity thành ViewModel để trả về
                var countryVM = country.ToGetVModel();
                return new SuccessResponseResult(countryVM, "Country updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the country: {ex.Message}");
            }
        }
    }
}