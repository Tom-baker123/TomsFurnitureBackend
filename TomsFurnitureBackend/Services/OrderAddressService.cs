using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TomsFurnitureBackend.Services.Interfaces;

namespace TomsFurnitureBackend.Services
{
    public class OrderAddressService : IOrderAddressService
    {
        private readonly TomfurnitureContext _context;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderAddressService(TomfurnitureContext context, IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        public static string Validate(OrderAddressCreateVModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Recipient))
                return "Tên người nhận là bắt buộc.";
            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                return "Số điện thoại là bắt buộc.";
            if (string.IsNullOrWhiteSpace(model.AddressDetailRecipient))
                return "Địa chỉ chi tiết là bắt buộc.";
            if (string.IsNullOrWhiteSpace(model.City))
                return "Thành phố là bắt buộc.";
            if (string.IsNullOrWhiteSpace(model.District))
                return "Quận/Huyện là bắt buộc.";
            if (string.IsNullOrWhiteSpace(model.Ward))
                return "Phường/Xã là bắt buộc.";
            if (model.Recipient.Length > 100)
                return "Tên người nhận phải ít hơn 100 ký tự.";
            if (model.PhoneNumber.Length > 20)
                return "Số điện thoại phải ít hơn 20 ký tự.";
            if (model.AddressDetailRecipient.Length > 200)
                return "Địa chỉ chi tiết phải ít hơn 200 ký tự.";
            return string.Empty;
        }

        private async Task<string> ValidateWithForeignKeysAsync(OrderAddressCreateVModel model)
        {
            // Kiểm tra khóa ngoại UserId
            if (model.UserId.HasValue && model.UserId > 0)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId);
                if (!userExists)
                    return $"Người dùng với ID {model.UserId} không tồn tại.";
            }
            return string.Empty;
        }

        public async Task<List<OrderAddressGetVModel>> GetAllAsync(int? userId = null, bool? isDeafaultAddress = null)
        {
            // Nếu không truyền userId, tự động lấy từ xác thực
            if (!userId.HasValue)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var user = httpContext?.User;
                if (user != null)
                {
                    var authStatus = await _authService.GetAuthStatusAsync(user, httpContext);
                    if (authStatus.IsAuthenticated)
                    {
                        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int uid))
                        {
                            userId = uid;
                        }
                    }
                }
            }
            var query = _context.OrderAddresses.AsQueryable();
            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);
            if (isDeafaultAddress.HasValue && isDeafaultAddress.Value)
            {
                return new List<OrderAddressGetVModel> {
                    await query.Where(x => x.IsDeafaultAddress).Select(x => x.ToGetVModel()).FirstOrDefaultAsync()
                };
            }
            var list = await query.ToListAsync();
            return list.Select(x => x.ToGetVModel()).ToList();
        }

        public async Task<OrderAddressGetVModel?> GetByIdAsync(int id)
        {
            var entity = await _context.OrderAddresses.FindAsync(id);
            return entity?.ToGetVModel();
        }

        public async Task<ResponseResult> CreateAsync(OrderAddressCreateVModel model)
        {
            var validation = Validate(model);
            if (!string.IsNullOrEmpty(validation))
                return new ErrorResponseResult(validation);

            var foreignKeyValidation = await ValidateWithForeignKeysAsync(model);
            if (!string.IsNullOrEmpty(foreignKeyValidation))
                return new ErrorResponseResult(foreignKeyValidation);

            // Lấy danh sách địa chỉ của user
            var userAddresses = _context.OrderAddresses.Where(x => x.UserId == model.UserId);
            var addressCount = await userAddresses.CountAsync();

            var entity = model.ToEntity();

            if (addressCount == 0)
            {
                // Nếu là địa chỉ đầu tiên, đặt mặc định
                entity.IsDeafaultAddress = true;
            }
            else if (model.IsDeafaultAddress)
            {
                // Nếu thêm mới và muốn đặt mặc định, tắt mặc định các địa chỉ khác
                foreach (var addr in await userAddresses.ToListAsync())
                {
                    addr.IsDeafaultAddress = false;
                }
                entity.IsDeafaultAddress = true;
            }
            else
            {
                entity.IsDeafaultAddress = false;
            }

            _context.OrderAddresses.Add(entity);
            await _context.SaveChangesAsync();
            return new SuccessResponseResult(entity.ToGetVModel(), "Tạo địa chỉ giao hàng thành công.");
        }

        public async Task<ResponseResult> UpdateAsync(OrderAddressUpdateVModel model)
        {
            var validation = Validate(model);
            if (!string.IsNullOrEmpty(validation))
                return new ErrorResponseResult(validation);

            var foreignKeyValidation = await ValidateWithForeignKeysAsync(model);
            if (!string.IsNullOrEmpty(foreignKeyValidation))
                return new ErrorResponseResult(foreignKeyValidation);

            var entity = await _context.OrderAddresses.FindAsync(model.Id);
            if (entity == null)
                return new ErrorResponseResult("Không tìm thấy địa chỉ giao hàng.");

            // Nếu cập nhật và muốn đặt mặc định
            if (model.IsDeafaultAddress)
            {
                var userAddresses = _context.OrderAddresses.Where(x => x.UserId == model.UserId && x.Id != model.Id);
                foreach (var addr in await userAddresses.ToListAsync())
                {
                    addr.IsDeafaultAddress = false;
                }
                entity.IsDeafaultAddress = true;
            }
            else
            {
                entity.IsDeafaultAddress = model.IsDeafaultAddress;
            }

            entity.UpdateEntity(model);
            await _context.SaveChangesAsync();
            return new SuccessResponseResult(entity.ToGetVModel(), "Cập nhật địa chỉ giao hàng thành công.");
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            var entity = await _context.OrderAddresses.FindAsync(id);
            if (entity == null)
                return new ErrorResponseResult("Không tìm thấy địa chỉ giao hàng.");

            // Kiểm tra xem có đơn hàng nào đang sử dụng địa chỉ này không
            var ordersUsingAddress = await _context.Orders.AnyAsync(o => o.OrderAddId == id);
            if (ordersUsingAddress)
                return new ErrorResponseResult("Không thể xóa địa chỉ này vì đang được sử dụng bởi một hoặc nhiều đơn hàng.");

            _context.OrderAddresses.Remove(entity);
            await _context.SaveChangesAsync();
            return new SuccessResponseResult(null, "Xóa địa chỉ giao hàng thành công.");
        }
    }
}