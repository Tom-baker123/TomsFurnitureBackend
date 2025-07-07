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
                _logger.LogInformation("GuestCart cookie not found.");
                return new List<CartCreateVModel>();
            }
            try
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartCreateVModel>>(cartJson) ?? new List<CartCreateVModel>();
                _logger.LogInformation("Successfully read GuestCart cookie with {Count} items: {Content}", cartItems.Count, cartJson);
                return cartItems;
            }
            catch
            {
                _logger.LogWarning("Unable to decode cart cookie.");
                return new List<CartCreateVModel>();
            }
        }

        private void SaveCartToCookies(HttpContext httpContext, List<CartCreateVModel> cartItems)
        {
            var cartJson = JsonConvert.SerializeObject(cartItems);
            _logger.LogInformation("Saving GuestCart cookie with content: {Content}", cartJson);
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
                // Biến thể sản phẩm với ID không tồn tại
                return new ErrorResponseResult($"Product variant with ID {model.ProVarId} does not exist.");
            }
            if (model.Quantity <= 0)
            {
                // Số lượng phải lớn hơn 0
                return new ErrorResponseResult("Quantity must be greater than 0.");
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
                        var cartEntity = CartMapping.ToEntity(model, userId); // Sử dụng CartMapping
                        _context.Carts.Add(cartEntity);
                    }
                    await _context.SaveChangesAsync();
                    var cart = await GetCartAsync(httpContext);
                    // Thêm thành công vào giỏ hàng
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
                    // Thêm thành công vào giỏ hàng (cookie)
                    return new SuccessResponseResult(cartItems.Select(c => CartMapping.ToGetVModel(new Cart { Quantity = c.Quantity, ProVarId = c.ProVarId })).ToList(), "Added to cart successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart: {Error}", ex.Message);
                // Lỗi khi thêm vào giỏ hàng
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
                        // Không tìm thấy mục giỏ hàng
                        return new ErrorResponseResult($"Cart item with ID {model.Id} not found.");
                    }
                    CartMapping.UpdateEntity(cartItem, model); // Sử dụng CartMapping
                    await _context.SaveChangesAsync();
                    var cart = await GetCartAsync(httpContext);
                    // Cập nhật giỏ hàng thành công
                    return new SuccessResponseResult(cart, "Cart updated successfully.");
                }
                else
                {
                    var cartItems = GetCartFromCookies(httpContext);
                    var existingItem = cartItems.FirstOrDefault(c => c.ProVarId == model.ProVarId);
                    if (existingItem == null)
                    {
                        // Không tìm thấy mục giỏ hàng (cookie)
                        return new ErrorResponseResult($"Cart item with ProVarId {model.ProVarId} not found.");
                    }
                    existingItem.Quantity = model.Quantity;
                    SaveCartToCookies(httpContext, cartItems);
                    // Cập nhật giỏ hàng thành công (cookie)
                    return new SuccessResponseResult(cartItems.Select(c => CartMapping.ToGetVModel(new Cart { Quantity = c.Quantity, ProVarId = c.ProVarId })).ToList(), "Cart updated successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart: {Error}", ex.Message);
                // Lỗi khi cập nhật giỏ hàng
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
                        // Không tìm thấy mục giỏ hàng
                        return new ErrorResponseResult($"Cart item with ID {id} not found.");
                    }
                    cartItem.IsActive = false;
                    cartItem.UpdatedDate = DateTime.UtcNow;
                    cartItem.UpdatedBy = userId.ToString();
                    await _context.SaveChangesAsync();
                    var cart = await GetCartAsync(httpContext);
                    // Xóa thành công khỏi giỏ hàng
                    return new SuccessResponseResult(cart, "Item removed from cart successfully.");
                }
                else
                {
                    var cartItems = GetCartFromCookies(httpContext);
                    var existingItem = cartItems.FirstOrDefault(c => c.ProVarId == id);
                    if (existingItem == null)
                    {
                        // Không tìm thấy mục giỏ hàng (cookie)
                        return new ErrorResponseResult($"Cart item with ProVarId {id} not found.");
                    }
                    cartItems.Remove(existingItem);
                    SaveCartToCookies(httpContext, cartItems);
                    // Xóa thành công khỏi giỏ hàng (cookie)
                    return new SuccessResponseResult(cartItems.Select(c => CartMapping.ToGetVModel(new Cart { Quantity = c.Quantity, ProVarId = c.ProVarId })).ToList(), "Item removed from cart successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from cart: {Error}", ex.Message);
                // Lỗi khi xóa khỏi giỏ hàng
                return new ErrorResponseResult($"Error removing from cart: {ex.Message}");
            }
        }

        public async Task<List<CartGetVModel>> GetCartAsync(HttpContext httpContext)
        {
            var userId = await GetCurrentUserIdAsync(httpContext);
            if (userId.HasValue)
            {
                var cartItems = await _context.Carts
                    .Include(c => c.ProVar)
                        .ThenInclude(pv => pv.Product)
                    .Include(c => c.ProVar)
                        .ThenInclude(pv => pv.Color)
                    .Include(c => c.ProVar)
                        .ThenInclude(pv => pv.Size)
                    .Include(c => c.ProVar)
                        .ThenInclude(pv => pv.Material)
                    .Include(c => c.ProVar)
                        .ThenInclude(pv => pv.Unit)
                    .Where(c => c.UserId == userId && c.IsActive == true)
                    .ToListAsync();
                return cartItems.Select(c => CartMapping.ToGetVModel(c)).ToList();
            }
            else
            {
                var cartItems = GetCartFromCookies(httpContext);
                var productVarIds = cartItems.Select(c => c.ProVarId).ToList();
                var productVariants = await _context.ProductVariants
                    .Include(pv => pv.Product)
                    .Include(pv => pv.Color)
                    .Include(pv => pv.Size)
                    .Include(pv => pv.Material)
                    .Include(pv => pv.Unit)
                    .Where(pv => productVarIds.Contains(pv.Id))
                    .ToListAsync();
                return cartItems.Select(c => {
                    var pv = productVariants.FirstOrDefault(pv => pv.Id == c.ProVarId);
                    var cartEntity = new Cart { Quantity = c.Quantity, ProVarId = c.ProVarId, ProVar = pv };
                    return CartMapping.ToGetVModel(cartEntity);
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
                    // User chưa đăng nhập
                    return new ErrorResponseResult("User is not logged in.");
                }

                var cartItems = GetCartFromCookies(httpContext);
                if (!cartItems.Any())
                {
                    // Không có mục nào trong cookie để hợp nhất
                    return new SuccessResponseResult(null, "No cart items in cookies to merge.");
                }

                foreach (var item in cartItems)
                {
                    var validationResult = await ValidateCartItemAsync(item);
                    if (validationResult != null)
                    {
                        // Bỏ qua các mục không hợp lệ
                        continue;
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
                        var cartEntity = CartMapping.ToEntity(item, userId); // Sử dụng CartMapping
                        _context.Carts.Add(cartEntity);
                    }
                }

                await _context.SaveChangesAsync();
                httpContext.Response.Cookies.Delete("GuestCart");
                var cart = await GetCartAsync(httpContext);
                // Hợp nhất giỏ hàng thành công
                return new SuccessResponseResult(cart, "Cart merged successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging cart from cookies: {Error}", ex.Message);
                // Lỗi khi hợp nhất giỏ hàng
                return new ErrorResponseResult($"Error merging cart: {ex.Message}");
            }
        }
    }
}