using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

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
                var result = await _sliderService.CreateAsync(slidervModel, imageUrl);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Ép kiểu để lấy ID
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(nameof(GetById), new { id = successResult.Id }, new
                {
                    SliderID = successResult.Data?.Id,
                    Message = successResult.Message,
                    ProductId = successResult.Data?.ProductId
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Bước 1: Gọi service để xóa Slider
            var result = await _sliderService.DeleteAsync(id);

            // Bước 2: Trả về kết quả
            return Ok(new { Success = result.IsSuccess, Message = result.Message });
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromForm] SliderUpdateVModel sliderModel, IFormFile? ImageFile = null)
        {
            try
            {
                // Xử lý upload ảnh nếu có
                string? imageUrl = null;
                // Kiểm tra file ảnh khi truyền vào không null thì execute.
                if (ImageFile != null && ImageFile.Length > 0) {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", "webp"};
                    var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

                    // Kiểm tra định dạng file
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest("Format file not support!");
                    }

                    // Tạo đối tượng uploadParams để cấu hình các tham số upload ảnh lên Cloudinary
                    var uploadParams = new ImageUploadParams
                    {
                        // FileDescription gồm:
                        // - ImageFile.FileName: tên file gốc do người dùng upload (vd: "avatar.png")
                        // - ImageFile.OpenReadStream(): mở luồng đọc dữ liệu từ file upload đó (stream)
                        File = new FileDescription(ImageFile.FileName, ImageFile.OpenReadStream()),
                    };

                    // Thực thi upload ảnh
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    // Kiểm tra lỗi upload ảnh
                    if (uploadResult.Error != null) {
                        _logger.LogError("Lỗi upload Cloudinary: ", uploadResult.Error.Message);
                        return BadRequest($"Upload Cloudinary Failed: {uploadResult.Error.Message}");
                    }

                    // - Lấy đường dẫn ảnh từ Cloudinary: 
                    //  + uploadResult Đây là kết quả trả về từ Cloudinary sau khi bạn gọi UploadAsync() hoặc Upload().
                    //  + uploadResult.SecureUrl Đây là URL của hình ảnh đã được upload, sử dụng HTTPS(an toàn).
                    //  + .AbsoluteUri Chuyển URL từ kiểu Uri sang string để dễ sử dụng trong code, JSON, hoặc hiển thị.
                    //  + imageUrl Biến để lưu lại đường dẫn ảnh – bạn có thể lưu vào database hoặc trả về cho client.
                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // Gọi service để cập nhật ảnh
                var result = await _sliderService.UpdateAsync(sliderModel, imageUrl);

                // Kiểm tra cập nhật api có thành công không?
                if (!result.IsSuccess) { 
                    return BadRequest(result.Message); 
                }

                // Trả về thành công
                return Ok(result);
            }
            catch (Exception ex) {
                // _logger.LogError($"You have an error when updating the slider: {ex.Message}");
                // return StatusCode(500, new { Messsage = ex.Message });
                return BadRequest(ex.Message);
            }
        }
    }
}
   
