using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Helpers;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class NewsService : INewsService
    {
        private readonly TomfurnitureContext _context;
        private readonly Cloudinary _cloudinary;
        private readonly IAuthService _authService;

        public NewsService(
            TomfurnitureContext context,
            Cloudinary cloudinary,
            IAuthService authService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        // Validation cho tạo tin tức
        private static string ValidateCreate(NewsCreateVModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return "Tiêu đề tin tức là bắt buộc.";
            }
            if (model.Title.Length > 200)
            {
                return "Tiêu đề tin tức không được dài quá 200 ký tự.";
            }
            return string.Empty;
        }

        // Validation cho cập nhật tin tức
        private static string ValidateUpdate(NewsUpdateVModel model)
        {
            if (model.Id <= 0)
            {
                return "ID tin tức không hợp lệ.";
            }
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return "Tiêu đề tin tức là bắt buộc.";
            }
            if (model.Title.Length > 200)
            {
                return "Tiêu đề tin tức không được dài quá 200 ký tự.";
            }
            return string.Empty;
        }

        // Kiểm tra định dạng file ảnh
        private static string ValidateImageFile(IFormFile imageFile)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return "Định dạng file không được hỗ trợ.";
            }
            return string.Empty;
        }

        public async Task<List<NewsGetVModel>> GetAllAsync()
        {
            try
            {
                var news = await _context.News
                    .Include(n => n.User)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToListAsync();
                var result = news.Select(n => n.ToGetVModel()).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách tin tức: {ex.Message}");
            }
        }

        public async Task<NewsGetVModel?> GetByIdAsync(int id)
        {
            try
            {
                var news = await _context.News
                    .Include(n => n.User)
                    .FirstOrDefaultAsync(n => n.Id == id);
                if (news == null)
                {
                    return null;
                }
                return news.ToGetVModel();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy tin tức: {ex.Message}");
            }
        }

        public async Task<ResponseResult> CreateAsync(NewsCreateVModel model, IFormFile? imageFile, HttpContext httpContext)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra tiêu đề đã tồn tại
                var existingNews = await _context.News
                    .AnyAsync(n => n.Title.ToLower() == model.Title.ToLower());
                if (existingNews)
                {
                    return new ErrorResponseResult("Tiêu đề tin tức đã tồn tại.");
                }

                // B3: Lấy thông tin người dùng từ AuthService
                var authStatus = await _authService.GetAuthStatusAsync(httpContext.User, httpContext);
                if (!authStatus.IsAuthenticated || authStatus.UserId == null)
                {
                    return new ErrorResponseResult("Người dùng chưa đăng nhập hoặc phiên không hợp lệ.");
                }

                // B4: Kiểm tra người dùng tồn tại
                var userExists = await _context.Users.AnyAsync(u => u.Id == authStatus.UserId);
                if (!userExists)
                {
                    return new ErrorResponseResult("Người dùng không tồn tại.");
                }

                // B5: Tạo slug duy nhất
                var slug = await SlugHelper.GenerateUniqueSlugAsync(model.Title, async (s) =>
                    await _context.News.AnyAsync(n => n.Slug == s));

                // B6: Xử lý upload ảnh nếu có
                string? newsAvatar = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageValidation = ValidateImageFile(imageFile);
                    if (!string.IsNullOrEmpty(imageValidation))
                    {
                        return new ErrorResponseResult(imageValidation);
                    }

                    newsAvatar = await CloudinaryHelper.HandleSliderImageUpload(_cloudinary, imageFile, null);
                    if (newsAvatar == null)
                    {
                        return new ErrorResponseResult("Lỗi khi upload ảnh lên Cloudinary.");
                    }
                }

                // B7: Tạo entity và lưu vào database
                var createdBy = authStatus.UserName ?? "System";
                var news = model.ToEntity(newsAvatar, createdBy, authStatus.UserId);
                news.Slug = slug; // Gán slug
                _context.News.Add(news);
                await _context.SaveChangesAsync();

                // B8: Trả về kết quả
                var newsVM = news.ToGetVModel();
                return new SuccessResponseResult(newsVM, "Tạo tin tức thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi tạo tin tức: {ex.Message}");
            }
        }

        public async Task<ResponseResult> UpdateAsync(NewsUpdateVModel model, IFormFile? imageFile, HttpContext httpContext)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm tin tức theo ID
                var news = await _context.News
                    .FirstOrDefaultAsync(n => n.Id == model.Id);
                if (news == null)
                {
                    return new ErrorResponseResult("Không tìm thấy tin tức.");
                }

                // B3: Kiểm tra tiêu đề đã tồn tại
                var existingNews = await _context.News
                    .AnyAsync(n => n.Title.ToLower() == model.Title.ToLower() && n.Id != model.Id);
                if (existingNews)
                {
                    return new ErrorResponseResult("Tiêu đề tin tức đã tồn tại.");
                }

                // B4: Lấy thông tin người dùng từ AuthService
                var authStatus = await _authService.GetAuthStatusAsync(httpContext.User, httpContext);
                if (!authStatus.IsAuthenticated || authStatus.UserId == null)
                {
                    return new ErrorResponseResult("Người dùng chưa đăng nhập hoặc phiên không hợp lệ.");
                }

                // B5: Kiểm tra người dùng tồn tại
                var userExists = await _context.Users.AnyAsync(u => u.Id == authStatus.UserId);
                if (!userExists)
                {
                    return new ErrorResponseResult("Người dùng không tồn tại.");
                }

                // B6: Tạo slug duy nhất (nếu tiêu đề thay đổi)
                var newSlug = await SlugHelper.GenerateUniqueSlugAsync(model.Title, async (s) =>
                    await _context.News.AnyAsync(n => n.Slug == s && n.Id != model.Id));

                // B7: Xử lý ảnh nếu có
                string? newsAvatar = news.NewsAvatar;
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageValidation = ValidateImageFile(imageFile);
                    if (!string.IsNullOrEmpty(imageValidation))
                    {
                        return new ErrorResponseResult(imageValidation);
                    }

                    // Xóa ảnh cũ trên Cloudinary nếu có
                    if (!string.IsNullOrEmpty(news.NewsAvatar))
                    {
                        try
                        {
                            var publicId = CloudinaryHelper.ExtractPublicIdFromUrl(news.NewsAvatar);
                            if (!string.IsNullOrEmpty(publicId))
                            {
                                var deletionParams = new DeletionParams(publicId)
                                {
                                    ResourceType = ResourceType.Image
                                };
                                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                                // Ignore deletionResult.Error
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore deletion error
                        }
                    }

                    // Upload ảnh mới
                    newsAvatar = await CloudinaryHelper.HandleSliderImageUpload(_cloudinary, imageFile, null);
                    if (newsAvatar == null)
                    {
                        return new ErrorResponseResult("Lỗi khi upload ảnh lên Cloudinary.");
                    }
                }

                // B8: Cập nhật entity
                var updatedBy = authStatus.UserName ?? "System";
                news.UpdateEntity(model, newsAvatar, updatedBy, authStatus.UserId);
                news.Slug = newSlug; // Cập nhật slug nếu có thay đổi

                // B9: Lưu thay đổi
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    var errorMessage = dbEx.InnerException != null
                        ? dbEx.InnerException.Message
                        : dbEx.Message;
                    return new ErrorResponseResult($"Lỗi khi lưu thay đổi tin tức: {errorMessage}");
                }

                // B10: Trả về kết quả
                var newsVM = news.ToGetVModel();
                return new SuccessResponseResult(newsVM, "Cập nhật tin tức thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi cập nhật tin tức: {ex.Message}");
            }
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm tin tức theo ID
                var news = await _context.News
                    .FirstOrDefaultAsync(n => n.Id == id);
                if (news == null)
                {
                    return new ErrorResponseResult("Không tìm thấy tin tức.");
                }

                // B2: Xóa ảnh trên Cloudinary nếu có
                if (!string.IsNullOrEmpty(news.NewsAvatar))
                {
                    try
                    {
                        var publicId = CloudinaryHelper.ExtractPublicIdFromUrl(news.NewsAvatar);
                        if (!string.IsNullOrEmpty(publicId))
                        {
                            var deletionParams = new DeletionParams(publicId)
                            {
                                ResourceType = ResourceType.Image
                            };
                            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                            // Ignore deletionResult.Error
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore deletion error
                    }
                }

                // B3: Xóa tin tức
                _context.News.Remove(news);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả
                return new SuccessResponseResult(null, "Xóa tin tức thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi xóa tin tức: {ex.Message}");
            }
        }
    }
}