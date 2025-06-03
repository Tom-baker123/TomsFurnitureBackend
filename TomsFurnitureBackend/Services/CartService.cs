using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.CartVModel;

namespace TomsFurnitureBackend.Services
{
    public class CartService : ICartService
    {
        private readonly TomfurnitureContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<CartService> _logger;

        public CartService(TomfurnitureContext context, IAuthService authService, ILogger<CartService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task<int?> GetCurrentUserIdAsync(HttpContext httpContext)
        {
            var authStatus = await _authService.GetAuthStatusAsync(httpContext.User, httpContext);
            if (authStatus.IsAuthenticated && int.TryParse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        private List<CartCreateVModel> GetCartFromCookies(HttpContext httpContext)
        {
            var cartJson = httpContext.Request.Cookies["GuestCart"];
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartCreateVModel>();
            }
            try
            {
                return JsonConvert.DeserializeObject<List<CartCreateVModel>>(cartJson) ?? new List<CartCreateVModel>();
            }
            catch
            {
                _logger.LogWarning("Không thể giải mã cookie giỏ hàng.");
                return new List<CartCreateVModel>();
            }
        }

        private void SaveCartToCookies(HttpContext httpContext, List<CartCreateVModel> cartItems)
        {
            var cartJson = JsonConvert.SerializeObject(cartItems);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
            httpContext.Response.Cookies.Append("GuestCart", cartJson, cookieOptions);
        }

        private async Task<ResponseResult?> ValidateCartItemAsync(CartCreateVModel model)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == model.ProId))
            {
                return new ErrorResponseResult($"Sản phẩm với ID {model.ProId} không tồn tại.");
            }
            if (model.Quantity <= 0)
            {
                return new ErrorResponseResult("Số lượng phải lớn hơn 0.");
            }
            return null;
        }

        public async Task<ResponseResult> AddToCartAsync(CartCreateVModel model, HttpContext httpContext)
        {
            try
            {
                var validationResult = await ValidateCartItemAsync(model);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var userId = await GetCurrentUserIdAsync(httpContext);
                if (userId.HasValue)
                {
                    var existingCartItem = await _context.Carts
                        .FirstOrDefaultAsync(c => c.UserId == userId && c.ProId == model.ProId && c.IsActive == true);
                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity += model.Quantity;
                        existingCartItem.UpdatedDate = DateTime.UtcNow;
                        existingCartItem.UpdatedBy = userId.ToString();
                    }
                    else
                    {
                        var cartEntity = model.ToEntity(userId);
                        _context.Carts.Add(cartEntity);
                    }
                    await _context.SaveChangesAsync();
                    var cart = await GetCartAsync(httpContext);
                    return new SuccessResponseResult(cart, "Thêm vào giỏ hàng thành công.");
                }
                else
                {
                    var cartItems = GetCartFromCookies(httpContext);
                    var existingItem = cartItems.FirstOrDefault(c => c.ProId == model.ProId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += model.Quantity;
                    }
                    else
                    {
                        cartItems.Add(model);
                    }
                    SaveCartToCookies(httpContext, cartItems);
                    return new SuccessResponseResult(cartItems.Select(c => new CartGetVModel
                    {
                        Quantity = c.Quantity,
                        ProId = c.ProId
                    }).ToList(), "Thêm vào giỏ hàng thành công.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm vào giỏ hàng: {Error}", ex.Message);
                return new ErrorResponseResult($"Lỗi khi thêm vào giỏ hàng: {ex.Message}");
            }
        }

