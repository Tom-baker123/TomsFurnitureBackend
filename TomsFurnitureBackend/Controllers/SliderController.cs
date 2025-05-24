using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        private readonly ISliderService _sliderService;
        private readonly ILogger<SliderController> _logger;
        private readonly Cloudinary _cloudinary;

        public SliderController(ISliderService sliderService, ILogger<SliderController> logger, Cloudinary cloudinary)
        {
            _sliderService = sliderService;
            _logger = logger;
            _cloudinary = cloudinary;
        }
        [HttpPost]
        public async Task<IActionResult> CreateSlider([FromForm] SliderCreateVModel slidervModel, IFormFile ImageFile)
        {
            try
            {
                // Kiểm tra trùng tên
                //var existingProduct = await _sliderService.GetProductByNameAsync(slidervModel.Name);
                //if (existingProduct != null) { return BadRequest(new { Message = "Product name already exists." }); }

                // Xử lý upload ảnh
                string imageUrl = null;
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                        return BadRequest("Unsupported file type");

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(ImageFile.FileName, ImageFile.OpenReadStream()),
                        //Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        return BadRequest($"Cloudinary upload failed: {uploadResult.Error.Message}");
                    }

                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;

                }

                // Gọi service
                var result = await _sliderService.Create(slidervModel, imageUrl);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Ép kiểu để lấy ID
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(nameof(GetById), new { id = successResult.Id }, new
                {
                    Message = successResult.Message,
                    ProductId = successResult.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating product: {Error}", ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<SliderGetVModel>?> GetById(int id)
        {
            // Trả về 1 Slider dựa vào ID
            var result = await _sliderService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<SliderGetVModel>> GetAllAsync()
        {
            // Trả về tất cả Slider
            return await _sliderService.GetAllAsync();
        }

        [HttpDelete]
        public async Task<ResponseResult> Delete(int id)
        {
            // Xóa Sldier dựa trên id 
            return await _sliderService.DeleteAsync(id);
        }
    }
}
   
