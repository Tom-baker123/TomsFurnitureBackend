using System.Collections.Generic;
using System.Threading.Tasks;
using TomsFurnitureBackend.VModels;
using OA.Domain.Common.Models;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IUserGuestService
    {
        Task<ResponseResult> CreateAsync(UserGuestCreateVModel model);
        //Task<ResponseResult> UpdateAsync(UserGuestUpdateVModel model);
        //Task<List<UserGuestGetVModel>> GetAllAsync();
        //Task<UserGuestGetVModel?> GetByIdAsync(int id);
        //Task<ResponseResult> DeleteAsync(int id); // Xóa m?m
        //Task<UserGuestGetVModel?> FindByPhoneOrEmailAsync(string? phone, string? email);
    }
}
