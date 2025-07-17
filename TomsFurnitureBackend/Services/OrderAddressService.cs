using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class OrderAddressService : IOrderAddressService
    {
        private readonly TomfurnitureContext _context;

        public OrderAddressService(TomfurnitureContext context)
        {
            _context = context;
        }

        // Validation dùng chung cho thêm/sửa
        public static string Validate(OrderAddressCreateVModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Recipient))
                return "Recipient is required.";
            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                return "Phone number is required.";
            if (string.IsNullOrWhiteSpace(model.AddressDetailRecipient))
                return "Address detail is required.";
            if (string.IsNullOrWhiteSpace(model.City))
                return "City is required.";
            if (string.IsNullOrWhiteSpace(model.District))
                return "District is required.";
            if (string.IsNullOrWhiteSpace(model.Ward))
                return "Ward is required.";
            if (model.Recipient.Length > 100)
                return "Recipient must be less than 100 characters.";
            if (model.PhoneNumber.Length > 20)
                return "Phone number must be less than 20 characters.";
            if (model.AddressDetailRecipient.Length > 200)
                return "Address detail must be less than 200 characters.";
            return string.Empty;
        }

        public async Task<List<OrderAddressGetVModel>> GetAllAsync(int? userId = null)
        {
            var query = _context.OrderAddresses.AsQueryable();
            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);
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
            return new SuccessResponseResult(entity.ToGetVModel(), "Order address created successfully.");
        }

        public async Task<ResponseResult> UpdateAsync(OrderAddressUpdateVModel model)
        {
            var validation = Validate(model);
            if (!string.IsNullOrEmpty(validation))
                return new ErrorResponseResult(validation);

            var entity = await _context.OrderAddresses.FindAsync(model.Id);
            if (entity == null)
                return new ErrorResponseResult("Order address not found.");

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
            return new SuccessResponseResult(entity.ToGetVModel(), "Order address updated successfully.");
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            var entity = await _context.OrderAddresses.FindAsync(id);
            if (entity == null)
                return new ErrorResponseResult("Order address not found.");
            _context.OrderAddresses.Remove(entity);
            await _context.SaveChangesAsync();
            return new SuccessResponseResult(null, "Order address deleted successfully.");
        }
    }
}
