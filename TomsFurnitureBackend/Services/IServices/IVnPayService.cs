// Interface ??nh ngh?a các ch?c n?ng cho d?ch v? VNPAY
// B??c 1: T?o URL thanh toán VNPAY
// B??c 2: X? lý callback tr? v? t? VNPAY
using Microsoft.AspNetCore.Http;
using TomsFurnitureBackend.Common.Models.Vnpay;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
