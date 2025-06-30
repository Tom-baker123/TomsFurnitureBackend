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

        // Khởi tạo dependencies
        public CartService(TomfurnitureContext context, IAuthService authService, ILogger<CartService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Lấy UserId hiện tại từ HttpContext
        private async Task<int?> GetCurrentUserIdAsync(HttpContext httpContext)
        {
            var authStatus = await _authService.GetAuthStatusAsync(httpContext.User, httpContext);
            if (authStatus.IsAuthenticated && int.TryParse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        // Lấy giỏ hàng từ cookies
        private List<CartCreateVModel> GetCartFromCookies(HttpContext httpContext)
        {
            var cartJson = httpContext.Request.Cookies["GuestCart"];
            if (string.IsNullOrEmpty(cartJson))
            {
                _logger.LogInformation("Không tìm thấy cookie GuestCart."); // Logger 1
                return new List<CartCreateVModel>();
            }
            try
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartCreateVModel>>(cartJson) ?? new List<CartCreateVModel>();
                _logger.LogInformation("Đọc cookie GuestCart thành công với {Count} mục: {Content}", cartItems.Count, cartJson); // Logger 2
                return cartItems;
            }
            catch
            {
                _logger.LogWarning("Không thể giải mã cookie giỏ hàng.");
                return new List<CartCreateVModel>();
            }
        }

        // Lưu giỏ hàng vào cookies
        private void SaveCartToCookies(HttpContext httpContext, List<CartCreateVModel> cartItems)
        {
            var cartJson = JsonConvert.SerializeObject(cartItems);
            _logger.LogInformation("Lưu cookie GuestCart với nội dung: {Content}", cartJson); // Logger 3
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
            httpContext.Response.Cookies.Append("GuestCart", cartJson, cookieOptions);
        }

        // Kiểm tra tính hợp lệ của mục giỏ hàng
        private async Task<ResponseResult?> ValidateCartItemAsync(CartCreateVModel model)
        {
            if (!await _context.ProductVariants.AnyAsync(p => p.Id == model.ProVarId))
            {
                return new ErrorResponseResult($"Biến thể sản phẩm với ID {model.ProVarId} không tồn tại.");
            }
            if (model.Quantity <= 0)
            {
                return new ErrorResponseResult("Số lượng phải lớn hơn 0.");
            }
            return null;
        }

        // Thêm sản phẩm vào giỏ hàng
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
                        .FirstOrDefaultAsync(c => c.UserId == userId && c.ProVarId == model.ProVarId && c.IsActive == true);
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
                    return new SuccessResponseResult(cart, "Added to cart successfully.");
                }
                else
                {
                    var cartItems = GetCartFromCookies(httpContext);
                    var existingItem = cartItems.FirstOrDefault(c => c.ProVarId == model.ProVarId);
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
                        ProVarId = c.ProVarId
                    }).ToList(), "Added to cart successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart: {Error}", ex.Message);
                return new ErrorResponseResult($"Error adding to cart: {ex.Message}");
            }
        }

        // Cập nhật giỏ hàng
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
                        return new ErrorResponseResult($"Cart item with ID {model.Id} not found.");
                    }
                    cartItem.UpdateEntity(model);
                    await _context.SaveChangesAsync();
                    var cart = await GetCartAsync(httpContext);
                    return new SuccessResponseResult(cart, "Cart updated successfully.");
                }
                else
                {
                    var cartItems = GetCartFromCookies(httpContext);
                    var existingItem = cartItems.FirstOrDefault(c => c.ProVarId == model.ProVarId);
                    if (existingItem == null)
                    {
                        return new ErrorResponseResult($"Cart item with ProVarId {model.ProVarId} not found.");
                    }
                    existingItem.Quantity = model.Quantity;
                    SaveCartToCookies(httpContext, cartItems);
                    return new SuccessResponseResult(cartItems.Select(c => new CartGetVModel
                    {
                        Quantity = c.Quantity,
                        ProVarId = c.ProVarId
                    }).ToList(), "Cart updated successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart: {Error}", ex.Message);
                return new ErrorResponseResult($"Error updating cart: {ex.Message}");
            }
        }

        // Xóa mục khỏi giỏ hàng
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
                        return new ErrorResponseResult($"Cart item with ID {id} not found.");
                    }
                    cartItem.IsActive = false;
                    cartItem.UpdatedDate = DateTime.UtcNow;
                    cartItem.UpdatedBy = userId.ToString();
                    await _context.SaveChangesAsync();
                    var cart = await GetCartAsync(httpContext);
                    return new SuccessResponseResult(cart, "Item removed from cart successfully.");
                }
                else
                {
                    var cartItems = GetCartFromCookies(httpContext);
                    var existingItem = cartItems.FirstOrDefault(c => c.ProVarId == id);
                    if (existingItem == null)
                    {
                        return new ErrorResponseResult($"Cart item with ProVarId {id} not found.");
                    }
                    cartItems.Remove(existingItem);
                    SaveCartToCookies(httpContext, cartItems);
                    return new SuccessResponseResult(cartItems.Select(c => new CartGetVModel
                    {
                        Quantity = c.Quantity,
                        ProVarId = c.ProVarId
                    }).ToList(), "Item removed from cart successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from cart: {Error}", ex.Message);
                return new ErrorResponseResult($"Error removing from cart: {ex.Message}");
            }
        }

        // Lấy giỏ hàng
        public async Task<List<CartGetVModel>> GetCartAsync(HttpContext httpContext)
        {
            var userId = await GetCurrentUserIdAsync(httpContext);
            if (userId.HasValue)
            {
                var cartItems = await _context.Carts
                    .Include(c => c.ProVar)
                    .ThenInclude(pv => pv.Product)
                    .Where(c => c.UserId == userId && c.IsActive == true)
                    .ToListAsync();
                return cartItems.Select(c => c.ToGetVModel()).ToList();
            }
            else
            {
                var cartItems = GetCartFromCookies(httpContext);
                var productVarIds = cartItems.Select(c => c.ProVarId).ToList();
                var productVariants = await _context.ProductVariants
                    .Include(pv => pv.Product)

                    .Where(pv => productVarIds.Contains(pv.Id))
                    .ToListAsync();
                return cartItems.Select(c => new CartGetVModel
                {
                    Quantity = c.Quantity,
                    ProVarId = c.ProVarId,
                    ProductName = productVariants.FirstOrDefault(pv => pv.Id == c.ProVarId)?.Product?.ProductName
                }).ToList();
            }
        }

        // Hợp nhất giỏ hàng từ cookies
        public async Task<ResponseResult> MergeCartFromCookiesAsync(HttpContext httpContext)
        {
            try
            {
                var userId = await GetCurrentUserIdAsync(httpContext);
                if (!userId.HasValue)
                {
                    return new ErrorResponseResult("User is not logged in.");
                }

                var cartItems = GetCartFromCookies(httpContext);
                if (!cartItems.Any())
                {
                    return new SuccessResponseResult(null, "No cart items in cookies to merge.");
                }

                foreach (var item in cartItems)
                {
                    var validationResult = await ValidateCartItemAsync(item);
                    if (validationResult != null)
                    {
                        continue; // Bỏ qua các mục không hợp lệ
                    }

                    var existingCartItem = await _context.Carts
                        .FirstOrDefaultAsync(c => c.UserId == userId && c.ProVarId == item.ProVarId && c.IsActive == true);
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
                return new SuccessResponseResult(cart, "Cart merged successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging cart from cookies: {Error}", ex.Message);
                return new ErrorResponseResult($"Error merging cart: {ex.Message}");
            }
        }
    }
}