using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;
        private readonly ILogger<ColorController> _logger;
        private readonly TomfurnitureContext _context;


        // Constructor injection for services
        public ColorController(IColorService colorService, ILogger<ColorController> logger, TomfurnitureContext context)
        {
            _colorService = colorService;
            _logger = logger;
            _context = context;
        }

        // [1.] Lấy tất cả Màu sắc  
        [HttpGet]
        public async Task<List<ColorGetVModel>> GetAllAsync()
        {
            // Gọi service để lấy danh sách tất cả danh mục
            return await _colorService.GetAllAsync();
        }

        // [2.] Lấy Màu sắc theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Gọi service để lấy danh mục theo ID
            var result = await _colorService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound($"- Not found category with ID: {id}");
            }
            return Ok(result);
        }

        // [3.] Tạo mới Màu sắc
        [HttpPost]
        public async Task<IActionResult> CreateColor(ColorCreateVModel colorVModel)
        {
            try
            {
                // B1: Gọi service để tạo mới danh mục
                var result = await _colorService.CreateAsync(colorVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về kết quả thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = successResult?.Id },
                    new { Message = successResult?.Message, Data = successResult?.Data?.Id }
                );

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"You have an error: {ex.Message}" });
            }
        }

        // [4.] Cập nhật Màu sắc
        [HttpPut]
        public async Task<IActionResult> UpdateColor([FromForm] ColorUpdateVModel colorVModel)
        {
            try
            {
                // B4: Gọi service để cập nhật danh mục
                var result = await _colorService.UpdateAsync(colorVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B5: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error when updating color: {ex.Message}" });
            }
        }

        // [5.] Xóa danh mục theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                // Gọi service để xóa danh mục
                var result = await _colorService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error when Delete category: {ex.Message}" });
            }
        }
    }
}
