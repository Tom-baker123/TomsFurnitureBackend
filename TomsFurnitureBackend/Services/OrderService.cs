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
using TomsFurnitureBackend.Helpers.EmailContentHelpers;

namespace TomsFurnitureBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly TomfurnitureContext _context;
        private readonly IAuthService _authService;
        private readonly IVnPayService _vnPayService;
        private readonly IEmailService _emailService;
        public OrderService(TomfurnitureContext context, IAuthService authService, IVnPayService vnPayService, IEmailService emailService)
        {
            _context = context;
            _authService = authService;
            _vnPayService = vnPayService;
            _emailService = emailService;
        }
       
        private string ValidateOrderWithDb(OrderCreateVModel model, bool isAuthenticated = false)
        {
            if (model.OrderDetails == null || !model.OrderDetails.Any())
                return "Đơn hàng phải có ít nhất một sản phẩm.";

            foreach (var detail in model.OrderDetails)
            {
                if (detail == null)
                    return "Thông tin sản phẩm không được để trống.";

                // Kiểm tra proVarId phải > 0 và tồn tại trong ProductVariant
                if (detail.ProVarId == null || detail.ProVarId <= 0 || !_context.ProductVariants.Any(pv => pv.Id == detail.ProVarId))
                    return $"Biến thể sản phẩm với ID {detail.ProVarId} không tồn tại.";

                // Kiểm tra số lượng
                if (detail.Quantity <= 0)
                    return "Số lượng sản phẩm phải lớn hơn 0.";
            }

            if (model.ShippingPrice < 0)
                return "Phí vận chuyển không được âm.";

            if (!model.PaymentMethodId.HasValue || model.PaymentMethodId <= 0)
                return "Phương thức thanh toán là bắt buộc.";

            // Kiểm tra phương thức thanh toán có tồn tại không
            if (!_context.PaymentMethods.Any(pm => pm.Id == model.PaymentMethodId))
                return $"Phương thức thanh toán với ID {model.PaymentMethodId} không tồn tại.";

            // Chỉ kiểm tra OrderAddId khi người dùng đăng nhập
            if (isAuthenticated)
            {
                if (!model.OrderAddId.HasValue || model.OrderAddId <= 0)
                    return "Địa chỉ giao hàng là bắt buộc.";

                // Kiểm tra địa chỉ giao hàng có tồn tại không
                if (!_context.OrderAddresses.Any(oa => oa.Id == model.OrderAddId))
                    return $"Địa chỉ giao hàng với ID {model.OrderAddId} không tồn tại.";
            }

            // Kiểm tra khuyến mãi nếu có
            if (model.PromotionId.HasValue && model.PromotionId > 0)
            {
                if (!_context.Promotions.Any(p => p.Id == model.PromotionId))
                    return $"Khuyến mãi với ID {model.PromotionId} không tồn tại.";
            }

            if (!string.IsNullOrEmpty(model.Note) && model.Note.Length > 500)
                return "Ghi chú phải ít hơn 500 ký tự.";

            return string.Empty;
        }

        // Sinh mã OrderCode duy nhất cho đơn hàng
        private async Task<string> GenerateUniqueOrderCodeAsync()
        {
            string code;
            var random = new Random();
            do
            {
                code = $"OD{DateTime.UtcNow:yyyyMMddHHmmss}{random.Next(1000, 9999)}";
            } while (await _context.Orders.AnyAsync(o => o.OrderCode == code));
            return code;
        }

        public async Task<ResponseResult> ProcessPaymentAsync(OrderCreateVModel model, ClaimsPrincipal user, HttpContext httpContext)
        {
            // Bước 1: Kiểm tra trạng thái đăng nhập
            var authStatus = await _authService.GetAuthStatusAsync(user, httpContext);
            bool isAuthenticated = authStatus.IsAuthenticated;

            // Nếu orderDetails null, empty, hoặc tất cả proVarId = 0 thì tự động lấy từ giỏ hàng
            bool shouldLoadCart = model.OrderDetails == null || !model.OrderDetails.Any() || model.OrderDetails.All(d => d == null || d.ProVarId == 0);
            if (shouldLoadCart)
            {
                if (isAuthenticated)
                {
                    var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                    {
                        var cartItems = await _context.Carts
                            .Where(c => c.UserId == userId)
                            .Include(c => c.ProVar)
                            .ToListAsync();
                        model.OrderDetails = cartItems.Select(c => new OrderDetailCreateVModel
                        {
                            ProVarId = c.ProVarId,
                            Quantity = c.Quantity,
                            Price = c.ProVar.DiscountedPrice ?? c.ProVar.OriginalPrice
                        }).ToList();
                    }
                }
                else
                {
                    var guestCartJson = httpContext.Request.Cookies["GuestCart"];
                    if (!string.IsNullOrEmpty(guestCartJson))
                    {
                        try
                        {
                            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<OrderDetailCreateVModel>>(guestCartJson);
                            if (cartItems != null)
                                model.OrderDetails = cartItems;
                        }
                        catch { /* ignore parse error */ }
                    }
                }
            }

            // Bước 2: Kiểm tra hợp lệ trạng thái đăng nhập/khách
            if (isAuthenticated)
            {
                // Nếu đã đăng nhập mà truyền UserGuestId thì báo lỗi
                if (model.UserGuestId.HasValue && model.UserGuestId.Value > 0)
                    return new ErrorResponseResult("Người dùng đã đăng nhập, không được cung cấp UserGuestId.");

                // Nếu IsUserGuest là true thì báo lỗi
                if (model.GetType().GetProperty("IsUserGuest") != null && (bool)model.GetType().GetProperty("IsUserGuest").GetValue(model) == true)
                    return new ErrorResponseResult("Người dùng đã đăng nhập, IsUserGuest không được là true.");
            }
            else
            {
                // Nếu chưa đăng nhập mà không có UserGuestId hoặc UserGuestId không hợp lệ thì báo lỗi
                if (!model.UserGuestId.HasValue || model.UserGuestId.Value <= 0)
                    return new ErrorResponseResult("Người dùng chưa đăng nhập, cần có UserGuestId hợp lệ.");

                // Kiểm tra UserGuestId có tồn tại trong database không
                var existUserGuest = await _context.UserGuests
                    .FirstOrDefaultAsync(ug => ug.Id == model.UserGuestId);
                if (existUserGuest == null)
                    return new ErrorResponseResult("Thông tin khách vãng lai không hợp lệ. Vui lòng thử lại!");
            }

            // Bước 3: Kiểm tra dữ liệu đầu vào đơn hàng
            var validation = ValidateOrderWithDb(model, isAuthenticated);
            if (!string.IsNullOrEmpty(validation))
                return new ErrorResponseResult(validation);

            // Bước 4: Chuyển dữ liệu từ ViewModel sang Entity
            var order = model.ToEntity();

            // Bổ sung: Sinh OrderCode duy nhất và gán OrderDate
            order.OrderDate = DateTime.UtcNow;
            order.OrderCode = await GenerateUniqueOrderCodeAsync();

            // Bước 5: Nếu là user đã đăng nhập thì gán UserId cho đơn hàng
            int? userIdForCart = null;
            if (isAuthenticated)
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    order.UserId = userId;
                    order.IsUserGuest = false;
                    order.UserGuestId = null;
                    userIdForCart = userId;
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
                    return new ErrorResponseResult("Khuyến mãi không hợp lệ hoặc đã hết hạn.");

                if (Total < promotion.OrderMinimum)
                    return new ErrorResponseResult($"Tổng đơn hàng phải ít nhất {promotion.OrderMinimum:N0} VNĐ để áp dụng khuyến mãi này.");

                if (DateTime.UtcNow < promotion.StartDate || DateTime.UtcNow > promotion.EndDate)
                    return new ErrorResponseResult("Khuyến mãi không có hiệu lực tại thời điểm này.");

                if (promotion.CouponUsage <= 0)
                    return new ErrorResponseResult("Đã đạt giới hạn sử dụng khuyến mãi.");

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
                        //case 3: // Miễn phí vận chuyển
                        //    finalShippingPrice = 0;
                        //    discount = 0;
                        //    break;
                    }
                }
                // promotion.CouponUsage--;
            }

            // Bước 8: Gán lại các giá trị tổng tiền, phí ship, giảm giá cho đơn hàng
            order.Total = Total - discount;
            order.ShippingPrice = finalShippingPrice;
            order.PriceDiscount = discount;

            // Bước 9: Gán trạng thái đơn hàng ban đầu
            order.OrderStaId = 1; // "Pending Confirmation"
            // Xử lý trạng thái thanh toán theo phương thức
            if (order.PaymentMethodId == 1) // COD
            {
                order.PaymentStatus = PaymentStatus.Unpaid;
                order.IsPaid = false; // Đánh dấu đã thanh toán thành công cho COD
            }
            else // VNPAY hoặc các phương thức khác
            {
                order.PaymentStatus = PaymentStatus.Processing;
                order.IsPaid = false; // Chưa thanh toán với VNPAY hoặc phương thức khác
            }

            // Bước 10: Gán lại PromotionId nếu không có khuyến mãi
            if (model.PromotionId == 0)
                order.PromotionId = null;
            else
                order.PromotionId = model.PromotionId;

            // Bước 11: Thêm đơn hàng vào database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // --- XÓA GIỎ HÀNG SAU KHI ĐẶT HÀNG ---
            if (isAuthenticated && userIdForCart.HasValue)
            {
                // Xóa các bản ghi cart trong DB cho user đăng nhập
                var userCartItems = _context.Carts.Where(c => c.UserId == userIdForCart.Value);
                _context.Carts.RemoveRange(userCartItems);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Xóa cookie GuestCart cho khách vãng lai
                httpContext.Response.Cookies.Delete("GuestCart", new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Path = "/",
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                    HttpOnly = true
                });
            }
            // --- END XÓA GIỎ HÀNG ---

            // Bước 12: Tạo URL thanh toán VNPAY
            string paymentUrl = string.Empty;
            // Load lại order từ DB kèm navigation property cần thiết
            var orderWithStatus = await _context.Orders
                .Include(o => o.OrderSta)
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderAdd)
                .Include(o => o.User)
                .Include(o => o.UserGuest)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (order.PaymentMethodId == 2)
            {
                var paymentInfo = new TomsFurnitureBackend.Common.Models.Vnpay.PaymentInformationModel
                {
                    OrderType = orderWithStatus?.OrderSta?.OrderStatusName ?? "Pending Confirmation",
                    Amount = (double)(orderWithStatus?.Total ?? 0),
                    OrderDescription = orderWithStatus?.Note ?? "",
                    Name = isAuthenticated ? (orderWithStatus?.UserId?.ToString() ?? "User") : (orderWithStatus?.UserGuestId?.ToString() ?? "User Guest")
                };
                paymentUrl = _vnPayService.CreatePaymentUrl(paymentInfo, httpContext, order.Id);
            }

            // Bước gửi email xác nhận thanh toán thành công (chỉ gửi khi là COD)
            if (orderWithStatus?.PaymentMethodId == 1) // Chỉ gửi mail nếu là COD
            {
                string? toEmail = null;
                if (orderWithStatus.User != null)
                {
                    toEmail = orderWithStatus.User.Email;
                }
                else if (orderWithStatus.UserGuest != null && !string.IsNullOrWhiteSpace(orderWithStatus.UserGuest.Email))
                {
                    toEmail = orderWithStatus.UserGuest.Email;
                }
                if (!string.IsNullOrWhiteSpace(toEmail))
                {
                    var (subject, body) = OrderEmailContentHelper.BuildOrderSuccessEmailContent(orderWithStatus);
                    await _emailService.SendEmailAsync(toEmail, subject, body);
                }
            }

            // Bước 13: Trả về kết quả thành công
            return new SuccessResponseResult(
                new {
                    OrderId = order.Id,
                    Order = orderWithStatus?.ToGetVModel(),
                    PaymentUrl = paymentUrl // Trả về URL thanh toán nếu có
                },
                "Xử lý thanh toán thành công. Đơn hàng đang chờ xác nhận."
            );
        }

        public async Task<OrderGetVModel?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderSta)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Color)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.ProductVariantImages)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Size)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Material)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                .Include(o => o.User)
                .Include(o => o.UserGuest)
                .FirstOrDefaultAsync(o => o.Id == id);
            return order?.ToGetVModel();
        }

        public async Task<List<OrderGetVModel>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderSta)
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
                .Include(o => o.User)
                .Include(o => o.UserGuest)
                .ToListAsync();
            return orders.Select(o => o.ToGetVModel()).ToList();
        }

        public async Task<List<OrderGetVModel>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderSta)
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
                .Include(o => o.User)
                .Include(o => o.UserGuest)
                .ToListAsync();
            return orders.Select(o => o.ToGetVModel()).ToList();
        }

        // Cập nhật trạng thái đơn hàng chỉ cho phép tiến tới, không cho phép lùi lại trạng thái trước đó và không được nhảy trạng thái
        public async Task<ResponseResult> UpdateOrderStatusAsync(int orderId, int newStatusId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderSta)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                .Include(o => o.User)
                .Include(o => o.UserGuest)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return new ErrorResponseResult("Không tìm thấy đơn hàng.");

            if (!order.OrderStaId.HasValue)
                return new ErrorResponseResult("Đơn hàng không có trạng thái hiện tại.");

            // Không cho phép cập nhật ngược trạng thái hoặc nhảy trạng thái
            if (newStatusId != order.OrderStaId.Value + 1)
            {
                return new ErrorResponseResult("Trạng thái đơn hàng phải được cập nhật theo thứ tự. Không thể bỏ qua hoặc quay lại trạng thái trước đó.");
            }

            // Kiểm tra trạng thái mới có tồn tại không
            var newStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Id == newStatusId);
            if (newStatus == null)
                return new ErrorResponseResult("Trạng thái đích không tồn tại.");

            // Nếu cập nhật sang trạng thái Confirmed (OrderStaId = 2) và trạng thái cũ < 2
            if (newStatusId == 2 && order.OrderStaId.Value < 2)
            {
                // Trừ số lượng tồn kho của các ProductVariant
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.ProVar != null)
                    {
                        if (detail.ProVar.StockQty < detail.Quantity)
                        {
                            return new ErrorResponseResult($"Không đủ tồn kho cho biến thể sản phẩm ID {detail.ProVar.Id}.");
                        }
                        detail.ProVar.StockQty -= detail.Quantity;
                    }
                }

                // Trừ số lượng sử dụng mã giảm giá nếu có
                if (order.PromotionId.HasValue)
                {
                    var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == order.PromotionId);
                    if (promotion != null && promotion.CouponUsage > 0)
                    {
                        promotion.CouponUsage--;
                    }
                }
            }

            order.OrderStaId = newStatusId;
            order.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Gửi email thông báo trạng thái mới cho user/guest
            string? toEmail = null;
            if (order.User != null)
                toEmail = order.User.Email;
            else if (order.UserGuest != null && !string.IsNullOrWhiteSpace(order.UserGuest.Email))
                toEmail = order.UserGuest.Email;

            if (!string.IsNullOrWhiteSpace(toEmail))
            {
                var (subject, body) = OrderEmailContentHelper.BuildOrderStatusUpdateEmailContent(order, newStatus.OrderStatusName);
                await _emailService.SendEmailAsync(toEmail, subject, body);
            }

            return new SuccessResponseResult(order.ToGetVModel(), "Cập nhật trạng thái đơn hàng thành công.");
        }

        public async Task<ResponseResult> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return new ErrorResponseResult("Không tìm thấy đơn hàng.");

            if (order.OrderStaId != 1) // 1 = Pending Confirmation
                return new ErrorResponseResult("Chỉ được hủy đơn khi đang ở trạng thái chờ xác nhận.");

            order.OrderStaId = 6; // 6 = Cancelled
            order.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // (Có thể gửi email thông báo hủy đơn ở đây nếu muốn)

            return new SuccessResponseResult(order.ToGetVModel(), "Hủy đơn hàng thành công.");
        }
    }
}
