using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ admin có quyền truy cập
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService _revenueService;
        private readonly ILogger<RevenueController> _logger;

        public RevenueController(IRevenueService revenueService, ILogger<RevenueController> logger)
        {
            _revenueService = revenueService ?? throw new ArgumentNullException(nameof(revenueService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> GetRevenue([FromBody] RevenueRequestVModel request)
        {
            try
            {
                _logger.LogInformation("Yêu cầu thống kê doanh thu từ {StartDate} đến {EndDate} theo {TimeUnit}",
                    request.StartDate, request.EndDate, request.TimeUnit);

                var response = await _revenueService.GetRevenueAsync(request);

                _logger.LogInformation("Thống kê doanh thu thành công. Tổng doanh thu ròng: {TotalNetRevenue}",
                    response.TotalNetRevenue);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Dữ liệu đầu vào không hợp lệ: {Error}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi thống kê doanh thu: {Error}, InnerException: {InnerError}",
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { Message = "Lỗi khi thống kê doanh thu.", Error = ex.Message });
            }
        }
    }
}