using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        // Constructor nhận các dependency
        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        // Phương thức lấy tất cả danh sách vật liệu
        [HttpGet]
        public async Task<List<MaterialGetVModel>> GetAllMaterials()
        {
            // Gọi service để lấy tất cả vật liệu
            return await _materialService.GetAllAsync();
        }

        // Phương thức lấy vật liệu theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Gọi service để lấy vật liệu theo ID
            var material = await _materialService.GetByIdAsync(id);
            if (material == null)
            {
                return NotFound($"Not Found Material With ID: {id}"); // Trả về 404 nếu không tìm thấy
            }
            return Ok(material); // Trả về 200 kèm dữ liệu vật liệu
        }

        // Phương thức tạo mới vật liệu
        [HttpPost]
        public async Task<IActionResult> CreateMaterial(MaterialCreateVModel materialModel)
        {
            try
            {
                // Gọi service để tạo mới vật liệu
                var result = await _materialService.CreateAsync(materialModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message); // Trả về 400 nếu có lỗi
                }

                //  Trả về 201 Created với dữ liệu vật liệu mới tạo
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = successResult.Id },
                    new { Message = successResult.Message, Data = successResult.Data.Id }
                );

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the material.", Error = ex.Message });
            }
        }

        // Phuong thức xóa vật liệu 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            try
            {
                // Gọi service để xóa vật liệu theo ID
                var result = await _materialService.DeleteAsync(id);
                if (!result.IsSuccess) {
                    return BadRequest(result.Message); // Trả về 400 nếu có lỗi
                }
                return Ok(result); // Trả về 200 OK nếu xóa thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the material.", Error = ex.Message });
            }
            // Gọi service để xóa vật liệu theo ID
        }

        // Phương thức cập nhật vật liệu
        [HttpPut]
        public async Task<IActionResult> UpdateMaterial([FromForm] MaterialUpdateVModel materialModel)
        {
            try
            {
                // Gọi service để cập nhật vật liệu
                var result = await _materialService.UpdateAsync(materialModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message); // Trả về 400 nếu có lỗi
                }

                return Ok(result); // Trả về 200 OK nếu cập nhật thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the material.", Error = ex.Message });
            }
        }
    }
}
