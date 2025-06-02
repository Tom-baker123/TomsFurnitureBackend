using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsController> _logger;
        private readonly Cloudinary _cloudinary;
        private readonly TomfurnitureContext _context;

        public NewsController(INewsService newsService, ILogger<NewsController> logger, Cloudinary cloudinary, TomfurnitureContext context)
        {
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllNews()
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy danh sách tất cả tin tức.");
                var news = await _newsService.GetAllAsync();
                _logger.LogInformation("Lấy danh sách {Count} tin tức thành công.", news.Count);
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy danh sách tin tức: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy danh sách tin tức.", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsById(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy tin tức với ID: {NewsId}", id);
                var news = await _newsService.GetByIdAsync(id);
                if (news == null)
                {
                    _logger.LogWarning("Không tìm thấy tin tức với ID: {NewsId}", id);
                    return NotFound(new { Message = "Không tìm thấy tin tức." });
                }
                _logger.LogInformation("Lấy tin tức {NewsId} thành công.", id);
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy tin tức.", Error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNews([FromForm] NewsCreateVModel model, IFormFile? imageFile)
        {
            try
            {
                _logger.LogInformation("Yêu cầu tạo tin tức mới với tiêu đề: {Title}", model.Title);
                string? newsAvatar = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        _logger.LogWarning("Định dạng file không được hỗ trợ: {FileName}", imageFile.FileName);
                        return BadRequest(new { Message = "Định dạng file không được hỗ trợ." });
                    }

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Lỗi khi upload ảnh lên Cloudinary: {Error}", uploadResult.Error.Message);
                        return BadRequest(new { Message = $"Lỗi khi upload ảnh: {uploadResult.Error.Message}" });
                    }
                    newsAvatar = uploadResult.SecureUrl.AbsoluteUri;
                }

                var createdBy = User.Identity?.Name ?? "System";
                var result = await _newsService.CreateAsync(model, newsAvatar, createdBy);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Tạo tin tức thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Tạo tin tức thành công với ID: {NewsId}", ((NewsGetVModel)successResult.Data).Id);
                return CreatedAtAction(nameof(GetNewsById), new { id = ((NewsGetVModel)successResult.Data).Id }, successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo tin tức: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi tạo tin tức.", Error = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateNews([FromForm] NewsUpdateVModel model, IFormFile? imageFile)
        {
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật tin tức với ID: {NewsId}", model.Id);
                // B1: Xử lý upload ảnh nếu có
                string? newsAvatar = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        _logger.LogWarning("Định dạng file không được hỗ trợ: {FileName}", imageFile.FileName);
                        return BadRequest(new { Message = "Định dạng file không được hỗ trợ." });
                    }

                    // Lấy thông tin tin tức hiện tại để xóa ảnh cũ
                    var existingNews = await _context.News
                        .AsNoTracking()
                        .FirstOrDefaultAsync(n => n.Id == model.Id);
                    if (existingNews == null)
                    {
                        _logger.LogWarning("Không tìm thấy tin tức với ID: {NewsId}", model.Id);
                        return NotFound(new { Message = "Không tìm thấy tin tức." });
                    }

                    // Xóa ảnh cũ trên Cloudinary nếu có
                    if (!string.IsNullOrEmpty(existingNews.NewsAvatar))
                    {
                        try
                        {
                            var publicId = ExtractPublicIdFromUrl(existingNews.NewsAvatar);
                            if (!string.IsNullOrEmpty(publicId))
                            {
                                var deletionParams = new DeletionParams(publicId)
                                {
                                    ResourceType = ResourceType.Image // Sửa: Sử dụng ResourceType.Image
                                };
                                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                                if (deletionResult.Error != null)
                                {
                                    _logger.LogWarning("Không thể xóa ảnh cũ trên Cloudinary: {Error}", deletionResult.Error.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning("Lỗi khi xóa ảnh cũ trên Cloudinary: {Error}", ex.Message);
                        }
                    }

                    // Upload ảnh mới
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Lỗi khi upload ảnh lên Cloudinary: {Error}", uploadResult.Error.Message);
                        return BadRequest(new { Message = $"Lỗi khi upload ảnh: {uploadResult.Error.Message}" });
                    }
                    newsAvatar = uploadResult.SecureUrl.AbsoluteUri; // Sửa: Chuyển Uri thành string
                }

                // B2: Gọi service để cập nhật tin tức
                var updatedBy = User.Identity?.Name ?? "System";
                var result = await _newsService.UpdateAsync(model, newsAvatar, updatedBy);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Cập nhật tin tức thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                // B3: Trả về phản hồi
                _logger.LogInformation("Cập nhật tin tức {NewsId} thành công.", model.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                    model.Id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi cập nhật tin tức.", Error = ex.Message, InnerException = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa tin tức với ID: {NewsId}", id);
                var news = await _context.News
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id);
                if (news == null)
                {
                    _logger.LogWarning("Không tìm thấy tin tức với ID: {NewsId}", id);
                    return NotFound(new { Message = "Không tìm thấy tin tức." });
                }

                if (!string.IsNullOrEmpty(news.NewsAvatar))
                {
                    try
                    {
                        var publicId = ExtractPublicIdFromUrl(news.NewsAvatar);
                        if (!string.IsNullOrEmpty(publicId))
                        {
                            var deletionParams = new DeletionParams(publicId)
                            {
                                ResourceType = ResourceType.Image // Sửa: Sử dụng ResourceType.Image
                            };
                            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                            if (deletionResult.Error != null)
                            {
                                _logger.LogWarning("Không thể xóa ảnh trên Cloudinary: {Error}", deletionResult.Error.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Lỗi khi xóa ảnh trên Cloudinary: {Error}", ex.Message);
                    }
                }

                var result = await _newsService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Xóa tin tức thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Xóa tin tức {NewsId} thành công.", id);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi xóa tin tức.", Error = ex.Message });
            }
        }

        private string? ExtractPublicIdFromUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return null;

                var uri = new Uri(url);
                var path = uri.AbsolutePath;
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length >= 2)
                {
                    var lastSegment = segments[^1];
                    return Path.GetFileNameWithoutExtension(lastSegment);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Lỗi khi trích xuất public ID từ URL {Url}: {Error}", url, ex.Message);
                return null;
            }
        }
    }
}