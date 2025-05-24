using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateVModel productModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Yêu cầu POST /api/Product có dữ liệu không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }

                var result = await _productService.CreateAsync(productModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(nameof(GetById), new { id = successResult.Data.Id }, new
                {
                    Message = successResult.Message,
                    ProductId = successResult.Data.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm: {Error}", ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductGetVModel>> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductGetVModel>>> GetAllAsync()
        {
            var result = await _productService.GetAllAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProductUpdateVModel productModel)
        {
            try
            {
                if (productModel == null)
                {
                    _logger.LogWarning("Yêu cầu PUT /api/Product/{id} nhận được body null.", id);
                    return BadRequest(new { Message = "Dữ liệu sản phẩm không được cung cấp." });
                }

                // Ghi đè ID trong model bằng ID từ URL
                productModel.Id = id;
                _logger.LogInformation("Đã ghi đè productModel.Id thành {id} từ URL.", id);

                // Gọi service để cập nhật sản phẩm
                var result = await _productService.UpdateAsync(productModel);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Cập nhật sản phẩm thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                return Ok(new
                {
                    Message = result.Message,
                    Data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm với ID {id}: {Error}", id, ex.Message);
                return StatusCode(500, new { Message = $"Lỗi server khi cập nhật sản phẩm: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Message = result.Message });
            }
            return Ok(result);
        }
    }
}