using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TomsFurnitureBackend.Common.Models.Vnpay;
using TomsFurnitureBackend.Libraries;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Common.Contansts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TomsFurnitureBackend.Helpers.EmailContentHelpers;
using System.Linq;

namespace TomsFurnitureBackend.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly TomfurnitureContext _context;
        private readonly IEmailService _emailService;
        public VnPayService(IConfiguration configuration, TomfurnitureContext context, IEmailService emailService)
        {
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
        }
        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, int OrderId)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnpayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];

            // Thêm các tham s? c?n thi?t cho VNPAY
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", OrderId.ToString());

            // T?o URL thanh toán
            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;
        }

        public async Task<bool> ProcessVnPayCallbackAsync(IQueryCollection query)
        {
            var hashSecret = _configuration["Vnpay:HashSecret"];
            var vnpayLib = new VnpayLibrary();
            var paymentResult = vnpayLib.GetFullResponseData(query, hashSecret);

            var orderId = int.TryParse(paymentResult.OrderId, out var oid) ? oid : 0;
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Color)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Size)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProVar)
                        .ThenInclude(pv => pv.Material)
                .Include(o => o.OrderAdd)
                .Include(o => o.User)
                .Include(o => o.UserGuest)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return false;

            if (paymentResult.Success && paymentResult.VnPayResponseCode == "00")
            {
                order.PaymentStatus = PaymentStatus.Paid;
                order.IsPaid = true; // Đánh dấu đã thanh toán thành công qua VNPAY
                // Gửi email xác nhận thanh toán thành công qua VNPAY
                string? toEmail = null;
                if (order.User != null)
                {
                    toEmail = order.User.Email;
                }
                else if (order.UserGuest != null && !string.IsNullOrWhiteSpace(order.UserGuest.Email))
                {
                    toEmail = order.UserGuest.Email;
                }
                if (!string.IsNullOrWhiteSpace(toEmail))
                {
                    var (subject, body) = VNPayEmailContentHelper.BuildVnPaySuccessEmailContent(order);
                    await _emailService.SendEmailAsync(toEmail, subject, body);
                }
            }
            else if (paymentResult.VnPayResponseCode == "24")
            {
                order.PaymentStatus = PaymentStatus.Cancelled;
                order.IsPaid = false;
            }
            else
            {
                order.PaymentStatus = PaymentStatus.Failed;
                order.IsPaid = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
