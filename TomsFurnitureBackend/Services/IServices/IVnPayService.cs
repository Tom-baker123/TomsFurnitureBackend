using Microsoft.AspNetCore.Http;
using TomsFurnitureBackend.Common.Models.Vnpay;
using TomsFurnitureBackend.Models;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, int OrderId);
        Task<bool> ProcessVnPayCallbackAsync(IQueryCollection query);
    }
}
