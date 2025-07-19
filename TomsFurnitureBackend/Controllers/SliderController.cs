using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;
using TomsFurnitureBackend.Helpers;
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
                    try
                    {
                        imageUrl = await CloudinaryHelper.HandleSliderImageUpload(_cloudinary, ImageFile, _logger);
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
                string? imageUrl = null;
                // Kiểm tra file ảnh khi truyền vào không null thì execute.
                if (ImageFile != null && ImageFile.Length > 0) {
                    try
                    {
                        imageUrl = await CloudinaryHelper.HandleSliderImageUpload(_cloudinary, ImageFile, _logger);
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
