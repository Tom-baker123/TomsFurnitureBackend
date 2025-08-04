using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using System;
using System.Threading.Tasks;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger, TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy danh sách tất cả phản hồi
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFeedback()
        {
            try
            {
                _logger.LogInformation("Lấy danh sách tất cả phản hồi.");
                var feedbacks = await _feedbackService.GetAllAsync();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phản hồi.");
                return StatusCode(500, new { Message = "Lỗi khi lấy danh sách phản hồi.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy phản hồi theo ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdFeedback(int id)
        {
            try
            {
                _logger.LogInformation("Lấy phản hồi với ID: {Id}", id);
                var feedback = await _feedbackService.GetByIdAsync(id);
                if (feedback == null)
                {
                    return NotFound(new { Message = "Không tìm thấy phản hồi." });
                }
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy phản hồi với ID: {Id}", id);
                return StatusCode(500, new { Message = "Lỗi khi lấy phản hồi.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo phản hồi mới
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateVModel feedbackVModel)
        {
            try
            {
                if (feedbackVModel == null)
                {
                    _logger.LogWarning("Dữ liệu phản hồi không hợp lệ.");
                    return BadRequest(new { Message = "Dữ liệu phản hồi không hợp lệ." });
                }

                _logger.LogInformation("Tạo phản hồi mới.");
                var result = await _feedbackService.CreateAsync(feedbackVModel, HttpContext);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetByIdFeedback),
                    new { id = ((FeedbackGetVModel)successResult.Data).Id },
                    new
                    {
                        Message = successResult.Message,
                        FeedbackId = ((FeedbackGetVModel)successResult.Data).Id
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo phản hồi.");
                return StatusCode(500, new { Message = "Lỗi khi tạo phản hồi.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật phản hồi
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackUpdateVModel feedbackVModel)
        {
            try
            {
                if (id != feedbackVModel.Id)
                {
                    _logger.LogWarning("ID trong URL không khớp với ID trong model.");
                    return BadRequest(new { Message = "ID trong URL không khớp với ID trong model." });
                }

                _logger.LogInformation("Cập nhật phản hồi với ID: {Id}", id);
                var result = await _feedbackService.UpdateAsync(feedbackVModel, HttpContext);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.Message });
                }

                return Ok(new { Message = ((SuccessResponseResult)result).Message, Feedback = ((SuccessResponseResult)result).Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật phản hồi với ID: {Id}", id);
                return StatusCode(500, new { Message = "Lỗi khi cập nhật phản hồi.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa phản hồi theo ID
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                _logger.LogInformation("Xóa phản hồi với ID: {Id}", id);
                var result = await _feedbackService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.Message });
                }

                return Ok(new { Message = ((SuccessResponseResult)result).Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa phản hồi với ID: {Id}", id);
                return StatusCode(500, new { Message = "Lỗi khi xóa phản hồi.", Error = ex.Message });
            }
        }
    }
}