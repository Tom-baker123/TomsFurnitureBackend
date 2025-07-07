using OA.Domain.Common.Models;
using System.Threading.Tasks;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.CartVModel;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface ICartService
    {
        Task<ResponseResult> AddToCartAsync(CartCreateVModel model, HttpContext httpContext);
        Task<ResponseResult> UpdateCartAsync(CartUpdateVModel model, HttpContext httpContext);
        Task<ResponseResult> RemoveFromCartAsync(int id, HttpContext httpContext);
        Task<List<CartGetVModel>> GetCartAsync(HttpContext httpContext);
        Task<ResponseResult> MergeCartFromCookiesAsync(HttpContext httpContext);
        Task<ResponseResult> MergeCartFromCookiesAsync(HttpContext httpContext, int? userId);
    }
}