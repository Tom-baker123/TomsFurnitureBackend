using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class NewsService : INewsService
    {
        private readonly TomfurnitureContext _context; // Context để truy cập cơ sở dữ liệu

        public NewsService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            if (!string.IsNullOrWhiteSpace(model.Content) && model.Content.Length > 5000)
            {
                return "Nội dung tin tức không được dài quá 5000 ký tự.";
            }
            if (model.UserId.HasValue && model.UserId <= 0)
            {
                return "ID người dùng không hợp lệ.";
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
            if (!string.IsNullOrWhiteSpace(model.Content) && model.Content.Length > 5000)
            {
                return "Nội dung tin tức không được dài quá 5000 ký tự.";
            }
            if (model.UserId.HasValue && model.UserId <= 0)
            {
                return "ID người dùng không hợp lệ.";
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

        public async Task<ResponseResult> CreateAsync(NewsCreateVModel model, string? newsAvatar, string createdBy)
        {
            try
            {
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                var existingNews = await _context.News
                    .AnyAsync(n => n.Title.ToLower() == model.Title.ToLower());
                if (existingNews)
                {
                    return new ErrorResponseResult("Tiêu đề tin tức đã tồn tại.");
                }

                if (model.UserId.HasValue)
                {
                    var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId);
                    if (!userExists)
                    {
                        return new ErrorResponseResult("Người dùng không tồn tại.");
                    }
                }

                var news = model.ToEntity(newsAvatar, createdBy);
                _context.News.Add(news);
                await _context.SaveChangesAsync();

                var newsVM = news.ToGetVModel();
                return new SuccessResponseResult(newsVM, "Tạo tin tức thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi tạo tin tức: {ex.Message}");
            }
        }

        public async Task<ResponseResult> UpdateAsync(NewsUpdateVModel model, string? newsAvatar, string updatedBy)
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

                // B3: Kiểm tra tiêu đề đã tồn tại (ngoại trừ tin tức hiện tại)
                var existingNews = await _context.News
                    .AnyAsync(n => n.Title.ToLower() == model.Title.ToLower() && n.Id != model.Id);
                if (existingNews)
                {
                    return new ErrorResponseResult("Tiêu đề tin tức đã tồn tại.");
                }

                // B4: Kiểm tra UserId (nếu có)
                if (model.UserId.HasValue)
                {
                    var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId);
                    if (!userExists)
                    {
                        return new ErrorResponseResult("Người dùng không tồn tại.");
                    }
                }

                // B5: Cập nhật entity
                news.UpdateEntity(model, newsAvatar, updatedBy);

                // B6: Lưu thay đổi
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    // Ghi log chi tiết lỗi từ cơ sở dữ liệu
                    var errorMessage = dbEx.InnerException != null
                        ? dbEx.InnerException.Message
                        : dbEx.Message;
                    return new ErrorResponseResult($"Lỗi khi lưu thay đổi tin tức: {errorMessage}");
                }

                // B7: Trả về kết quả thành công
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
                var news = await _context.News
                    .FirstOrDefaultAsync(n => n.Id == id);
                if (news == null)
                {
                    return new ErrorResponseResult("Không tìm thấy tin tức.");
                }

                _context.News.Remove(news);
                await _context.SaveChangesAsync();

                return new SuccessResponseResult(null, "Xóa tin tức thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi xóa tin tức: {ex.Message}");
            }
        }
    }
}