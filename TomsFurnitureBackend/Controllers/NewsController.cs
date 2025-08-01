using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using System;
using System.Threading.Tasks;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsService newsService, ILogger<NewsController> logger)
        {
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllNews()
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy danh sách tất cả tin tức.");
                var news = await _newsService.GetAllAsync();
                _logger.LogInformation("Lấy danh sách {Count} tin tức thành công.", news.Count);
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy danh sách tin tức: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy danh sách tin tức.", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsById(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy tin tức với ID: {NewsId}", id);
                var news = await _newsService.GetByIdAsync(id);
                if (news == null)
                {
                    _logger.LogWarning("Không tìm thấy tin tức với ID: {NewsId}", id);
                    return NotFound(new { Message = "Không tìm thấy tin tức." });
                }
                _logger.LogInformation("Lấy tin tức {NewsId} thành công.", id);
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy tin tức.", Error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNews([FromForm] NewsCreateVModel model, IFormFile? imageFile)
        {
            try
            {
                var result = await _newsService.CreateAsync(model, imageFile, HttpContext);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                return CreatedAtAction(nameof(GetNewsById), new { id = ((NewsGetVModel)successResult.Data).Id }, successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo tin tức: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi tạo tin tức.", Error = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateNews([FromForm] NewsUpdateVModel model, IFormFile? imageFile)
        {
            try
            {
                var result = await _newsService.UpdateAsync(model, imageFile, HttpContext);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                    model.Id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi cập nhật tin tức.", Error = ex.Message, InnerException = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                var result = await _newsService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.Message });
                }

                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa tin tức {NewsId}: {Error}, InnerException: {InnerError}",
                    id, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi xóa tin tức.", Error = ex.Message });
            }
        }
    }
}