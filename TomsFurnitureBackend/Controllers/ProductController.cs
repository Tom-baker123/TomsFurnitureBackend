﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using System;
using System.Threading.Tasks;
using TomsFurnitureBackend.Common.Models;
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
                if (successResult == null)
                {
                    _logger.LogWarning("Result is not a SuccessResponseResult when creating product.");
                    return StatusCode(500, new { Message = "Unexpected error occurred while creating the product." });
                }
                if (successResult.Data == null)
                {
                    _logger.LogWarning("SuccessResponseResult.Data is null when creating product.");
                    return StatusCode(500, new { Message = "Unexpected error occurred while creating the product." });
                }
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
        /// Lấy thông tin biến thể sản phẩm theo ID
        /// </summary>
        /// <param name="variantId">ID của biến thể sản phẩm cần lấy</param>
        /// <returns>Trả về thông tin biến thể sản phẩm hoặc lỗi nếu không tìm thấy</returns>
        [HttpGet("variant/{variantId}")]
        public async Task<ActionResult<ProductVModel.ProductVariantGetVModel>> GetVariantById(int variantId)
        {
            try
            {
                var result = await _productService.GetVariantByIdAsync(variantId);
                if (result == null)
                {
                    _logger.LogWarning("Product variant with ID {variantId} not found.", variantId);
                    return NotFound(new { Message = $"Product variant with ID {variantId} not found." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product variant with ID {variantId}: {Error}", variantId, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving the product variant." });
            }
        }

        /// <summary>
        /// Kiểm tra biến thể sản phẩm theo mã/tên sản phẩm và thuộc tính (màu sắc, kích thước, vật liệu)
        /// </summary>
        /// <param name="productIdentifier">Mã (ID) hoặc tên sản phẩm</param>
        /// <param name="colorId">ID của màu sắc</param>
        /// <param name="sizeId">ID của kích thước</param>
        /// <param name="materialId">ID của vật liệu</param>
        /// <returns>Trả về ID của biến thể hoặc thông báo lỗi</returns>
        [HttpGet("variant/check")]
        public async Task<IActionResult> GetVariantIdByAttributes(
            [FromQuery] string productIdentifier,
            [FromQuery] int colorId,
            [FromQuery] int sizeId,
            [FromQuery] int materialId)
        {
            try
            {
                // Bước 1: Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(productIdentifier))
                {
                    _logger.LogWarning("Invalid or missing product identifier in GET /api/Product/variant/check.");
                    return BadRequest(new { Message = "Product identifier is required." });
                }
                if (colorId <= 0)
                {
                    _logger.LogWarning("Invalid color ID in GET /api/Product/variant/check: {colorId}", colorId);
                    return BadRequest(new { Message = "Valid Color ID is required." });
                }
                if (sizeId <= 0)
                {
                    _logger.LogWarning("Invalid size ID in GET /api/Product/variant/check: {sizeId}", sizeId);
                    return BadRequest(new { Message = "Valid Size ID is required." });
                }
                if (materialId <= 0)
                {
                    _logger.LogWarning("Invalid material ID in GET /api/Product/variant/check: {materialId}", materialId);
                    return BadRequest(new { Message = "Valid Material ID is required." });
                }

                // Bước 2: Gọi service để kiểm tra biến thể
                var result = await _productService.GetVariantIdByAttributesAsync(productIdentifier, colorId, sizeId, materialId);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve variant ID for product identifier {productIdentifier}: {Message}", productIdentifier, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                // Bước 3: Kiểm tra dữ liệu trả về từ service
                var successResult = result as SuccessResponseResult;
                if (successResult == null || successResult.Data == null)
                {
                    _logger.LogWarning("Unexpected null data when retrieving variant ID for product identifier {productIdentifier}", productIdentifier);
                    return StatusCode(500, new { Message = "Unexpected error occurred while retrieving the product variant ID." });
                }

                // Bước 4: Trả về kết quả thành công với ID biến thể
                return Ok(new
                {
                    Message = result.Message,
                    Success = true,
                    VariantId = successResult?.Data
                });
            }
            catch (Exception ex)
            {
                // Bước 5: Ghi log lỗi và trả về thông báo lỗi
                _logger.LogError(ex, "Error occurred while retrieving variant ID for product identifier {productIdentifier}: {Error}", productIdentifier, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving the product variant ID." });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        /// <returns>Trả về danh sách sản phẩm</returns>
        [HttpGet]
        public async Task<ActionResult<PaginationModel<ProductGetVModel>>> GetAllAsync([FromQuery] ProductFilterParams param)
        {
            try
            {
                var result = await _productService.GetAllAsync(param);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all products: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving products." });
            }
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm và các biến thể
        /// </summary>
        /// <param name="productModel">Dữ liệu sản phẩm cần cập nhật, bao gồm ID và danh sách biến thể trong body</param>
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

                // Gọi service để cập nhật sản phẩm và biến thể
                var result = await _productService.UpdateAsync(productModel);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to update product: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                // Kiểm tra result.Data để tránh dereference null
                if (result.Data == null)
                {
                    _logger.LogWarning("Unexpected null data when updating product with ID {id}", productModel.Id);
                    return StatusCode(500, new { Message = "Unexpected error occurred while updating the product." });
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
                _logger.LogError(ex, "Error occurred while updating product with ID {id}: {Error}", productModel?.Id ?? 0, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the product and its variants." });
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

                // Kiểm tra result.Data để tránh dereference null
                if (result.Data == null)
                {
                    _logger.LogWarning("Unexpected null data when updating product variant with ID {id}", variantModel.Id);
                    return StatusCode(500, new { Message = "Unexpected error occurred while updating the product variant." });
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
                    variantModel?.Id ?? 0, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the product variant." });
            }
        }

        /// <summary>
        /// Xóa biến thể sản phẩm theo ID
        /// </summary>
        /// <param name="variantId">ID của biến thể sản phẩm cần xóa</param>
        /// <returns>Trả về thông báo thành công hoặc lỗi</returns>
        [HttpDelete("variant/{variantId}")]
        public async Task<IActionResult> DeleteVariant(int variantId)
        {
            try
            {
                // Gọi service để xóa biến thể
                var result = await _productService.DeleteVariantAsync(variantId);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to delete product variant with ID {variantId}: {Message}", variantId, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product variant with ID {variantId}: {Error}", variantId, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the product variant." });
            }
        }
    }
}