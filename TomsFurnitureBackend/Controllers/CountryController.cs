using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;
        private readonly Cloudinary _cloudinary;

        // Constructor nhận các dependency qua DI
        public CountryController(ICountryService countryService, ILogger<CountryController> logger, Cloudinary cloudinary, TomfurnitureContext context)
        {
            _context = context;
            _countryService = countryService;
            _logger = logger;
            _cloudinary = cloudinary;
        }

        // [1.] Lấy danh sách tất cả xuất xứ
        [HttpGet]
        public async Task<List<CountryGetVModel>> GetAllCountry()
        {
            // Gọi service để lấy danh sách tất cả xuất xứ
            return await _countryService.GetAllAsync();
        }

        // [2.] Lấy xuất xứ theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdCountry(int id)
        {
            // Gọi service để lấy xuất xứ theo ID
            var country = await _countryService.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound(new { Message = "Country not found." });
            }
            return Ok(country);
        }

        // [3.] Tạo xuất xứ mới
        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromForm] CountryCreateVModel countryVModel, IFormFile? imageFile)
        {
            try
            {
                // B1: Xử lý upload hình ảnh nếu có
                string? imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Kiểm tra định dạng file
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest("File format not supported!");
                    }

                    // Cấu hình tham số upload
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                    };

                    // Upload ảnh lên Cloudinary
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        return BadRequest($"Cloudinary upload error: {uploadResult.Error.Message}");
                    }

                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // B2: Gọi service để tạo xuất xứ
                var result = await _countryService.CreateAsync(countryVModel, imageUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B3: Trả về phản hồi thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetByIdCountry),
                    new { id = ((CountryGetVModel)successResult.Data).Id },
                    new
                    {
                        Message = successResult.Message,
                        CountryId = ((CountryGetVModel)successResult.Data).Id
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating country: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the country.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật xuất xứ
        [HttpPut]
        public async Task<IActionResult> UpdateCountry([FromForm] CountryUpdateVModel countryVModel, IFormFile? imageFile = null)
        {
            try
            {
                // B1: Xử lý upload hình ảnh nếu có
                string? imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Kiểm tra định dạng file
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest("File format not supported!");
                    }

                    // Tìm xuất xứ để lấy URL ảnh cũ
                    var existingCountry = await _context.Countries
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == countryVModel.Id);
                    if (existingCountry == null)
                    {
                        return NotFound($"Country not found with ID: {countryVModel.Id}");
                    }

                    // Xóa ảnh cũ trên Cloudinary nếu tồn tại
                    if (!string.IsNullOrEmpty(existingCountry.ImageUrl))
                    {
                        try
                        {
                            // Trích xuất PublicId từ ImageUrl
                            var uri = new Uri(existingCountry.ImageUrl);
                            var publicId = uri.AbsolutePath
                                .Split(new[] { "/image/upload/" }, StringSplitOptions.None)[1]
                                .Replace(Path.GetExtension(uri.AbsolutePath), "")
                                .TrimEnd('/');

                            // Xóa ảnh trên Cloudinary
                            var deletionParams = new DeletionParams(publicId)
                            {
                                ResourceType = ResourceType.Image
                            };
                            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                            if (deletionResult.Error != null)
                            {
                                _logger.LogWarning("Failed to delete old image on Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                            }
                            else
                            {
                                _logger.LogInformation("Deleted old image on Cloudinary with PublicId: {PublicId}", publicId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning("Error deleting old image on Cloudinary: {Error}", ex.Message);
                        }
                    }

                    // Upload ảnh mới lên Cloudinary
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        return BadRequest($"Cloudinary upload error: {uploadResult.Error.Message}");
                    }

                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // B2: Gọi service để cập nhật xuất xứ
                var result = await _countryService.UpdateAsync(countryVModel, imageUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating country: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the country.", Error = ex.Message });
            }
        }

        // [5.] Xóa xuất xứ theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            try
            {
                // B1: Tìm xuất xứ để lấy ImageUrl
                var country = await _context.Countries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (country == null)
                {
                    return NotFound($"Country not found with ID: {id}");
                }

                // B2: Xóa ảnh trên Cloudinary nếu tồn tại
                if (!string.IsNullOrEmpty(country.ImageUrl))
                {
                    try
                    {
                        // Trích xuất PublicId từ ImageUrl
                        var uri = new Uri(country.ImageUrl);
                        var publicId = uri.AbsolutePath
                            .Split(new[] { "/image/upload/" }, StringSplitOptions.None)[1]
                            .Replace(Path.GetExtension(uri.AbsolutePath), "")
                            .TrimEnd('/');

                        // Xóa ảnh trên Cloudinary
                        var deletionParams = new DeletionParams(publicId)
                        {
                            ResourceType = ResourceType.Image
                        };
                        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                        if (deletionResult.Error != null)
                        {
                            _logger.LogWarning("Failed to delete image on Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                        }
                        else
                        {
                            _logger.LogInformation("Deleted image on Cloudinary with PublicId: {PublicId}", publicId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error deleting image on Cloudinary: {Error}", ex.Message);
                    }
                }

                // B3: Gọi service để xóa xuất xứ
                var result = await _countryService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B4: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting country: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the country.", Error = ex.Message });
            }
        }
    }
}