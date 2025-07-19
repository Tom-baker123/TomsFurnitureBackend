using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TomsFurnitureBackend.Common.Models.Vnpay;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly TomfurnitureContext _context;
        public PaymentController(IVnPayService vnPayService, TomfurnitureContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        [HttpPost("create-vnpay-url/{orderId}")]
        public async Task<IActionResult> CreatePaymentUrlVnpay(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("??n hàng không t?n t?i");
            }
            // Tạo model thông tin thanh toán đơn hàng
            var paymentInfo = new PaymentInformationModel
            {
                OrderType = order.OrderSta?.OrderStatusName ?? "default",
                Amount = (double)(order.Total ?? 0),
                OrderDescription = order.Note ?? "",
                Name = order.User?.Id.ToString() ?? "Khách"
            };
            // Tạo URL thanh toán VNPAY
            var url = _vnPayService.CreatePaymentUrl(paymentInfo, HttpContext);
            return Ok(new { PaymentUrl = url });
        }
    }
}
