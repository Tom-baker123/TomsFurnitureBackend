// Interface ??nh ngh?a c�c ch?c n?ng cho d?ch v? VNPAY
// B??c 1: T?o URL thanh to�n VNPAY
// B??c 2: X? l� callback tr? v? t? VNPAY
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
