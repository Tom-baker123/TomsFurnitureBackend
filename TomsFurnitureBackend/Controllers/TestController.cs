using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Helpers;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly ITestService _testService;

        // Constructor injection for services
        public TestController(ITestService testService, TomfurnitureContext context)
        {
            _context = context;
            _testService = testService;
        }

        // [1.] Controller Lấy danh sách tất cả Test
        [HttpGet]
        public async Task<List<TestGetVModel>> GetAllTestAsync() {
            return await _testService.GetAllTestAsync();
        }

        // [2.] Controller Lấy Test theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestByIdAsync(int id) {
            var test = await _testService.GetTestByIdAsync(id);
            if (test == null)
            {
                return NotFound(new { Message = "Không tìm được test theo id." });
            }
            return Ok(test);
        }

        // [3.] Controller Tạo mới Test
        [HttpPost]
        public async Task<IActionResult> CreateTestAsync([FromBody] TestCreateVModel model) {
            try {
                var result = await _testService.CreateTestAsync(model);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                var successResult = result as SuccessResponseResult;
                return Ok("Đã tạo thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi khi thêm: {ex.Message}");
            }
        }

        // [4.] Controller Cập nhật Test
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTestAsync(int id, [FromBody] TestUpdateVModel model){
            try
            {
                var result = await _testService.UpdateTestAsync(id, model);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi khi cập nhật: {ex.Message}");
            }
        }

        // [5.] Controller Xóa Test
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestAsync(int id){
            try
            {
                // B1: Tìm thương hiệu để lấy ảnh URL cũ
                var test = await _context.Tests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (test == null)
                {
                    return NotFound($"Not found ID {id} in category");
                }

                // B3: Gọi service để xóa thương hiệu theo ID
                var result = await _testService.DeleteTestAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi khi xóa: {ex.Message}");
            }
        }

    }
}
