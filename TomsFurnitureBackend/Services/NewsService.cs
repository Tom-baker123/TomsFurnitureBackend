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
        private readonly ILogger<NewsService> _logger; // Logger để ghi log

        public NewsService(TomfurnitureContext context, ILogger<NewsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogInformation("Bắt đầu lấy danh sách tất cả tin tức.");
                var news = await _context.News
                    .Include(n => n.User)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToListAsync();
                var result = news.Select(n => n.ToGetVModel()).ToList();
                _logger.LogInformation("Lấy danh sách {Count} tin tức thành công.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy danh sách tin tức: {Error}", ex.Message);
                throw new Exception($"Lỗi khi lấy danh sách tin tức: {ex.Message}");
            }
        }

        public async Task<NewsGetVModel?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy tin tức với ID: {NewsId}", id);
                var news = await _context.News
                    .Include(n => n.User)
                    .FirstOrDefaultAsync(n => n.Id == id);
                if (news == null)
                {
                    _logger.LogWarning("Không tìm thấy tin tức với ID: {NewsId}", id);
                    return null;
                }
                _logger.LogInformation("Lấy tin tức {NewsId} thành công.", id);
                return news.ToGetVModel();
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy tin tức {NewsId}: {Error}", id, ex.Message);
                throw new Exception($"Lỗi khi lấy tin tức: {ex.Message}");
            }
        }

        public async Task<ResponseResult> CreateAsync(NewsCreateVModel model, string? newsAvatar, string createdBy)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo tin tức mới với tiêu đề: {Title}", model.Title);
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validate tạo tin tức thất bại: {Error}", validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                var existingNews = await _context.News
                    .AnyAsync(n => n.Title.ToLower() == model.Title.ToLower());
                if (existingNews)
                {
                    _logger.LogWarning("Tiêu đề tin tức đã tồn tại: {Title}", model.Title);
                    return new ErrorResponseResult("Tiêu đề tin tức đã tồn tại.");
                }

                if (model.UserId.HasValue)
                {
                    var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId);
                    if (!userExists)
                    {
                        _logger.LogWarning("Không tìm thấy người dùng với ID: {UserId}", model.UserId);
                        return new ErrorResponseResult("Người dùng không tồn tại.");
                    }
                }

                var news = model.ToEntity(newsAvatar, createdBy);
                _context.News.Add(news);
                await _context.SaveChangesAsync();

                var newsVM = news.ToGetVModel();
                _logger.LogInformation("Tạo tin tức {NewsId} thành công.", news.Id);
                return new SuccessResponseResult(newsVM, "Tạo tin tức thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo tin tức: {Error}, InnerException: {InnerError}", ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi tạo tin tức: {ex.Message}");
            }
        }

        public async Task<ResponseResult> UpdateAsync(NewsUpdateVModel model, string? newsAvatar, string updatedBy)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật tin tức với ID: {NewsId}", model.Id);
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validate cập nhật tin tức thất bại: {Error}", validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm tin tức theo ID
                var news = await _context.News
                    .FirstOrDefaultAsync(n => n.Id == model.Id);
                if (news == null)
                {
                    _logger.LogWarning("Không tìm thấy tin tức với ID: {NewsId}", model.Id);
                    return new ErrorResponseResult("Không tìm thấy tin tức.");
                }

                // B3: Kiểm tra tiêu đề đã tồn tại (ngoại trừ tin tức hiện tại)
                var existingNews = await _context.News
                    .AnyAsync(n => n.Title.ToLower() == model.Title.ToLower() && n.Id != model.Id);
                if (existingNews)
                {
                    _logger.LogWarning("Tiêu đề tin tức đã tồn tại: {Title}", model.Title);
                    return new ErrorResponseResult("Tiêu đề tin tức đã tồn tại.");
                }

                // B4: Kiểm tra UserId (nếu có)
                if (model.UserId.HasValue)
                {
                    var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId);
                    if (!userExists)
                    {
                        _logger.LogWarning("Không tìm thấy người dùng với ID: {UserId}", model.UserId);
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
                    _logger.LogError("Lỗi khi lưu thay đổi tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                        model.Id, dbEx.Message, errorMessage);
                    return new ErrorResponseResult($"Lỗi khi lưu thay đổi tin tức: {errorMessage}");
                }

                // B7: Trả về kết quả thành công
                var newsVM = news.ToGetVModel();
                _logger.LogInformation("Cập nhật tin tức {NewsId} thành công.", model.Id);
                return new SuccessResponseResult(newsVM, "Cập nhật tin tức thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                    model.Id, ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi cập nhật tin tức: {ex.Message}");
            }
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa tin tức với ID: {NewsId}", id);
                var news = await _context.News
                    .FirstOrDefaultAsync(n => n.Id == id);
                if (news == null)
                {
                    _logger.LogWarning("Không tìm thấy tin tức với ID: {NewsId}", id);
                    return new ErrorResponseResult("Không tìm thấy tin tức.");
                }

                _context.News.Remove(news);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Xóa tin tức {NewsId} thành công.", id);
                return new SuccessResponseResult(null, "Xóa tin tức thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return new ErrorResponseResult($"Lỗi khi xóa tin tức: {ex.Message}");
            }
        }
    }
}