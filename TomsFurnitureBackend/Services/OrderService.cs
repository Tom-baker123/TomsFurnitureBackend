using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.OrderVModel;

namespace TomsFurnitureBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly TomfurnitureContext _context;
        private readonly IAuthService _authService;

        public OrderService(TomfurnitureContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        private static string ValidateOrder(OrderCreateVModel model)
        {
            if (model.OrderDetails == null || !model.OrderDetails.Any())
                return "Order must have at least one order detail.";
            if (model.ShippingPrice < 0)
                return "Shipping price must be non-negative.";
            if (!model.UserGuestId.HasValue)
                return "UserGuestId is required for guest order.";
            return string.Empty;
        }

        private static string ValidateOrderUpdate(OrderUpdateVModel model)
        {
            if (model.Id <= 0)
                return "Order Id is invalid.";
            return ValidateOrder(model);
        }

        public async Task<ResponseResult> ProcessPaymentAsync(OrderCreateVModel model, ClaimsPrincipal user, HttpContext httpContext)
        {
            var authStatus = await _authService.GetAuthStatusAsync(user, httpContext);
            bool isAuthenticated = authStatus.IsAuthenticated;

            // Validate login/guest status
            if (isAuthenticated)
            {
                // If authenticated and UserGuestId is provided (> 0), return error
                if (model.UserGuestId.HasValue && model.UserGuestId.Value > 0)
                    return new ErrorResponseResult("User is authenticated, UserGuestId must not be provided.");
                // If IsUserGuest is true, return error
                if (model.GetType().GetProperty("IsUserGuest") != null && (bool)model.GetType().GetProperty("IsUserGuest").GetValue(model) == true)
                    return new ErrorResponseResult("User is authenticated, IsUserGuest must not be true.");
            }
            else
            {
                // If not authenticated and UserGuestId is missing or invalid, return error
                if (!model.UserGuestId.HasValue || model.UserGuestId.Value <= 0)
                    return new ErrorResponseResult("User is not authenticated, a valid UserGuestId is required.");
                
                // Kiểm tra xem UserGuestId có tồn tại trong cơ sở dữ liệu không
                var existUserGuest = await _context.UserGuests
                    .FirstOrDefaultAsync(ug => ug.Id == model.UserGuestId);

                if (existUserGuest == null)
                    return new ErrorResponseResult("Userguest cannot process. Please try again!");

            }

            var validation = ValidateOrder(model);
            if (!string.IsNullOrEmpty(validation))
                return new ErrorResponseResult(validation);

            var order = model.ToEntity();

            if (isAuthenticated)
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    order.UserId = userId;
                    order.IsUserGuest = false;
                    order.UserGuestId = null;
                }
            }

            decimal subTotal = order.Total ?? 0;
            decimal discount = 0;
            decimal finalShippingPrice = model.ShippingPrice;
            
            // Tính tổng tiền + phí ship
            decimal Total = subTotal + finalShippingPrice - discount;

            if (order.PromotionId.HasValue)
            {
                var promotion = await _context.Promotions
                    .Include(pt => pt.PromotionType)
                    .FirstOrDefaultAsync(p => p.Id == order.PromotionId && p.IsActive == true);

                if (promotion == null)
                    return new ErrorResponseResult("Invalid or inactive promotion.");

                if (Total < promotion.OrderMinimum)
                    return new ErrorResponseResult($"Order total must be at least {promotion.OrderMinimum} to apply this promotion.");

                if (DateTime.UtcNow < promotion.StartDate || DateTime.UtcNow > promotion.EndDate)
                    return new ErrorResponseResult("Promotion is not valid at this time.");

                if (promotion.CouponUsage <= 0)
                    return new ErrorResponseResult("Promotion usage limit reached.");

                if (promotion.PromotionType != null)
                {
                    switch (promotion.PromotionType.Id)
                    {
                        case 1: // Percentage Discount
                            discount = Math.Min(Total * promotion.DiscountValue / 100, promotion.MaximumDiscountAmount);
                            break;
                        case 2: // Fixed Amount Discount
                            discount = Math.Min(promotion.DiscountValue, promotion.MaximumDiscountAmount);
                            break;
                        case 3: // Free Shipping
                            finalShippingPrice = 0;
                            discount = 0;
                            break;
                    }
                }
                promotion.CouponUsage--;
            }

            order.Total = Total - discount;
            order.ShippingPrice = finalShippingPrice;
            order.PriceDiscount = discount;

            order.OrderStaId = 1; // "Pending Confirmation"
            order.IsPaid = true;
            order.PaymentStatus = true;

            if (model.PromotionId == 0)
                order.PromotionId = null;
            else
                order.PromotionId = model.PromotionId;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new SuccessResponseResult(
                order.Id, 
                order.ToGetVModel(),
                "Payment processed successfully. Order is pending confirmation."
            );
        }

        public async Task<OrderGetVModel?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProVar)
                .FirstOrDefaultAsync(o => o.Id == id);
            return order?.ToGetVModel();
        }

        public async Task<List<OrderGetVModel>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProVar)
                .ToListAsync();
            return orders.Select(o => o.ToGetVModel()).ToList();
        }

        public async Task<List<OrderGetVModel>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.ProductVariantImages)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Color)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Size)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Material)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Unit)
                .ToListAsync();
            return orders.Select(o => o.ToGetVModel()).ToList();
        }
    }
}
