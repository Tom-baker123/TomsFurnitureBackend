using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using System;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionTypeController : ControllerBase
    {
        private readonly IPromotionTypeService _promotionTypeService;
        private readonly ILogger<PromotionTypeController> _logger;

        public PromotionTypeController(IPromotionTypeService promotionTypeService, ILogger<PromotionTypeController> logger)
        {
            _promotionTypeService = promotionTypeService ?? throw new ArgumentNullException(nameof(promotionTypeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // [1.] Lấy danh sách tất cả loại khuyến mãi
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPromotionTypes()
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy danh sách tất cả loại khuyến mãi.");
                var promotionTypes = await _promotionTypeService.GetAllAsync();
                _logger.LogInformation("Lấy danh sách {Count} loại khuyến mãi thành công.", promotionTypes.Count);
                return Ok(promotionTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy danh sách loại khuyến mãi: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy danh sách loại khuyến mãi.", Error = ex.Message });
            }
        }

        // [2.] Lấy loại khuyến mãi theo ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotionTypeById(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy loại khuyến mãi với ID: {PromotionTypeId}", id);
                var promotionType = await _promotionTypeService.GetByIdAsync(id);
                if (promotionType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại khuyến mãi với ID: {PromotionTypeId}", id);
                    return NotFound(new { Message = "Không tìm thấy loại khuyến mãi." });
                }
                _logger.LogInformation("Lấy loại khuyến mãi {PromotionTypeId} thành công.", id);
                return Ok(promotionType);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy loại khuyến mãi {PromotionTypeId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy loại khuyến mãi.", Error = ex.Message });
            }
        }

        // [3.] Tạo loại khuyến mãi mới
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePromotionType([FromForm] PromotionTypeCreateVModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu tạo loại khuyến mãi mới với tên: {PromotionTypeName}", model.PromotionTypeName);
                var createdBy = User.Identity?.Name ?? "System";
                var result = await _promotionTypeService.CreateAsync(model, createdBy);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Tạo loại khuyến mãi thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Tạo loại khuyến mãi thành công với ID: {PromotionTypeId}", ((PromotionTypeGetVModel)successResult.Data).Id);
                return CreatedAtAction(nameof(GetPromotionTypeById), new { id = ((PromotionTypeGetVModel)successResult.Data).Id }, successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo loại khuyến mãi: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi tạo loại khuyến mãi.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật loại khuyến mãi
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePromotionType([FromForm] PromotionTypeUpdateVModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật loại khuyến mãi với ID: {PromotionTypeId}", model.Id);
                var updatedBy = User.Identity?.Name ?? "System";
                var result = await _promotionTypeService.UpdateAsync(model, updatedBy);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Cập nhật loại khuyến mãi thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Cập nhật loại khuyến mãi {PromotionTypeId} thành công.", model.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật loại khuyến mãi {PromotionTypeId}: {Error}, InnerException: {InnerError}",
                    model.Id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi cập nhật loại khuyến mãi.", Error = ex.Message });
            }
        }

        // [5.] Xóa loại khuyến mãi
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePromotionType(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa loại khuyến mãi với ID: {PromotionTypeId}", id);
                var result = await _promotionTypeService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Xóa loại khuyến mãi thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Xóa loại khuyến mãi {PromotionTypeId} thành công.", id);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa loại khuyến mãi {PromotionTypeId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi xóa loại khuyến mãi.", Error = ex.Message });
            }
        }
    }
}