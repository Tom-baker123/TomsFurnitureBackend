using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TomsFurnitureBackend.Common.Models.Vnpay;
using TomsFurnitureBackend.Libraries;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Common.Contansts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly TomfurnitureContext _context;
        public VnPayService(IConfiguration configuration, TomfurnitureContext context)
        {
            _configuration = configuration;
            _context = context;
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
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return false;

            if (paymentResult.Success && paymentResult.VnPayResponseCode == "00")
                order.PaymentStatus = PaymentStatus.Paid;
            else if (paymentResult.VnPayResponseCode == "24")
                order.PaymentStatus = PaymentStatus.Cancelled;
            else
                order.PaymentStatus = PaymentStatus.Failed;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
