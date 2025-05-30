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
    public class SizeController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly ISizeService _sizeService;
        private readonly ILogger<SizeController> _logger;

        // Constructor nhận các dependency qua DI
        public SizeController(ISizeService sizeService, ILogger<SizeController> logger, TomfurnitureContext context)
        {
            _context = context;
            _sizeService = sizeService;
            _logger = logger;
        }

        // [1.] Lấy danh sách tất cả kích thước
        [HttpGet]
        public async Task<List<SizeGetVModel>> GetAllSize()
        {
            // Gọi service để lấy danh sách tất cả kích thước
            return await _sizeService.GetAllAsync();
        }

        // [2.] Lấy kích thước theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdSize(int id)
        {
            // Gọi service để lấy kích thước theo ID
            var size = await _sizeService.GetByIdAsync(id);
            if (size == null)
            {
                return NotFound(new { Message = "Size not found." });
            }
            return Ok(size);
        }

        // [3.] Tạo kích thước mới
        [HttpPost]
        public async Task<IActionResult> CreateSize([FromBody] SizeCreateVModel sizeVModel)
        {
            try
            {
                // B1: Gọi service để tạo kích thước
                var result = await _sizeService.CreateAsync(sizeVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetByIdSize),
                    new { id = ((SizeGetVModel)successResult.Data).Id },
                    new
                    {
                        Message = successResult.Message,
                        SizeId = ((SizeGetVModel)successResult.Data).Id
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating size: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the size.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật kích thước
        [HttpPut]
        public async Task<IActionResult> UpdateSize([FromForm] SizeUpdateVModel sizeVModel)
        {
            try
            {
                // B1: Gọi service để cập nhật kích thước
                var result = await _sizeService.UpdateAsync(sizeVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating size: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the size.", Error = ex.Message });
            }
        }

        // [5.] Xóa kích thước theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(int id)
        {
            try
            {
                // B1: Gọi service để xóa kích thước
                var result = await _sizeService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting size: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the size.", Error = ex.Message });
            }
        }
    }
}
