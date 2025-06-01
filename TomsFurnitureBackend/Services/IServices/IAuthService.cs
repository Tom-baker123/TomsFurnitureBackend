using OA.Domain.Common.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseResult> LoginAsync(LoginVModel model, HttpContext httpContext);
        Task<ResponseResult> LogoutAsync(HttpContext httpContext);
        Task<AuthStatusVModel> GetAuthStatusAsync(ClaimsPrincipal user, HttpContext httpContext);
        Task<ResponseResult> RegisterAsync(RegisterVModel model);
        Task<ResponseResult> VerifyOtpAsync(ConfirmOtpVModel model);
        Task<ResponseResult> ResendOtpAsync(string email);
    }
}