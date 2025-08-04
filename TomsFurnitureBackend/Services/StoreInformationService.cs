using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class StoreInformationService : IStoreInformationService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext qua DI
        public StoreInformationService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation cho phương thức tạo mới thông tin cửa hàng
        public static string ValidateCreate(StoreInformationCreateVModel model)
        {
            // Kiểm tra loại hình kinh doanh không được để trống
            if (string.IsNullOrWhiteSpace(model.BusinessType))
            {
                return "Loại hình kinh doanh là bắt buộc.";
            }

            // Kiểm tra loại hình kinh doanh không quá 100 ký tự
            if (model.BusinessType.Length > 100)
            {
                return "Loại hình kinh doanh phải dưới 100 ký tự.";
            }

            // Kiểm tra tên cửa hàng nếu có
            if (!string.IsNullOrWhiteSpace(model.StoreName) && model.StoreName.Length > 100)
            {
                return "Tên cửa hàng phải dưới 100 ký tự.";
            }

            // Kiểm tra địa chỉ nếu có
            if (!string.IsNullOrWhiteSpace(model.StoreAddress) && model.StoreAddress.Length > 255)
            {
                return "Địa chỉ cửa hàng phải dưới 255 ký tự.";
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

            // Kiểm tra email nếu có
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return "Định dạng email không hợp lệ.";
                }
                if (model.Email.Length > 255)
                {
                    return "Email phải dưới 255 ký tự.";
                }
            }

            // Kiểm tra các link URL nếu có
            var urlRegex = @"^(https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?$";
            if (!string.IsNullOrWhiteSpace(model.LinkWebsite))
            {
                if (!Regex.IsMatch(model.LinkWebsite, urlRegex))
                    return "URL website không hợp lệ.";
                if (model.LinkWebsite.Length > 255)
                    return "URL website phải dưới 255 ký tự.";
            }
            if (!string.IsNullOrWhiteSpace(model.LinkSocialFacebook))
            {
                if (!Regex.IsMatch(model.LinkSocialFacebook, urlRegex))
                    return "URL Facebook không hợp lệ.";
                if (model.LinkSocialFacebook.Length > 255)
                    return "URL Facebook phải dưới 255 ký tự.";
            }
            if (!string.IsNullOrWhiteSpace(model.LinkSocialTwitter))
            {
                if (!Regex.IsMatch(model.LinkSocialTwitter, urlRegex))
                    return "URL Twitter không hợp lệ.";
                if (model.LinkSocialTwitter.Length > 255)
                    return "URL Twitter phải dưới 255 ký tự.";
            }
            if (!string.IsNullOrWhiteSpace(model.LinkSocialInstagram))
            {
                if (!Regex.IsMatch(model.LinkSocialInstagram, urlRegex))
                    return "URL Instagram không hợp lệ.";
                if (model.LinkSocialInstagram.Length > 255)
                    return "URL Instagram phải dưới 255 ký tự.";
            }
            if (!string.IsNullOrWhiteSpace(model.LinkSocialTiktok))
            {
                if (!Regex.IsMatch(model.LinkSocialTiktok, urlRegex))
                    return "URL TikTok không hợp lệ.";
                if (model.LinkSocialTiktok.Length > 255)
                    return "URL TikTok phải dưới 255 ký tự.";
            }
            if (!string.IsNullOrWhiteSpace(model.LinkSocialYoutube))
            {
                if (!Regex.IsMatch(model.LinkSocialYoutube, urlRegex))
                    return "URL YouTube không hợp lệ.";
                if (model.LinkSocialYoutube.Length > 255)
                    return "URL YouTube phải dưới 255 ký tự.";
            }

            // Kiểm tra vĩ độ nếu có
            if (model.Latitude.HasValue && (model.Latitude < -90 || model.Latitude > 90))
            {
                return "Vĩ độ phải nằm trong khoảng từ -90 đến 90.";
            }

            // Kiểm tra kinh độ nếu có
            if (model.Longitude.HasValue && (model.Longitude < -180 || model.Longitude > 180))
            {
                return "Kinh độ phải nằm trong khoảng từ -180 đến 180.";
            }

            // Kiểm tra tên chủ sở hữu nếu có
            if (!string.IsNullOrWhiteSpace(model.OwnerName) && model.OwnerName.Length > 100)
            {
                return "Tên chủ sở hữu phải dưới 100 ký tự.";
            }

            // Kiểm tra giờ hoạt động nếu có
            if (!string.IsNullOrWhiteSpace(model.OperatingHours) && model.OperatingHours.Length > 100)
            {
                return "Giờ hoạt động phải dưới 100 ký tự.";
            }

            // Kiểm tra mô tả cửa hàng nếu có
            if (!string.IsNullOrWhiteSpace(model.StoreDescription) && model.StoreDescription.Length > 1000)
            {
                return "Mô tả cửa hàng phải dưới 1000 ký tự.";
            }

            // Kiểm tra mã số thuế nếu có
            if (!string.IsNullOrWhiteSpace(model.TaxId) && model.TaxId.Length > 50)
            {
                return "Mã số thuế phải dưới 50 ký tự.";
            }

            // Kiểm tra mã chi nhánh nếu có
            if (!string.IsNullOrWhiteSpace(model.BranchCode) && model.BranchCode.Length > 50)
            {
                return "Mã chi nhánh phải dưới 50 ký tự.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức cập nhật thông tin cửa hàng
        public static string ValidateUpdate(StoreInformationUpdateVModel model)
        {
            // Kiểm tra Id hợp lệ
            if (model.Id <= 0)
            {
                return "ID thông tin cửa hàng không hợp lệ.";
            }

            // Áp dụng các validation của Create
            return ValidateCreate(model);
        }

        // [1.] Tạo mới thông tin cửa hàng
        public async Task<ResponseResult> CreateAsync(StoreInformationCreateVModel model, string? logoUrl)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra chỉ có một bản ghi IsActive = true
                var activeStore = await _context.StoreInformations
                    .AnyAsync(s => s.IsActive == true);
                if (activeStore)
                {
                    return new ErrorResponseResult("Đã tồn tại một bản ghi thông tin cửa hàng đang hoạt động.");
                }

                // B3: Kiểm tra email nếu có
                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    var existingEmail = await _context.StoreInformations
                        .AnyAsync(s => s.Email != null && s.Email.ToLower() == model.Email.ToLower());
                    if (existingEmail)
                    {
                        return new ErrorResponseResult("Email đã tồn tại.");
                    }
                }

                // B4: Kiểm tra mã số thuế nếu có
                if (!string.IsNullOrWhiteSpace(model.TaxId))
                {
                    var existingTaxId = await _context.StoreInformations
                        .AnyAsync(s => s.TaxId != null && s.TaxId.ToLower() == model.TaxId.ToLower());
                    if (existingTaxId)
                    {
                        return new ErrorResponseResult("Mã số thuế đã tồn tại.");
                    }
                }

                // B5: Chuyển ViewModel sang Entity
                var storeInformation = model.ToEntity(logoUrl);

                // B6: Thêm thông tin cửa hàng vào DbContext
                _context.StoreInformations.Add(storeInformation);
                await _context.SaveChangesAsync();

                // B7: Trả về kết quả thành công
                var storeInformationVM = storeInformation.ToGetVModel();
                return new SuccessResponseResult(storeInformationVM, "Thêm mới thông tin cửa hàng thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Đã xảy ra lỗi khi tạo mới thông tin cửa hàng: {ex.Message}");
            }
        }

        // [2.] Xóa thông tin cửa hàng
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm thông tin cửa hàng theo ID
                var storeInformation = await _context.StoreInformations
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (storeInformation == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy thông tin cửa hàng với ID: {id}.");
                }

                // B2: Xóa thông tin cửa hàng
                _context.StoreInformations.Remove(storeInformation);
                await _context.SaveChangesAsync();

                // B3: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Xóa thông tin cửa hàng thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Đã xảy ra lỗi khi xóa thông tin cửa hàng: {ex.Message}");
            }
        }

        // [3.] Lấy tất cả thông tin cửa hàng
        public async Task<List<StoreInformationGetVModel>> GetAllAsync()
        {
            // Lấy tất cả thông tin cửa hàng từ database và chuyển thành ViewModel
            var storeInformations = await _context.StoreInformations
                .OrderBy(s => s.StoreName ?? s.Id.ToString()) // Sắp xếp theo tên hoặc ID nếu tên null
                .ToListAsync();
            return storeInformations.Select(s => s.ToGetVModel()).ToList();
        }

        // [4.] Lấy thông tin cửa hàng theo ID
        public async Task<StoreInformationGetVModel?> GetByIdAsync(int id)
        {
            // Tìm thông tin cửa hàng theo ID
            var storeInformation = await _context.StoreInformations
                .FirstOrDefaultAsync(s => s.Id == id);
            return storeInformation?.ToGetVModel();
        }

        // [5.] Cập nhật thông tin cửa hàng
        public async Task<ResponseResult> UpdateAsync(StoreInformationUpdateVModel model, string? logoUrl = null)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm thông tin cửa hàng theo ID
                var storeInformation = await _context.StoreInformations
                    .FirstOrDefaultAsync(s => s.Id == model.Id);
                if (storeInformation == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy thông tin cửa hàng với ID: {model.Id}.");
                }

                // B3: Kiểm tra chỉ có một bản ghi IsActive = true
                if (model.IsActive == true)
                {
                    var activeStore = await _context.StoreInformations
                        .AnyAsync(s => s.IsActive == true && s.Id != model.Id);
                    if (activeStore)
                    {
                        return new ErrorResponseResult("Đã tồn tại một bản ghi thông tin cửa hàng đang hoạt động khác.");
                    }
                }

                // B4: Kiểm tra email nếu có
                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    var existingEmail = await _context.StoreInformations
                        .AnyAsync(s => s.Email != null && s.Email.ToLower() == model.Email.ToLower() && s.Id != model.Id);
                    if (existingEmail)
                    {
                        return new ErrorResponseResult("Email đã tồn tại.");
                    }
                }

                // B5: Kiểm tra mã số thuế nếu có
                if (!string.IsNullOrWhiteSpace(model.TaxId))
                {
                    var existingTaxId = await _context.StoreInformations
                        .AnyAsync(s => s.TaxId != null && s.TaxId.ToLower() == model.TaxId.ToLower() && s.Id != model.Id);
                    if (existingTaxId)
                    {
                        return new ErrorResponseResult("Mã số thuế đã tồn tại.");
                    }
                }

                // B6: Cập nhật thông tin cửa hàng
                storeInformation.UpdateEntity(model, logoUrl);

                // B7: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B8: Chuyển Entity thành ViewModel để trả về
                var storeInformationVM = storeInformation.ToGetVModel();
                return new SuccessResponseResult(storeInformationVM, "Cập nhật thông tin cửa hàng thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Đã xảy ra lỗi khi cập nhật thông tin cửa hàng: {ex.Message}");
            }
        }
    }
}