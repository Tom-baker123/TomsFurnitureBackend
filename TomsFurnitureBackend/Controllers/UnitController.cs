using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;
        private readonly ILogger<UnitController> _logger;
        public UnitController(IUnitService unitService, ILogger<UnitController> logger) { 
            _unitService = unitService;
            _logger = logger;
        }

        // Lấy tất cả các đơn vị
        [HttpGet]
        public async Task<IActionResult> GetAllAsync() { 
            // Trả về kết quả
            var result = await _unitService.GetAllAsync();
            return Ok(result);
        }

        // Lấy đơn vị theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var result = await _unitService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { Message = "Unit not found." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the unit." });
            }
        }
        // Cập nhật đơn vị
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UnitCreateVModel vModel)
        {
            try
            {
                var result = await _unitService.CreateAsync(vModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the unit." });
            }
        }

        // Xóa đơn vị theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var result = await _unitService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    BadRequest(new { Message = result.Message });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the unit." });
            }
        }

        // Sửa đơn vị theo ID
        // Cập nhật đơn vị theo ID
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromForm] UnitUpdateVModel model)
        {
            try
            {
                var result = await _unitService.UpdateAsync(model);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Cập nhật đơn vị thất bại: {Message}.", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi server khi cập nhật đơn vị." });
            }
        }
    }
}
