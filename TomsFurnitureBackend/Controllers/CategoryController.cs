using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly Cloudinary _cloudinary;
        private readonly TomfurnitureContext _context;


        // Constructor injection for services
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, Cloudinary cloudinary, TomfurnitureContext context) { 
            _categoryService = categoryService;
            _logger = logger;
            _cloudinary = cloudinary;
            _context = context;
        }

        // [1.] Lấy tất cả Danh mục  
        [HttpGet]
        public async Task<List<CategoryGetVModel>> GetAllAsync()
        {
            // Gọi service để lấy danh sách tất cả danh mục
            return await _categoryService.GetAllAsync();
        }

        // [2.] Lấy Danh mục theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Gọi service để lấy danh mục theo ID
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound($"- Not found category with ID: {id}");
            }
            return Ok(result);
        }

        // [3.] Tạo mới Danh mục
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateVModel categoryVModel, IFormFile imageFile)
        {
            try
            {
                // B1: Xử lý upload ảnh lên Cloudinary
                string? imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Kiểm tra định dạng file
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower(); // Lấy ra đuôi file và chuyển về chữ thường
                    // Kiểm tra xem đuôi file có nằm trong danh sách cho phép không
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest("Format is not supported");
                    }

                    // Cấu hình upload lên Cloudinary
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                        Transformation = new Transformation().Width(800).Height(800).Crop("fill")
                    };

                    // Upload ảnh lên Cloudinary + tham số
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        return BadRequest($"Error uploading image: {uploadResult.Error.Message}");
                    }

                    imageUrl = uploadResult.SecureUrl.AbsoluteUri; // Lấy URL của ảnh đã upload
                }

                // B2: Gọi service để tạo mới danh mục
                var result = await _categoryService.CreateAsync(categoryVModel, imageUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B3: Trả về kết quả thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = successResult.Id },
                    new { Message = successResult.Message, Data = successResult.Data.Id }
                );

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"You have an error: {ex.Message}" });
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateVModel categoryVModel, IFormFile? imageFile = null) {
            try
            {
                // B1: Kiểm tra Id hợp lệ không?
                if (id != categoryVModel.Id) {
                    return BadRequest("Id not match in vmodel");
                }

                // B2: Tìm danh mục để lấy URL ảnh cũ
                var category = await _context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == categoryVModel.Id);
                if (category == null) {
                    return NotFound($"Not found ID {id} in category");
                }

                // B3: Xử lý upload ảnh nếu có
                string? imageUrl = null;
                if(imageFile != null && imageFile.Length > 0) {
                    // Kiểm tra định dạng file
                    string[]  allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension)) { 
                        return BadRequest("Format file not support!");
                    }

                    if (!string.IsNullOrEmpty(category.ImageUrl))
                    {
                        try
                        {
                            // Trích xuất PublicId từ ImageUrl
                            var uri = new Uri(category.ImageUrl);
                            var publicId = Path.GetFileNameWithoutExtension(uri.AbsolutePath);

                            // Xóa ảnh trên cloudinary
                            var deletionParams = new DeletionParams(publicId)
                            {
                                ResourceType = ResourceType.Image
                            };


                            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                            if (deletionResult.Error != null)
                            {
                                _logger.LogWarning("Không thể xóa ảnh cũ trên Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning("Lỗi khi xóa ảnh cũ trên Cloudinary: {Error}", ex.Message);
                            // Tiếp tục xử lý ngay cả khi xóa ảnh cũ thất bại
                        }
                    }

                    // Upload ảnh mới lên Cloudinary
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null) {
                        return BadRequest($"Upload Cloudinary Error: {uploadResult.Error.Message}");
                    }

                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // B4: Gọi service để cập nhật danh mục
                var result = await _categoryService.UpdateAsync(categoryVModel, imageUrl);
                if (!result.IsSuccess) {
                    return BadRequest(result.Message);
                }

                // B5: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { Message = $"Error when updating category: {ex.Message}" });
            }
        }

        // [5.] Xóa danh mục theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id) {
            try
            {
                // Gọi service để xóa danh mục
                var result = await _categoryService.DeleteAsync(id);
                if (!result.IsSuccess) {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { Message = $"Error when Delete category: {ex.Message}" });
            }
        }
    }
}