        public async Task<ResponseResult> UpdateCartAsync(CartUpdateVModel model, HttpContext httpContext)
        {
            try
            {
                var validationResult = await ValidateCartItemAsync(model);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var userId = await GetCurrentUserIdAsync(httpContext);
                if (userId.HasValue)
                {
                    var cartItem = await _context.Carts
                        .FirstOrDefaultAsync(c => c.Id == model.Id && c.UserId == userId && c.IsActive == true);
                    if (cartItem == null)
                    {
                        return new ErrorResponseResult($"Không tìm thấy mục giỏ hàng với ID {model.Id}.");
                    }
                    cartItem.UpdateEntity(model);
                    await _context.SaveChangesAsync();
                    var cart = await GetCartAsync(httpContext);
                    return new SuccessResponseResult(cart, "Cập nhật giỏ hàng thành công.");
                }
                else
                {
                    var cartItems = GetCartFromCookies(httpContext);
                    var existingItem = cartItems.FirstOrDefault(c => c.ProId == model.ProId);
                    if (existingItem == null)
                    {
                        return new ErrorResponseResult($"Không tìm thấy mục giỏ hàng với ProId {model.ProId}.");
                    }
                    existingItem.Quantity = model.Quantity;
                    SaveCartToCookies(httpContext, cartItems);
                    return new SuccessResponseResult(cartItems.Select(c => new CartGetVModel
                    {
                        Quantity = c.Quantity,
                        ProId = c.ProId
                    }).ToList(), "Cập nhật giỏ hàng thành công.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật giỏ hàng: {Error}", ex.Message);
                return new ErrorResponseResult($"Lỗi khi cập nhật giỏ hàng: {ex.Message}");
            }
        }

        public async Task<ResponseResult> RemoveFromCartAsync(int id, HttpContext httpContext)
        {
            try
            {
                var userId = await GetCurrentUserIdAsync(httpContext);
                if (userId.HasValue)
                {
                    var cartItem = await _context.Carts
                        .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.IsActive == true);
                    if (cartItem == null)
                    {
                        return new ErrorResponseResult($"Không tìm thấy mục giỏ hàng với ID {id}.");
                    }
                    cartItem.IsActive = false;
                    cartItem.UpdatedDate = DateTime.UtcNow;
                    cartItem.UpdatedBy = userId.ToString();
                    await _context.SaveChangesAsync();
                    var cart = await GetCartAsync(httpContext);
                    return new SuccessResponseResult(cart, "Xóa mục khỏi giỏ hàng thành công.");
                }
                else
                {
                    var cartItems = GetCartFromCookies(httpContext);
                    var existingItem = cartItems.FirstOrDefault(c => c.ProId == id);
                    if (existingItem == null)
                    {
                        return new ErrorResponseResult($"Không tìm thấy mục giỏ hàng với ProId {id}.");
                    }
                    cartItems.Remove(existingItem);
                    SaveCartToCookies(httpContext, cartItems);
                    return new SuccessResponseResult(cartItems.Select(c => new CartGetVModel
                    {
                        Quantity = c.Quantity,
                        ProId = c.ProId
                    }).ToList(), "Xóa mục khỏi giỏ hàng thành công.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa mục khỏi giỏ hàng: {Error}", ex.Message);
                return new ErrorResponseResult($"Lỗi khi xóa mục khỏi giỏ hàng: {ex.Message}");
            }
        }

        public async Task<List<CartGetVModel>> GetCartAsync(HttpContext httpContext)
        {
            var userId = await GetCurrentUserIdAsync(httpContext);
            if (userId.HasValue)
            {
                var cartItems = await _context.Carts
                    .Include(c => c.Pro)
                    .Where(c => c.UserId == userId && c.IsActive == true)
                    .ToListAsync();
                return cartItems.Select(c => c.ToGetVModel()).ToList();
            }
            else
            {
                var cartItems = GetCartFromCookies(httpContext);
                var productIds = cartItems.Select(c => c.ProId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();
                return cartItems.Select(c => new CartGetVModel
                {
                    Quantity = c.Quantity,
                    ProId = c.ProId,
                    ProductName = products.FirstOrDefault(p => p.Id == c.ProId)?.ProductName
                }).ToList();
            }
        }

        public async Task<ResponseResult> MergeCartFromCookiesAsync(HttpContext httpContext)
        {
            try
            {
                var userId = await GetCurrentUserIdAsync(httpContext);
                if (!userId.HasValue)
                {
                    return new ErrorResponseResult("Người dùng chưa đăng nhập.");
                }

                var cartItems = GetCartFromCookies(httpContext);
                if (!cartItems.Any())
                {
                    return new SuccessResponseResult(null, "Không có giỏ hàng trong cookie để hợp nhất.");
                }

                foreach (var item in cartItems)
                {
                    var validationResult = await ValidateCartItemAsync(item);
                    if (validationResult != null)
                    {
                        continue; // Bỏ qua các mục không hợp lệ
                    }

                    var existingCartItem = await _context.Carts
                        .FirstOrDefaultAsync(c => c.UserId == userId && c.ProId == item.ProId && c.IsActive == true);
                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity += item.Quantity;
                        existingCartItem.UpdatedDate = DateTime.UtcNow;
                        existingCartItem.UpdatedBy = userId.ToString();
                    }
                    else
                    {
                        var cartEntity = item.ToEntity(userId);
                        _context.Carts.Add(cartEntity);
                    }
                }

                await _context.SaveChangesAsync();
                httpContext.Response.Cookies.Delete("GuestCart");
                var cart = await GetCartAsync(httpContext);
                return new SuccessResponseResult(cart, "Hợp nhất giỏ hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hợp nhất giỏ hàng từ cookie: {Error}", ex.Message);
                return new ErrorResponseResult($"Lỗi khi hợp nhất giỏ hàng: {ex.Message}");
            }
        }
    }
}