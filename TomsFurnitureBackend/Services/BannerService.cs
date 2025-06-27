using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services
{
    public class BannerService : IBannerService
    {
        private readonly TomfurnitureContext _context;

        public BannerService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation chung cho cả Create và Update
        public static string Validate(BannerCreateVModel model, bool isUpdate = false, int id = 0)
        {
            // Kiểm tra Id cho Update
            if (isUpdate && id <= 0)
                return "Invalid Banner ID.";

            // Kiểm tra Title
            if (string.IsNullOrWhiteSpace(model.Title))
                return "Title is required.";
            if (model.Title.Length > 100)
                return "Title must be less than 100 characters.";

            // Kiểm tra LinkUrl
            if (string.IsNullOrWhiteSpace(model.LinkUrl))
                return "Link URL is required.";

            // Kiểm tra StartDate
            if (model.StartDate < DateTime.Now)
                return "Start date cannot be in the past.";

            // Kiểm tra EndDate
            if (model.EndDate < model.StartDate)
                return "End date must be after start date.";

            // Kiểm tra DisplayOrder
            if (model.DisplayOrder.HasValue && model.DisplayOrder < 0)
                return "Display order cannot be negative.";

            return string.Empty;
        }

        // Tạo mới banner
        public async Task<ResponseResult> CreateAsync(BannerCreateVModel model, string imageUrl, string imageUrlMobile)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = Validate(model);
                if (!string.IsNullOrEmpty(validationResult))
                    return new ErrorResponseResult(validationResult);

                // B2: Kiểm tra xem tiêu đề banner đã tồn tại chưa
                var existingBanner = await _context.Banners
                    .AnyAsync(b => b.Title.ToLower() == model.Title.ToLower());
                if (existingBanner)
                    return new ErrorResponseResult("Banner title already exists.");

                // B3: Chuyển ViewModel sang Entity
                var banner = model.ToEntity(imageUrl, imageUrlMobile);

                // B4: Thêm banner vào DbContext
                _context.Banners.Add(banner);
                await _context.SaveChangesAsync();

                // B5: Trả về kết quả thành công
                var bannerVM = banner.ToGetVModel();
                return new SuccessResponseResult(bannerVM, "Banner created successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("An error occurred while creating the banner: " + ex.Message);
            }
        }

        // Xóa banner
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm banner theo ID
                var banner = await _context.Banners
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (banner == null)
                    return new ErrorResponseResult("Banner not found.");

                // B2: Xóa banner
                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();

                // B3: Trả về kết quả thành công
                return new SuccessResponseResult("Banner deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("An error occurred while deleting the banner: " + ex.Message);
            }
        }

        // Lấy tất cả banner
        public async Task<List<BannerGetVModel>> GetAllAsync()
        {
            var banners = await _context.Banners
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();
            return banners.Select(b => b.ToGetVModel()).ToList();
        }

        // Lấy banner theo ID
        public async Task<BannerGetVModel?> GetByIdAsync(int id)
        {
            var banner = await _context.Banners
                .FirstOrDefaultAsync(b => b.Id == id);
            return banner?.ToGetVModel();
        }

        // Cập nhật banner
        public async Task<ResponseResult> UpdateAsync(BannerUpdateVModel model, string? imageUrl = null, string? imageUrlMobile = null)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = Validate(model, isUpdate: true, id: model.Id);
                if (!string.IsNullOrEmpty(validationResult))
                    return new ErrorResponseResult(validationResult);

                // B2: Tìm banner theo ID
                var banner = await _context.Banners
                    .FirstOrDefaultAsync(b => b.Id == model.Id);
                if (banner == null)
                    return new ErrorResponseResult($"Banner not found with ID: {model.Id}.");

                // B3: Kiểm tra xem tiêu đề banner đã tồn tại chưa (ngoại trừ banner hiện tại)
                var existingBanner = await _context.Banners
                    .AnyAsync(b => b.Title.ToLower() == model.Title.ToLower() && b.Id != model.Id);
                if (existingBanner)
                    return new ErrorResponseResult("Banner title already exists.");

                // B4: Cập nhật thông tin banner
                banner.UpdateEntity(model, imageUrl, imageUrlMobile);

                // B5: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B6: Trả về kết quả thành công
                var bannerVM = banner.ToGetVModel();
                return new SuccessResponseResult(bannerVM, "Banner updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("An error occurred while updating the banner: " + ex.Message);
            }
        }
    }
}