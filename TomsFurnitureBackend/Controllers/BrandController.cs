using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using TomsFurnitureBackend.Helpers;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly IBrandService _brandService;
        private readonly ILogger<BrandController> _logger;
        private readonly Cloudinary _cloudinary;

        // Constructor injection for services
        public BrandController(IBrandService brandService, ILogger<BrandController> logger, Cloudinary cloudinary, TomfurnitureContext context)
        {
            _context = context;
            _brandService = brandService;
            _logger = logger;
            _cloudinary = cloudinary;
        }

        // [1.] Lấy danh sách tất cả Thương hiệu
        [HttpGet]
        public async Task<List<BrandGetVModel>> GetAllBrand()
        {
            // Gọi service để lấy danh sách thương hiệu
            return await _brandService.GetAllAsync();
        }

        // [2.] Lấy thương hiệu theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdBrand(int id)
        {
            // Gọi service để lấy thương hiệu theo ID
            var brand = await _brandService.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound(new { Message = "Brand not found." });
            }
            return Ok(brand);
        }

        // [4.] Tạo thương hiệu mới
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm] BrandCreateVModel brandVModel, IFormFile? imageFile)
        {
            try
            {
                // B1: Xử lý upload hình ảnh nếu có
                string? imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        imageUrl = await CloudinaryHelper.HandleSliderImageUpload(_cloudinary, imageFile, _logger);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }

                // B2: Gọi service để tạo thương hiệu
                var result = await _brandService.CreateAsync(brandVModel, imageUrl);
                if (!result.IsSuccess) {
                    return BadRequest(result.Message); 
                }

                // B3: Trả về phản hồi thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(nameof(GetByIdBrand),
                    new { id = successResult.Id },
                    new { Message = successResult.Message, Data = successResult.Data.Id }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the brand.", Error = ex.Message });
            }
        }
        // [5.] Xóa thương hiệu theo ID
        [HttpDelete]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                // B1: Tìm thương hiệu để lấy ảnh URL cũ
                var brand = await _context.Brands
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (brand == null) {
                    return NotFound($"Not found ID {id} in category");
                }

                // B2: Xóa ảnh cũ trên Cloudinary nếu tồn tại
                if (!string.IsNullOrEmpty(brand.ImageUrl)) {
                    try
                    {
                        // Trích xuất PublicId từ ImageUrl
                        var uri = new Uri(brand.ImageUrl);
                        // Lấy tên ảnh từ url
                        var publicId = Path.GetFileNameWithoutExtension(uri.AbsolutePath);

                        // Xóa ảnh trên cloudinary
                        var deletionParams = new DeletionParams(publicId)
                        {
                            ResourceType = ResourceType.Image
                        };

                        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                        if(deletionResult.Error != null) {
                            _logger.LogWarning("Không thể xóa ảnh trên Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Message = "An error occurred while deleting the brand.", Details = ex.Message });
                    }
                }

                // B3: Gọi service để xóa thương hiệu theo ID
                var result = await _brandService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B4: Trả về kết quả thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the brand.", Details = ex.Message });
            }
        }
        // [5.] Câp nhật thương hiệu theo ID
        [HttpPut]
        public async Task<IActionResult> UpdateBrand([FromForm] BrandUpdateVModel brandVModel, IFormFile? imageFile = null) 
        {
            try {
                // B1: Xử lý upload hình ảnh nếu có
                string? imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        imageUrl = await CloudinaryHelper.HandleSliderImageUpload(_cloudinary, imageFile, _logger);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                    // Tìm thương hiệu để lấy ảnh URL cũ
                    var existingBrand = await _context.Brands
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.Id == brandVModel.Id);
                    if (existingBrand == null)
                    {
                        return NotFound($"Not found ID {brandVModel.Id} in category");
                    }
                    // Xóa ảnh cũ trên Cloudinary nếu tồn tại
                    if (!string.IsNullOrEmpty(existingBrand.ImageUrl))
                    {
                        try
                        {
                            // Trích xuất PublicId từ ImageUrl
                            var uri = new Uri(existingBrand.ImageUrl);
                            // Lấy tên ảnh từ url
                            var publicId = Path.GetFileNameWithoutExtension(uri.AbsolutePath);

                            // Xóa ảnh trên cloudinary 
                            var deletionParams = new DeletionParams (publicId){
                                ResourceType = ResourceType.Image,
                            };

                            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                            if (deletionResult.Error != null)
                            {
                                _logger.LogWarning("Không thể xóa ảnh cũ trên Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                            }


                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { Message = $"Error when delete old image in Cloudinary: {ex.Message}" });
                        }
                    }
                }
                // B2: Gọi service để cập nhật thương hiệu
                var result = await _brandService.UpdateAsync(brandVModel, imageUrl);
                if (!result.IsSuccess) {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error when updating brand: {ex.Message}" });
            }
        }
    }
}
