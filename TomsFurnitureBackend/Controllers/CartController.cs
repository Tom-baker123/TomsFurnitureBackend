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

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart([FromBody] CartCreateVModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Yêu cầu thêm vào giỏ hàng không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }

                var result = await _cartService.AddToCartAsync(model, HttpContext);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Thêm vào giỏ hàng thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Thêm vào giỏ hàng thành công cho sản phẩm ID: {ProId}", model.ProId);
                return Ok(new { Message = successResult.Message, Cart = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm vào giỏ hàng: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Lỗi khi thêm vào giỏ hàng.", Error = ex.Message });
            }
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCart([FromBody] CartUpdateVModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Yêu cầu cập nhật giỏ hàng không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }

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
                return StatusCode(500, new { Message = "Lỗi khi cập nhật giỏ hàng.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
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
                return StatusCode(500, new { Message = "Lỗi khi xóa mục khỏi giỏ hàng.", Error = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var cart = await _cartService.GetCartAsync(HttpContext);
                _logger.LogInformation("Lấy giỏ hàng thành công.");
                return Ok(new { Cart = cart });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy giỏ hàng: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Lỗi khi lấy giỏ hàng.", Error = ex.Message });
            }
        }

        [HttpPost("merge")]
        [Authorize]
        public async Task<IActionResult> MergeCart()
        {
            try
            {
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
                return StatusCode(500, new { Message = "Lỗi khi hợp nhất giỏ hàng.", Error = ex.Message });
            }
        }
    }
}