using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using System;
using System.Threading.Tasks;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        // Constructor để khởi tạo ProductService và Logger
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Tạo mới một sản phẩm
        /// </summary>
        /// <param name="productModel">Dữ liệu sản phẩm từ body request</param>
        /// <returns>Trả về thông tin sản phẩm vừa tạo hoặc thông báo lỗi</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateVModel productModel)
        {
            try
            {
                // Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid data in POST /api/Product: {Errors}",
                        string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(new { Message = "Invalid product data provided." });
                }

                // Gọi service để tạo sản phẩm mới
                var result = await _productService.CreateAsync(productModel);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to create product: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                // Trả về kết quả thành công với ID sản phẩm
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(nameof(GetById), new { id = successResult.Data.Id }, new
                {
                    Message = successResult.Message,
                    ProductId = successResult.Data.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the product." });
            }
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo ID
        /// </summary>
        /// <param name="id">ID của sản phẩm cần lấy</param>
        /// <returns>Trả về thông tin sản phẩm hoặc lỗi nếu không tìm thấy</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductGetVModel>> GetById(int id)
        {
            try
            {
                var result = await _productService.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Product with ID {id} not found.", id);
                    return NotFound(new { Message = $"Product with ID {id} not found." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product with ID {id}: {Error}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving the product." });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        /// <returns>Trả về danh sách sản phẩm</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductGetVModel>>> GetAllAsync()
        {
            try
            {
                var result = await _productService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all products: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving products." });
            }
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        /// <param name="productModel">Dữ liệu sản phẩm cần cập nhật, bao gồm ID trong body</param>
        /// <returns>Trả về thông tin sản phẩm đã cập nhật hoặc thông báo lỗi</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] ProductUpdateVModel productModel)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (productModel == null)
                {
                    _logger.LogWarning("Received null body in PUT /api/Product.");
                    return BadRequest(new { Message = "Product data is required." });
                }

                // Kiểm tra ID trong body
                if (productModel.Id <= 0)
                {
                    _logger.LogWarning("Invalid or missing product ID in body: {id}", productModel.Id);
                    return BadRequest(new { Message = "Valid product ID is required in the request body." });
                }

                // Gọi service để cập nhật sản phẩm
                var result = await _productService.UpdateAsync(productModel);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to update product: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                // Trả về kết quả thành công
                return Ok(new
                {
                    Message = result.Message,
                    Data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID {id}: {Error}", productModel?.Id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the product." });
            }
        }

        /// <summary>
        /// Xóa sản phẩm theo ID
        /// </summary>
        /// <param name="id">ID của sản phẩm cần xóa</param>
        /// <returns>Trả về thông báo thành công hoặc lỗi</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to delete product with ID {id}: {Message}", id, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID {id}: {Error}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the product." });
            }
        }

        /// <summary>
        /// Cập nhật biến thể sản phẩm theo ID trong body
        /// </summary>
        /// <param name="variantModel">Dữ liệu biến thể sản phẩm cần cập nhật</param>
        /// <returns>Trả về thông tin sản phẩm chứa biến thể đã cập nhật hoặc thông báo lỗi</returns>
        [HttpPut("variant")]
        public async Task<IActionResult> UpdateVariantAsync([FromBody] ProductVModel.ProductVariantUpdateVModel variantModel)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid data in PUT /api/Product/variant: {Errors}",
                        string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(new { Message = "Invalid product variant data provided." });
                }

                // Gọi service để cập nhật biến thể
                var result = await _productService.UpdateVariantAsync(variantModel);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to update product variant: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                // Trả về kết quả thành công
                return Ok(new
                {
                    Message = result.Message,
                    Data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product variant with ID {id}: {Error}",
                    variantModel?.Id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the product variant." });
            }
        }
    }
}