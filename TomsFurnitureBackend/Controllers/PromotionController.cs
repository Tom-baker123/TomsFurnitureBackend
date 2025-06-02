using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using System;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(IPromotionService promotionService, ILogger<PromotionController> logger)
        {
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // [1.] Lấy danh sách tất cả khuyến mãi
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPromotions()
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy danh sách tất cả khuyến mãi.");
                var promotions = await _promotionService.GetAllAsync();
                _logger.LogInformation("Lấy danh sách {Count} khuyến mãi thành công.", promotions.Count);
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy danh sách khuyến mãi: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy danh sách khuyến mãi.", Error = ex.Message });
            }
        }

        // [2.] Lấy khuyến mãi theo ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotionById(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy khuyến mãi với ID: {PromotionId}", id);
                var promotion = await _promotionService.GetByIdAsync(id);
                if (promotion == null)
                {
                    _logger.LogWarning("Không tìm thấy khuyến mãi với ID: {PromotionId}", id);
                    return NotFound(new { Message = "Không tìm thấy khuyến mãi." });
                }
                _logger.LogInformation("Lấy khuyến mãi {PromotionId} thành công.", id);
                return Ok(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy khuyến mãi {PromotionId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy khuyến mãi.", Error = ex.Message });
            }
        }

        // [3.] Tạo khuyến mãi mới
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePromotion([FromForm] PromotionCreateVModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu tạo khuyến mãi mới với mã: {PromotionCode}", model.PromotionCode);
                var createdBy = User.Identity?.Name ?? "System";
                var result = await _promotionService.CreateAsync(model, createdBy);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Tạo khuyến mãi thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Tạo khuyến mãi thành công với ID: {PromotionId}", ((PromotionGetVModel)successResult.Data).Id);
                return CreatedAtAction(nameof(GetPromotionById), new { id = ((PromotionGetVModel)successResult.Data).Id }, successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo khuyến mãi: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi tạo khuyến mãi.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật khuyến mãi
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePromotion([FromForm] PromotionUpdateVModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật khuyến mãi với ID: {PromotionId}", model.Id);
                var updatedBy = User.Identity?.Name ?? "System";
                var result = await _promotionService.UpdateAsync(model, updatedBy);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Cập nhật khuyến mãi thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Cập nhật khuyến mãi {PromotionId} thành công.", model.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật khuyến mãi {PromotionId}: {Error}, InnerException: {InnerError}",
                    model.Id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi cập nhật khuyến mãi.", Error = ex.Message });
            }
        }

        // [5.] Xóa khuyến mãi
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa khuyến mãi với ID: {PromotionId}", id);
                var result = await _promotionService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Xóa khuyến mãi thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Xóa khuyến mãi {PromotionId} thành công.", id);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa khuyến mãi {PromotionId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi xóa khuyến mãi.", Error = ex.Message });
            }
        }
    }
}