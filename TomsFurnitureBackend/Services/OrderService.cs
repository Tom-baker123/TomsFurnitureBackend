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
using TomsFurnitureBackend.Common.Contansts;

namespace TomsFurnitureBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly TomfurnitureContext _context;
        private readonly IAuthService _authService;
        private readonly IVnPayService _vnPayService;
        public OrderService(TomfurnitureContext context, IAuthService authService, IVnPayService vnPayService)
        {
            _context = context;
            _authService = authService;
            _vnPayService = vnPayService;
        }

        // Hàm kiểm tra dữ liệu đầu vào cho đơn hàng
        // Trả về chuỗi rỗng nếu hợp lệ, trả về thông báo lỗi nếu có lỗi
        private static string ValidateOrder(OrderCreateVModel model)
        {
            // Kiểm tra danh sách chi tiết đơn hàng
            if (model.OrderDetails == null || !model.OrderDetails.Any())
                return "Order must have at least one order detail.";
            // Kiểm tra từng chi tiết đơn hàng
            foreach (var detail in model.OrderDetails)
            {
                if (detail.ProVarId <= 0)
                    return "Product variant ID must be greater than 0.";
                if (detail.Quantity <= 0)
                    return "Quantity must be greater than 0.";
                if (detail.Price < 0)
                    return "Price must be non-negative.";
            }
            // Kiểm tra phí vận chuyển
            if (model.ShippingPrice < 0)
                return "Shipping price must be non-negative.";
            // Kiểm tra phương thức thanh toán
            if (!model.PaymentMethodId.HasValue || model.PaymentMethodId <= 0)
                return "Payment method is required.";
            // Kiểm tra địa chỉ giao hàng
            if (!model.OrderAddId.HasValue || model.OrderAddId <= 0)
                return "Order address is required.";
            // Kiểm tra ghi chú (nếu có) không quá 500 ký tự
            if (!string.IsNullOrEmpty(model.Note) && model.Note.Length > 500)
                return "Note must be less than 500 characters.";
            // Kiểm tra UserGuestId cho đơn hàng khách vãng lai
            //if (model.UserGuestId.HasValue && model.UserGuestId <= 0)
            //    return "UserGuestId must be greater than 0 if provided.";
            return string.Empty;
        }

        public async Task<ResponseResult> ProcessPaymentAsync(OrderCreateVModel model, ClaimsPrincipal user, HttpContext httpContext)
        {
            // Bước 1: Kiểm tra trạng thái đăng nhập
            var authStatus = await _authService.GetAuthStatusAsync(user, httpContext);
            bool isAuthenticated = authStatus.IsAuthenticated;

            // Bước 2: Kiểm tra hợp lệ trạng thái đăng nhập/khách
            if (isAuthenticated)
            {
                // Nếu đã đăng nhập mà truyền UserGuestId thì báo lỗi
                if (model.UserGuestId.HasValue && model.UserGuestId.Value > 0)
                    return new ErrorResponseResult("User is authenticated, UserGuestId must not be provided.");
                // Nếu IsUserGuest là true thì báo lỗi
                if (model.GetType().GetProperty("IsUserGuest") != null && (bool)model.GetType().GetProperty("IsUserGuest").GetValue(model) == true)
                    return new ErrorResponseResult("User is authenticated, IsUserGuest must not be true.");
            }
            else
            {
                // Nếu chưa đăng nhập mà không có UserGuestId hoặc UserGuestId không hợp lệ thì báo lỗi
                if (!model.UserGuestId.HasValue || model.UserGuestId.Value <= 0)
                    return new ErrorResponseResult("User is not authenticated, a valid UserGuestId is required.");
                // Kiểm tra UserGuestId có tồn tại trong database không
                var existUserGuest = await _context.UserGuests
                    .FirstOrDefaultAsync(ug => ug.Id == model.UserGuestId);
                if (existUserGuest == null)
                    return new ErrorResponseResult("Userguest cannot process. Please try again!");
            }

            // Bước 3: Kiểm tra dữ liệu đầu vào đơn hàng
            var validation = ValidateOrder(model);
            if (!string.IsNullOrEmpty(validation))
                return new ErrorResponseResult(validation);

            // Bước 4: Chuyển dữ liệu từ ViewModel sang Entity
            var order = model.ToEntity();

            // Bước 5: Nếu là user đã đăng nhập thì gán UserId cho đơn hàng
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

            // Bước 6: Tính toán tổng tiền, giảm giá, phí vận chuyển
            decimal subTotal = order.Total ?? 0;
            decimal discount = 0;
            decimal finalShippingPrice = model.ShippingPrice;
            decimal Total = subTotal + finalShippingPrice - discount;

            // Bước 7: Áp dụng khuyến mãi nếu có
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
                        case 1: // Giảm giá theo phần trăm
                            discount = Math.Min(Total * promotion.DiscountValue / 100, promotion.MaximumDiscountAmount);
                            break;
                        case 2: // Giảm giá cố định
                            discount = Math.Min(promotion.DiscountValue, promotion.MaximumDiscountAmount);
                            break;
                        case 3: // Miễn phí vận chuyển
                            finalShippingPrice = 0;
                            discount = 0;
                            break;
                    }
                }
                promotion.CouponUsage--;
            }

            // Bước 8: Gán lại các giá trị tổng tiền, phí ship, giảm giá cho đơn hàng
            order.Total = Total - discount;
            order.ShippingPrice = finalShippingPrice;
            order.PriceDiscount = discount;

            // Bước 9: Gán trạng thái đơn hàng ban đầu
            order.OrderStaId = 1; // "Pending Confirmation"
            // order.IsPaid = true;
            // Xử lý trạng thái thanh toán theo phương thức
            if (order.PaymentMethodId == 1) // COD
            {
                order.PaymentStatus = PaymentStatus.Unpaid;
            }
            else // VNPAY hoặc các phương thức khác
            {
                order.PaymentStatus = PaymentStatus.Processing;
            }

            // Bước 10: Gán lại PromotionId nếu không có khuyến mãi
            if (model.PromotionId == 0)
                order.PromotionId = null;
            else
                order.PromotionId = model.PromotionId;

            // Bước 11: Thêm đơn hàng vào database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Bước 12: Tạo URL thanh toán VNPAY
            var paymentInfo = new TomsFurnitureBackend.Common.Models.Vnpay.PaymentInformationModel
            {
                OrderType = order.OrderSta?.OrderStatusName ?? "Pending Confirmation",
                Amount = (double)(order.Total ?? 0),
                OrderDescription = order.Note ?? "",
                Name = isAuthenticated ? (order.UserId?.ToString() ?? "User") : (order.UserGuestId?.ToString() ?? "User Guest")
            };
            var paymentUrl = _vnPayService.CreatePaymentUrl(paymentInfo, httpContext);

            // Bước 13: Trả về kết quả thành công
            return new SuccessResponseResult(
                new {
                    OrderId = order.Id,
                    Order = order.ToGetVModel(),
                    PaymentUrl = paymentUrl
                },
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
