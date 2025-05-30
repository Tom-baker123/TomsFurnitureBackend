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

        // Constructor nhận các dependency qua DI
        public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger, TomfurnitureContext context)
        {
            _context = context;
            _feedbackService = feedbackService;
            _logger = logger;
        }

        // [1.] Lấy danh sách tất cả phản hồi
        [HttpGet]
        public async Task<List<FeedbackGetVModel>> GetAllFeedback()
        {
            // Gọi service để lấy danh sách tất cả phản hồi
            return await _feedbackService.GetAllAsync();
        }

        // [2.] Lấy phản hồi theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdFeedback(int id)
        {
            // Gọi service để lấy phản hồi theo ID
            var feedback = await _feedbackService.GetByIdAsync(id);
            if (feedback == null)
            {
                return NotFound(new { Message = "Feedback not found." });
            }
            return Ok(feedback);
        }

        // [3.] Tạo phản hồi mới
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateVModel feedbackVModel)
        {
            try
            {
                // B1: Gọi service để tạo phản hồi
                var result = await _feedbackService.CreateAsync(feedbackVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
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
                _logger.LogError("Error creating feedback: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the feedback.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật phản hồi
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackUpdateVModel feedbackVModel)
        {
            try
            {
                // Kiểm tra ID hợp lệ
                if (id != feedbackVModel.Id)
                {
                    return BadRequest("ID in URL does not match ID in model.");
                }

                // B1: Gọi service để cập nhật phản hồi
                var result = await _feedbackService.UpdateAsync(feedbackVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating feedback: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the feedback.", Error = ex.Message });
            }
        }

        // [5.] Xóa phản hồi theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                // B1: Gọi service để xóa phản hồi
                var result = await _feedbackService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting feedback: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the feedback.", Error = ex.Message });
            }
        }
    }
}