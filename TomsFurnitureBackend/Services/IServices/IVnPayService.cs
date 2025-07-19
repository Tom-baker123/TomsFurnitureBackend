using Microsoft.AspNetCore.Http;
using TomsFurnitureBackend.Common.Models.Vnpay;
using TomsFurnitureBackend.Models;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
    }
}
