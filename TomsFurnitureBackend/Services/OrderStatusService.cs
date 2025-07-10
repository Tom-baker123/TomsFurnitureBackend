using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly TomfurnitureContext _context;

        public OrderStatusService(TomfurnitureContext context)
        {
            _context = context;
        }

        // Validation dùng chung cho thêm/s?a
        public static string Validate(OrderStatusCreateVModel model)
        {
            if (string.IsNullOrWhiteSpace(model.OrderStatusName))
                return "Order status name is required.";
            if (model.OrderStatusName.Length > 100)
                return "Order status name must be less than 100 characters.";
            return string.Empty;
        }

        public async Task<List<OrderStatusGetVModel>> GetAllAsync()
        {
            var list = await _context.OrderStatuses.ToListAsync();
            return list.Select(x => x.ToGetVModel()).ToList();
        }

        public async Task<OrderStatusGetVModel?> GetByIdAsync(int id)
        {
            var entity = await _context.OrderStatuses.FindAsync(id);
            return entity?.ToGetVModel();
        }

        public async Task<ResponseResult> CreateAsync(OrderStatusCreateVModel model)
        {
            var validation = Validate(model);
            if (!string.IsNullOrEmpty(validation))
                return new ErrorResponseResult(validation);

            var entity = model.ToEntity();
            _context.OrderStatuses.Add(entity);
            await _context.SaveChangesAsync();
            return new SuccessResponseResult(entity.ToGetVModel(), "Order status created successfully.");
        }

        public async Task<ResponseResult> UpdateAsync(OrderStatusUpdateVModel model)
        {
            var validation = Validate(model);
            if (!string.IsNullOrEmpty(validation))
                return new ErrorResponseResult(validation);

            var entity = await _context.OrderStatuses.FindAsync(model.Id);
            if (entity == null)
                return new ErrorResponseResult("Order status not found.");

            entity.UpdateEntity(model);
            await _context.SaveChangesAsync();
            return new SuccessResponseResult(entity.ToGetVModel(), "Order status updated successfully.");
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            var entity = await _context.OrderStatuses.FindAsync(id);
            if (entity == null)
                return new ErrorResponseResult("Order status not found.");
            _context.OrderStatuses.Remove(entity);
            await _context.SaveChangesAsync();
            return new SuccessResponseResult(null, "Order status deleted successfully.");
        }
    }
}
