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

        // B??c 1: Nh?n orderId, l?y th�ng tin ??n h�ng v� t?o URL thanh to�n VNPAY
        [HttpPost("create-vnpay-url/{orderId}")]
        public async Task<IActionResult> CreatePaymentUrlVnpay(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("??n h�ng kh�ng t?n t?i");
            }
            // T?o model th�ng tin thanh to�n t? ??n h�ng
            var paymentInfo = new PaymentInformationModel
            {
                OrderType = order.OrderSta?.OrderStatusName ?? "default",
                Amount = (double)(order.Total ?? 0),
                OrderDescription = order.Note ?? "",
                Name = order.User?.Id.ToString() ?? "Kh�ch"
            };
            // T?o URL thanh to�n VNPAY
            var url = _vnPayService.CreatePaymentUrl(paymentInfo, HttpContext);
            return Ok(new { PaymentUrl = url });
        }

        // B??c 2: Nh?n callback t? VNPAY v� tr? v? k?t qu? thanh to�n
        [HttpGet("callback-vnpay")]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            return Ok(response);
        }
    }
}
