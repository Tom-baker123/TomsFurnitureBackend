using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SupplierController> _logger;
        private readonly Cloudinary _cloudinary;

        // Constructor nhận các dependency qua DI
        public SupplierController(ISupplierService supplierService, ILogger<SupplierController> logger, Cloudinary cloudinary, TomfurnitureContext context)
        {
            _context = context;
            _supplierService = supplierService;
            _logger = logger;
            _cloudinary = cloudinary;
        }

        // [1.] Lấy danh sách tất cả nhà cung cấp
        [HttpGet]
        public async Task<List<SupplierGetVModel>> GetAllSupplier()
        {
            // Gọi service để lấy danh sách tất cả nhà cung cấp
            return await _supplierService.GetAllAsync();
        }

        // [2.] Lấy nhà cung cấp theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdSupplier(int id)
        {
            // Gọi service để lấy nhà cung cấp theo ID
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound(new { Message = "Supplier not found." });
            }
            return Ok(supplier);
        }

        // [3.] Tạo nhà cung cấp mới
        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromForm] SupplierCreateVModel supplierVModel, IFormFile? imageFile)
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

                // B2: Gọi service để tạo nhà cung cấp
                var result = await _supplierService.CreateAsync(supplierVModel, imageUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B3: Trả về phản hồi thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetByIdSupplier),
                    new { id = ((SupplierGetVModel)successResult.Data).Id },
                    new
                    {
                        Message = successResult.Message,
                        SupplierId = ((SupplierGetVModel)successResult.Data).Id
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating supplier: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the supplier.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật nhà cung cấp
        [HttpPut]
        public async Task<IActionResult> UpdateSupplier([FromForm] SupplierUpdateVModel supplierVModel, IFormFile? imageFile = null)
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

                    // Tìm nhà cung cấp để lấy URL ảnh cũ
                    var existingSupplier = await _context.Suppliers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Id == supplierVModel.Id);
                    if (existingSupplier == null)
                    {
                        return NotFound($"Supplier not found with ID: {supplierVModel.Id}");
                    }

                    // Xóa ảnh cũ trên Cloudinary nếu tồn tại
                    if (!string.IsNullOrEmpty(existingSupplier.ImageUrl))
                    {
                        try
                        {
                            // Trích xuất PublicId từ ImageUrl
                            var uri = new Uri(existingSupplier.ImageUrl);
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

                // B2: Gọi service để cập nhật nhà cung cấp
                var result = await _supplierService.UpdateAsync(supplierVModel, imageUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating supplier: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the supplier.", Error = ex.Message });
            }
        }

        // [5.] Xóa nhà cung cấp theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            try
            {
                // B1: Tìm nhà cung cấp để lấy ImageUrl
                var supplier = await _context.Suppliers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (supplier == null)
                {
                    return NotFound($"Supplier not found with ID: {id}");
                }

                // B2: Xóa ảnh trên Cloudinary nếu tồn tại
                if (!string.IsNullOrEmpty(supplier.ImageUrl))
                {
                    try
                    {
                        // Trích xuất PublicId từ ImageUrl
                        var uri = new Uri(supplier.ImageUrl);
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

                // B3: Gọi service để xóa nhà cung cấp
                var result = await _supplierService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B4: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting supplier: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the supplier.", Error = ex.Message });
            }
        }
    }
}