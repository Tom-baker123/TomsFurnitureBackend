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
    public class StoreInformationController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly IStoreInformationService _storeInformationService;
        private readonly ILogger<StoreInformationController> _logger;
        private readonly Cloudinary _cloudinary;

        // Constructor nhận các dependency qua DI
        public StoreInformationController(IStoreInformationService storeInformationService, ILogger<StoreInformationController> logger, Cloudinary cloudinary, TomfurnitureContext context)
        {
            _context = context;
            _storeInformationService = storeInformationService;
            _logger = logger;
            _cloudinary = cloudinary;
        }

        // [1.] Lấy danh sách tất cả thông tin cửa hàng
        [HttpGet]
        public async Task<List<StoreInformationGetVModel>> GetAllStoreInformation()
        {
            // Gọi service để lấy danh sách tất cả thông tin cửa hàng
            return await _storeInformationService.GetAllAsync();
        }

        // [2.] Lấy thông tin cửa hàng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdStoreInformation(int id)
        {
            // Gọi service để lấy thông tin cửa hàng theo ID
            var storeInformation = await _storeInformationService.GetByIdAsync(id);
            if (storeInformation == null)
            {
                return NotFound(new { Message = "Store information not found." });
            }
            return Ok(storeInformation);
        }

        // [3.] Tạo thông tin cửa hàng mới
        [HttpPost]
        public async Task<IActionResult> CreateStoreInformation([FromForm] StoreInformationCreateVModel storeInformationVModel, IFormFile? logoFile)
        {
            try
            {
                // B1: Xử lý upload logo nếu có
                string? logoUrl = null;
                if (logoFile != null && logoFile.Length > 0)
                {
                    // Kiểm tra định dạng file
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(logoFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest("File format not supported!");
                    }

                    // Cấu hình tham số upload
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(logoFile.FileName, logoFile.OpenReadStream())
                    };

                    // Upload logo lên Cloudinary
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        return BadRequest($"Cloudinary upload error: {uploadResult.Error.Message}");
                    }

                    logoUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // B2: Gọi service để tạo thông tin cửa hàng
                var result = await _storeInformationService.CreateAsync(storeInformationVModel, logoUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B3: Trả về phản hồi thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetByIdStoreInformation),
                    new { id = ((StoreInformationGetVModel)successResult.Data).Id },
                    new
                    {
                        Message = successResult.Message,
                        StoreInformationId = ((StoreInformationGetVModel)successResult.Data).Id
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating store information: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the store information.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật thông tin cửa hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStoreInformation(int id, [FromForm] StoreInformationUpdateVModel storeInformationVModel, IFormFile? logoFile = null)
        {
            try
            {
                // Kiểm tra ID hợp lệ
                if (id != storeInformationVModel.Id)
                {
                    return BadRequest("ID in URL does not match ID in model.");
                }

                // B1: Xử lý upload logo nếu có
                string? logoUrl = null;
                if (logoFile != null && logoFile.Length > 0)
                {
                    // Kiểm tra định dạng file
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(logoFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest("File format not supported!");
                    }

                    // Tìm thông tin cửa hàng để lấy URL logo cũ
                    var existingStoreInformation = await _context.StoreInformations
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Id == storeInformationVModel.Id);
                    if (existingStoreInformation == null)
                    {
                        return NotFound($"Store information not found with ID: {storeInformationVModel.Id}");
                    }

                    // Xóa logo cũ trên Cloudinary nếu tồn tại
                    if (!string.IsNullOrEmpty(existingStoreInformation.Logo))
                    {
                        try
                        {
                            // Trích xuất PublicId từ Logo
                            var uri = new Uri(existingStoreInformation.Logo);
                            var publicId = uri.AbsolutePath
                                .Split(new[] { "/image/upload/" }, StringSplitOptions.None)[1]
                                .Replace(Path.GetExtension(uri.AbsolutePath), "")
                                .TrimEnd('/');

                            // Xóa logo trên Cloudinary
                            var deletionParams = new DeletionParams(publicId)
                            {
                                ResourceType = ResourceType.Image
                            };
                            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                            if (deletionResult.Error != null)
                            {
                                _logger.LogWarning("Failed to delete old logo on Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                            }
                            else
                            {
                                _logger.LogInformation("Deleted old logo on Cloudinary with PublicId: {PublicId}", publicId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning("Error deleting old logo on Cloudinary: {Error}", ex.Message);
                        }
                    }

                    // Upload logo mới lên Cloudinary
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(logoFile.FileName, logoFile.OpenReadStream())
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        return BadRequest($"Cloudinary upload error: {uploadResult.Error.Message}");
                    }

                    logoUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // B2: Gọi service để cập nhật thông tin cửa hàng
                var result = await _storeInformationService.UpdateAsync(storeInformationVModel, logoUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating store information: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the store information.", Error = ex.Message });
            }
        }

        // [5.] Xóa thông tin cửa hàng theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoreInformation(int id)
        {
            try
            {
                // B1: Tìm thông tin cửa hàng để lấy Logo
                var storeInformation = await _context.StoreInformations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (storeInformation == null)
                {
                    return NotFound($"Store information not found with ID: {id}");
                }

                // B2: Xóa logo trên Cloudinary nếu tồn tại
                if (!string.IsNullOrEmpty(storeInformation.Logo))
                {
                    try
                    {
                        // Trích xuất PublicId từ Logo
                        var uri = new Uri(storeInformation.Logo);
                        var publicId = uri.AbsolutePath
                            .Split(new[] { "/image/upload/" }, StringSplitOptions.None)[1]
                            .Replace(Path.GetExtension(uri.AbsolutePath), "")
                            .TrimEnd('/');

                        // Xóa logo trên Cloudinary
                        var deletionParams = new DeletionParams(publicId)
                        {
                            ResourceType = ResourceType.Image
                        };
                        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                        if (deletionResult.Error != null)
                        {
                            _logger.LogWarning("Failed to delete logo on Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                        }
                        else
                        {
                            _logger.LogInformation("Deleted logo on Cloudinary with PublicId: {PublicId}", publicId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error deleting logo on Cloudinary: {Error}", ex.Message);
                    }
                }

                // B3: Gọi service để xóa thông tin cửa hàng
                var result = await _storeInformationService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B4: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting store information: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the store information.", Error = ex.Message });
            }
        }
    }
}
