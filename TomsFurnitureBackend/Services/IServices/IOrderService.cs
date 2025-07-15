using OA.Domain.Common.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.OrderVModel;
using Microsoft.AspNetCore.Http;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IOrderService
    {
        Task<ResponseResult> ProcessPaymentAsync(OrderCreateVModel model, ClaimsPrincipal user, HttpContext httpContext);
        Task<OrderGetVModel?> GetOrderByIdAsync(int id);
        Task<List<OrderGetVModel>> GetOrdersByUserAsync(int userId);
        Task<List<OrderGetVModel>> GetAllOrdersAsync();
    }
}
