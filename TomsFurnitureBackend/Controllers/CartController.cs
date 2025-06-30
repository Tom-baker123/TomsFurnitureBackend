using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using System.Threading.Tasks;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.CartVModel;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        // Khởi tạo dependencies
        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart([FromBody] CartCreateVModel model)
        {
            try
            {
                // Kiểm tra tính hợp lệ của model
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Yêu cầu thêm vào giỏ hàng không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }

                // Gọi service để thêm vào giỏ hàng
                var result = await _cartService.AddToCartAsync(model, HttpContext);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Thêm vào giỏ hàng thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Thêm vào giỏ hàng thành công cho biến thể sản phẩm ID: {ProVarId}", model.ProVarId);
                return Ok(new { Message = successResult.Message, Cart = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm vào giỏ hàng: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Error adding to cart.", Error = ex.Message });
            }
        }

        // Cập nhật giỏ hàng
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCart([FromBody] CartUpdateVModel model)
        {
            try
            {
                // Kiểm tra tính hợp lệ của model
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Yêu cầu cập nhật giỏ hàng không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }

                // Gọi service để cập nhật giỏ hàng
                var result = await _cartService.UpdateCartAsync(model, HttpContext);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Cập nhật giỏ hàng thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Cập nhật giỏ hàng thành công cho ID: {Id}", model.Id);
                return Ok(new { Message = successResult.Message, Cart = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật giỏ hàng: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Error updating cart.", Error = ex.Message });
            }
        }

        // Xóa mục khỏi giỏ hàng
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
                // Gọi service để xóa mục khỏi giỏ hàng
                var result = await _cartService.RemoveFromCartAsync(id, HttpContext);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Xóa mục khỏi giỏ hàng thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Xóa mục khỏi giỏ hàng thành công cho ID: {Id}", id);
                return Ok(new { Message = successResult.Message, Cart = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa mục khỏi giỏ hàng: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Error removing from cart.", Error = ex.Message });
            }
        }

        // Lấy giỏ hàng
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                // Gọi service để lấy giỏ hàng
                var cart = await _cartService.GetCartAsync(HttpContext);
                _logger.LogInformation("Lấy giỏ hàng thành công.");
                return Ok(new { Cart = cart });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy giỏ hàng: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Error retrieving cart.", Error = ex.Message });
            }
        }

        // Hợp nhất giỏ hàng từ cookies
        [HttpPost("merge")]
        [Authorize]
        public async Task<IActionResult> MergeCart()
        {
            try
            {
                // Gọi service để hợp nhất giỏ hàng
                var result = await _cartService.MergeCartFromCookiesAsync(HttpContext);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Hợp nhất giỏ hàng thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Hợp nhất giỏ hàng thành công.");
                return Ok(new { Message = successResult.Message, Cart = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hợp nhất giỏ hàng: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Error merging cart.", Error = ex.Message });
            }
        }
    }
}