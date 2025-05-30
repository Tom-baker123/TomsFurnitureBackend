using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;
using System;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        // Constructor nhận các dependency qua DI
        public RoleController(IRoleService roleService, ILogger<RoleController> logger, TomfurnitureContext context)
        {
            _context = context;
            _roleService = roleService;
            _logger = logger;
        }

        // [1.] Lấy danh sách tất cả vai trò
        [HttpGet]
        public async Task<List<RoleGetVModel>> GetAllRole()
        {
            // Gọi service để lấy danh sách tất cả vai trò
            return await _roleService.GetAllAsync();
        }

        // [2.] Lấy vai trò theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdRole(int id)
        {
            // Gọi service để lấy vai trò theo ID
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { Message = "Role not found." });
            }
            return Ok(role);
        }

        // [3.] Tạo vai trò mới
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateVModel roleVModel)
        {
            try
            {
                // B1: Gọi service để tạo vai trò
                var result = await _roleService.CreateAsync(roleVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetByIdRole),
                    new { id = ((RoleGetVModel)successResult.Data).Id },
                    new
                    {
                        Message = successResult.Message,
                        RoleId = ((RoleGetVModel)successResult.Data).Id
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating role: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the role.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật vai trò
        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromForm] RoleUpdateVModel roleVModel)
        {
            try
            {
                // B1: Gọi service để cập nhật vai trò
                var result = await _roleService.UpdateAsync(roleVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating role: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the role.", Error = ex.Message });
            }
        }

        // [5.] Xóa vai trò theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                // B1: Gọi service để xóa vai trò
                var result = await _roleService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting role: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the role.", Error = ex.Message });
            }
        }
    }
}